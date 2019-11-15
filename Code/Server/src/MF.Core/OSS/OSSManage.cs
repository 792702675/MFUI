using Abp.AutoMapper;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Aliyun.OSS;
using Microsoft.EntityFrameworkCore;
using MF.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Threading;
using MF.Buckets;
using MF.FS430;
using System.Net;
using Newtonsoft.Json;
using Abp.BackgroundJobs;
using Abp.Dependency;

namespace MF.OSS
{
    /// <summary>
    /// 文件系统
    /// 规则：
    /// 0、规定文件路径的分割符使用‘/’，而且分割的地方只能有一个符号
    /// 1、任何时候路径都不能以‘/’开头
    /// 2、以‘/’结尾的表示文件夹，否则认为是文件
    /// </summary>
    public class OSSManage :
       MFDomainServiceBase
       //, IAsyncEventHandler<OssDataChanged>
       , IAsyncEventHandler<EntityChangingEventData<OSSObject>>, ITransientDependency, ISingletonDependency
    {
        private string CacheKey => "AliyunOssDirectory";
        private OssClient _client;
        public OssClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new OssClient(
                        SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.Endpoint),
                        SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.AccessKeyId),
                        SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.AccessKeySecret)
                        );
                }
                return _client;
            }
        }


        private readonly ICacheManager _cacheManager;
        private readonly IRepository<OSSObject> _oSSObjectRepository;
        private readonly TagManage _tagManage;
        private readonly IRepository<ObjectTag> _ObjectTagRepository;
        private readonly BucketManage _bucketManage;
        private readonly FS430Manage _fs430Manage;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public OSSManage(
            ICacheManager cacheManager,
            IRepository<OSSObject> oSSObjectRepository,
            TagManage tagManage,
            IRepository<ObjectTag> ObjectTagRepository,
            BucketManage bucketManage,
            FS430Manage fs430Manage,
            IBackgroundJobManager backgroundJobManager)
        {
            _cacheManager = cacheManager;
            _oSSObjectRepository = oSSObjectRepository;
            _tagManage = tagManage;
            _ObjectTagRepository = ObjectTagRepository;
            _bucketManage = bucketManage;
            _fs430Manage = fs430Manage;
            _backgroundJobManager = backgroundJobManager;
        }

        public int SetAllObjectResolution()
        {
            var bucket = _bucketManage.GetBuckets();
            int count = 0;
            foreach (var item in bucket)
            {
                var objs = _oSSObjectRepository.GetAll().Where(x => x.BucketName == item.Name).ToList();
                foreach (var obj in objs)
                {
                    if (obj.Width == 0 && obj.Key.IsImage())
                    {
                        SetResolution(obj);
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 从oss服务器更新本地数据(一般是初始化本地数据时使用)
        /// </summary>
        public string PullData(string bucketName, OssClient client = null)
        {
            var data = GetListObjectAllOfAliyun(bucketName, client);
            int newObjectCount = 0;
            foreach (var item in data)
            {
                if (!_oSSObjectRepository.GetAll().Any(x => x.Key == item.Key))
                {
                    newObjectCount++;
                    _oSSObjectRepository.Insert(item.MapTo<OSSObject>());
                }
            }
            if (newObjectCount > 0)
            {
                EventBus.Default.Trigger(new OssDataChanged(bucketName));
            }
            return newObjectCount + "/" + data.Count;
        }

        /// <summary>
        /// 从oss服务器更新本地数据(一般是初始化本地数据时使用)
        /// </summary>
        public async Task<List<string>> PullAllBucketData()
        {
            var buckets = await _bucketManage.GetBucketsAsync();
            var data = new List<string>();
            foreach (var item in buckets)
            {
                var msg = PullData(item.Name, new OssClient(
                     SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.Endpoint),
                     SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.AccessKeyId),
                     SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.AccessKeySecret)
                     ));

                data.Add(item.Name + ":" + msg);
            }
            return data;
        }



        public bool IsFS430(string key)
        {
            var eName = Path.GetExtension(key).ToLower();
            var fs430EName = SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.FileType).ToLower().ReadAllLine();
            return fs430EName.Contains(eName);
        }
        public bool IsProhibitedFile(string key)
        {
            var eName = Path.GetExtension(key).ToLower();
            var prohibitedFileType = SettingManager.GetSettingValue(AppSettingNames.OSS.ProhibitedFileType).ToLower().ReadAllLine();
            return prohibitedFileType.Contains(eName);
        }

        public async Task<bool> ObjectIsExists(string bucketName, string key)
        {
            return await _oSSObjectRepository
                .GetAll()
                .Where(x => x.BucketName == bucketName)
                .Where(x => x.Key == key)
                .AnyAsync();
        }


        /// <summary>
        /// 创建文件 支持批量上传 (批量上传时存在潜在的缺陷，如果本地数据库操作失败会导致oss服务器的操作不会撤销)
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="obj"></param>
        public async Task CreateObject(string bucketName, string[] tagNames, params (string path, Stream stream)[] objs)
        {
            foreach (var item in objs)
            {
                await CreateObject(bucketName, item.path, item.stream, tagNames);
            }
        }
        public async Task<(Guid gruop, OssETagFileDto fileInfo)> CreateObjectRelatedToThisArticle(Guid? group, string fileName, Stream stream)
        {
            group = group ?? Guid.NewGuid();
            var bucketName = SettingManager.GetSettingValue(AppSettingNames.OSS.ContextStore);
            var key = $"ContextStore/{group}/{fileName}";
            var etag = await CreateObject(bucketName, key, stream);
            var fileInfo = new OssETagFileDto() { BucketName = bucketName, ETag = etag, Url = GetUrlOfKey(bucketName, key) };
            return (group.Value, fileInfo);
        }

        public async Task<string> CreateObject(string bucketName, string path, Stream stream, string[] tagNames = null)
        {
            if (IsProhibitedFile(path))
            {
                throw new Abp.UI.UserFriendlyException("不允许的文件类型！");
            }
            var key = path.TrimStart('/');
            if (await _oSSObjectRepository.GetAll().AnyAsync(x => x.BucketName.ToLower() == bucketName.ToLower() && x.Key.ToLower() == key.ToLower()))
            {
                throw new Abp.UI.UserFriendlyException("同名文件已存在，不允许覆盖！");
            }

            // 上传文件夹时，指定的文件路径中的文件夹可能不存在，需要即时创建
            await CreatePrefixDirectory(bucketName, key, tagNames);

            var data = new OSSObject
            {
                BucketName = bucketName,
                Key = key,
                Size = stream.Length,
            };
            _oSSObjectRepository.Insert(data);
            UnitOfWorkManager.Current.SaveChanges();

            UnitOfWorkManager.Current.SaveChanges();
            data.ETag = await Save(bucketName, key, stream);
            SetResolution(data);

            // 应用携带的tag
            await AddTag(bucketName, key, tagNames);
            UnitOfWorkManager.Current.SaveChanges();

            EventBus.Default.Trigger(new OssDataChanged(bucketName));
            return data.ETag;

        }
        /// <summary>
        /// 设置分辨率
        /// </summary>
        private void SetResolution(OSSObject oSSObject)
        {
            if (oSSObject.Key.IsImage())
            {
                var url = GetUrlOfKey(oSSObject.BucketName, oSSObject.Key) + "?x-oss-process=image/info";
                try
                {
                    var json = new WebClient().DownloadString(url);
                    var result = JsonConvert.DeserializeObject<dynamic>(json);
                    oSSObject.Width = result.ImageWidth.value;
                    oSSObject.Height = result.ImageHeight.value;
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message);
                }
            }
        }

        /// <summary>
        /// 上传文件夹时，指定的文件路径中的文件夹可能不存在，需要即时创建
        /// </summary>
        private async Task CreatePrefixDirectory(string bucket, string key, string[] tags)
        {
            var dirs = key.GetFolders();
            foreach (var item in dirs)
            {
                // 因为创建父级有并发的问题，所以改为后台工作者来创建
                await _backgroundJobManager.EnqueueAsync<OssCreateFolderJob, CreateFolderDto>(new CreateFolderDto
                {
                    BucketName = bucket,
                    Key = item,
                    Tags = tags,
                    IsHidden = false
                });
            }
        }
        /// <summary>
        /// 继承父级文件夹的tag  现在由ui端自动添加，允许操作人员修改
        /// </summary>
        /// <param name="id">objectId</param>
        /// <param name="key">objKey</param>
        private async Task ApplyParentTag(int id, string key)
        {
            var dir = Path.GetDirectoryName(key.TrimEnd('/')).EnsureEndsWith('/').CorrectKey();
            var p = await _oSSObjectRepository.GetAll().Where(x => x.Key == dir).FirstOrDefaultAsync();
            if (p != null)
            {
                var tags = _ObjectTagRepository.GetAll().Where(x => x.OSSObjectId == p.Id).Select(x => x.Tag.Name).ToArray();
                await UpdateItemTag(id, tags);
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
        /// <summary>
        /// 存储策略
        /// </summary>
        private async Task<string> Save(string bucketName, string key, Stream stream)
        {
            if (IsFS430(key))
            {
                return await _fs430Manage.UploadFileAsync(Combine(bucketName, key), stream);
            }
            else
            {
                var pr = Client.PutObject(bucketName, key, stream);
                return pr.ETag;
            }
        }
        PutObjectResult PutObject(string bucketName, string key, Stream stream)
        {
            var eName = Path.GetExtension(key);
            if (new string[] { ".pdf" }.Contains(eName.ToLower()))
            {
                var om = new ObjectMetadata();
                om.AddHeader("Access-Control-Allow-Origin", "*");
                om.AddHeader("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
                om.AddHeader("Access-Control-Allow-Headers", "*");
                return Client.PutObject(bucketName, key, stream, om);
            }
            else
            {
                return Client.PutObject(bucketName, key, stream);
            }
        }

        /// <summary>
        /// 创建文件夹 ,如果存在则不会创建
        /// </summary>
        public async Task CreateFolder(string bucketName, string folder, string[] tagNames = null, bool isHidden = false)
        {
            folder = folder.EnsureEndsWith('/').CorrectKey();

            if (await _oSSObjectRepository.GetAll().AnyAsync(x => x.BucketName == bucketName && x.Key == folder))
            {
                return;
            }
            var data = new OSSObject
            {
                BucketName = bucketName,
                Key = folder,
                Size = 0,
                IsHidden = isHidden
            };
            _oSSObjectRepository.Insert(data);
            UnitOfWorkManager.Current.SaveChanges();

            await AddTag(bucketName, folder, tagNames);

            var pr = Client.PutObject(bucketName, folder, new MemoryStream());
            data.ETag = pr.ETag;


            EventBus.Default.Trigger(new OssDataChanged(bucketName));
        }
        private async Task<OSSObject> GetOrCreate(OSSObject obj)
        {
            var _obj = await _oSSObjectRepository.GetAll()
                .Where(x => x.BucketName.ToLower() == obj.BucketName.ToLower())
                .Where(x => x.Key == obj.Key)
                .FirstOrDefaultAsync();
            if (_obj == null)
            {
                _obj = obj;
                await _oSSObjectRepository.InsertAndGetIdAsync(_obj);
            }
            return _obj;
        }


        /// <summary>
        /// 删除文件或目录
        /// </summary>
        public async Task DeleteObject(string bucketName, string key)
        {
            if (!key.IsFile())
            {
                await DeleteDirectory(bucketName, key);
            }
            else
            {
                await DeleteFile(bucketName, key);
            }
        }

        /// <summary>
        /// 删除一个文件
        /// </summary>
        private async Task DeleteFile(string bucketName, string key)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                _oSSObjectRepository.Delete(x => x.BucketName == bucketName && x.Key == key);
                UnitOfWorkManager.Current.SaveChanges();

                if (IsFS430(key))
                    await _fs430Manage.DeleteAsync(Combine(bucketName, key));
                else
                    Client.DeleteObject(bucketName, key);

                uow.Complete();
            }

            EventBus.Default.Trigger(new OssDataChanged(bucketName));
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        private async Task DeleteDirectory(string bucketName, string key)
        {
            var subs = GetObjectListOfDirectory(bucketName, key);
            foreach (var item in subs)
            {
                if (item.Key.IsFile())
                {
                    await DeleteFile(item.BucketName, item.Key);
                }
                else
                {
                    await DeleteDirectory(item.BucketName, item.Key);
                }
            }

            // 删除目录自己
            await _oSSObjectRepository.DeleteAsync(x => x.BucketName == bucketName && x.Key == key);
            Client.DeleteObject(bucketName, key);

            EventBus.Default.Trigger(new OssDataChanged(bucketName));
        }

        /// <summary>
        /// 删除文件或目录 批量
        /// </summary>
        public async Task<bool> DeleteObjects(DeleteObjectsRequest input)
        {
            foreach (var key in input.Keys)
            {
                await DeleteObject(input.BucketName, key);
            }
            return true;
        }

        /// <summary>
        /// 删除文件或目录 (批量）
        /// </summary>
        private async Task DeleteBatch(ObjectKey[] source)
        {
            foreach (var item in source)
            {
                await DeleteObject(item.BucketName, item.Key);
            }
        }

        /// <summary>
        /// 获取指定对象的Tags
        /// </summary>
        public async Task<string[]> GetTags(string bucketName, string key)
        {
            return
            await _ObjectTagRepository
                .GetAll()
                .Include(x => x.OSSObject)
                .Include(x => x.Tag)
                .Where(x => x.OSSObject.BucketName == bucketName)
                .Where(x => x.OSSObject.Key == key)
                .Select(x => x.Tag.Name)
                .ToArrayAsync();
        }

        private async Task CopyTag(string sourceBucketName, string sourceKey, int destinationObjectId)
        {
            var sourceTags = await _ObjectTagRepository
                .GetAll()
                .Include(x => x.OSSObject)
                .Where(x => x.OSSObject.BucketName == sourceBucketName)
                .Where(x => x.OSSObject.Key == sourceKey)
                .Select(x => x.TagId)
                .ToListAsync();

            foreach (var item in sourceTags.Select(x => new ObjectTag { OSSObjectId = destinationObjectId, TagId = x }))
            {
                await _ObjectTagRepository.InsertAsync(item);
            }
        }
        /// <summary>
        /// 复制一个文件  到文件夹
        /// </summary>
        private async Task CopyFileToFile(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationKey)
        {
            if (!sourceKey.IsFile() || !destinationKey.IsFile())
            {
                throw new UserFriendlyException("源和目标需要是文件！");
            }
            await DeleteFile(destinationBucketName, destinationKey); // 存在就覆盖

            var data = new OSSObject
            {
                BucketName = destinationBucketName,
                Key = destinationKey,
            };
            await _oSSObjectRepository.InsertAndGetIdAsync(data);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (IsFS430(sourceKey))
            {
                await _fs430Manage.CopyAsync(Combine(sourceBucketName, sourceKey), Combine(destinationBucketName, destinationKey));
            }
            else
            {
                if (!Client.DoesObjectExist(sourceBucketName, sourceKey))
                {
                    return;
                }
                var req = new CopyObjectRequest(sourceBucketName, sourceKey, destinationBucketName, destinationKey);
                Client.CopyObject(req);
            }
            var sourceFileInfo = await _oSSObjectRepository.GetAll().Where(x => x.BucketName == sourceBucketName && x.Key == sourceKey).FirstOrDefaultAsync();
            data.ETag = sourceFileInfo?.ETag;
            data.Size = sourceFileInfo?.Size ?? 0;
            data.LastModified = sourceFileInfo?.LastModified;

            await CopyTag(sourceBucketName, sourceKey, data.Id);
            EventBus.Default.Trigger(new OssDataChanged(destinationBucketName));
        }
        /// <summary>
        /// 复制一个文件  到文件夹
        /// </summary>
        private async Task CopyFileToDirectory(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationDirectory)
        {
            if (!sourceKey.IsFile() || destinationDirectory.IsFile())
            {
                throw new UserFriendlyException("源需要是文件，目标需要是文件夹！");
            }
            if (sourceBucketName == destinationBucketName && Path.GetDirectoryName(sourceKey).CorrectKey() == destinationDirectory)
            {
                throw new UserFriendlyException("目标目录不能是当前目录！");
            }

            var newFile = (destinationDirectory.EnsureEndsWith('/') + Path.GetFileName(sourceKey)).CorrectKey();
            await CopyFileToFile(sourceBucketName, sourceKey, destinationBucketName, newFile);
        }
        /// <summary>
        /// 复制一个文件夹  到另一个文件夹
        /// </summary>
        private async Task CopyDirectory(string sourceBucketName, string sourceDirectory, string destinationBucketName, string destinationDirectory)
        {
            if (sourceDirectory.IsFile() || destinationDirectory.IsFile())
            {
                throw new UserFriendlyException("源或目标不是文件夹，复制文件请使用“CopyFile”。");
            }
            var newDir = (destinationDirectory.EnsureEndsWith('/') + Path.GetFileName(sourceDirectory.TrimEnd('/')).EnsureEndsWith('/')).CorrectKey();
            var tags = await GetTags(sourceBucketName, sourceDirectory);
            await CreateFolder(destinationBucketName, newDir, tags);


            var dirs = GetObjectListOfDirectory(sourceBucketName, sourceDirectory);
            if (dirs != null && dirs.Count > 0)
            {
                foreach (var item in dirs)
                {
                    if (item.Key.IsFile())
                    {
                        await CopyFileToDirectory(item.BucketName, item.Key, destinationBucketName, newDir);
                    }
                    else
                    {
                        await CopyDirectory(item.BucketName, item.Key, destinationBucketName, newDir);
                    }
                }
            }

            EventBus.Default.Trigger(new OssDataChanged(destinationBucketName));
        }

        /// <summary>
        /// 复制 (批量）(递归)
        /// </summary>
        public async Task CopyBatch(BatchToDestination input)
        {
            if (input.Destination.Key.IsFile() && input.Source.Count() > 1)
            {
                throw new UserFriendlyException("参数错误。");
            }
            input.Destination.Key = input.Destination.Key ?? "";

            foreach (var item in input.Source)
            {
                if (item.Key.IsFile())
                {
                    await CopyFileToDirectory(item.BucketName, item.Key, input.Destination.BucketName, input.Destination.Key);
                }
                else
                {
                    await CopyDirectory(item.BucketName, item.Key, input.Destination.BucketName, input.Destination.Key);
                }
            }
        }

        /// <summary>
        /// 移动文件到文件夹
        /// </summary>
        public async Task MoveFile(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationDirectory)
        {
            await CopyFileToDirectory(sourceBucketName, sourceKey, destinationBucketName, destinationDirectory);
            await DeleteObject(sourceBucketName, sourceKey);
        }

        /// <summary>
        /// 移动文件夹到另一个文件夹
        /// </summary>
        public async Task MoveDirectory(string sourceBucketName, string sourceDirectory, string destinationBucketName, string destinationDirectory)
        {
            await CopyDirectory(sourceBucketName, sourceDirectory, destinationBucketName, destinationDirectory);
            await DeleteDirectory(sourceBucketName, sourceDirectory);
        }

        /// <summary>
        /// 移动 (批量）(递归)
        /// </summary>
        public async Task MoveBatch(BatchToDestination input)
        {
            await CopyBatch(input);
            await DeleteBatch(input.Source);
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        public async Task RenameFile(string sourceBucketName, string sourceKey, string destinationKey)
        {
            if (sourceKey == destinationKey)
            {
                throw new UserFriendlyException("和原始一样，不需要重命名");
            }
            if (!sourceKey.IsFile() || !destinationKey.IsFile())
            {
                throw new UserFriendlyException("该处只能重命名文件，如果要重命名文件夹请使用“RenameDirectory”！");
            }
            if (await ObjectIsExists(sourceBucketName, destinationKey))
            {
                throw new UserFriendlyException("命名冲突！");
            }
            await CopyFileToFile(sourceBucketName, sourceKey, sourceBucketName, destinationKey);
            await DeleteFile(sourceBucketName, sourceKey);
        }

        /// <summary>
        /// 重命名文件夹
        /// </summary>
        public async Task RenameDirectory(string sourceBucketName, string sourceDirectory, string destinationDirectory)
        {
            if (sourceDirectory == destinationDirectory)
            {
                throw new UserFriendlyException("和原始一样，不需要重命名");
            }
            if (sourceDirectory.IsFile() || destinationDirectory.IsFile())
            {
                throw new UserFriendlyException("该处只能重命名文件夹，如果要重命名文件请使用“RenameFile”！");
            }
            if (await ObjectIsExists(sourceBucketName, destinationDirectory))
            {
                throw new UserFriendlyException("命名冲突！");
            }

            var tags = await GetTags(sourceBucketName, sourceDirectory);
            await CreateFolder(sourceBucketName, destinationDirectory, tags);

            var subs = GetObjectListOfDirectory(sourceBucketName, sourceDirectory);
            await MoveBatch(new BatchToDestination
            {
                Source = subs.Select(x => new ObjectKey { BucketName = x.BucketName, Key = x.Key }).ToArray(),
                Destination = new ObjectKey { BucketName = sourceBucketName, Key = destinationDirectory }
            });

            await DeleteDirectory(sourceBucketName, sourceDirectory);
        }

        /// <summary>
        /// 重命名文件或者文件夹
        /// </summary>
        public async Task Rename(string sourceBucketName, string sourceKey, string destinationKey)
        {
            if (sourceKey.IsFile() && !destinationKey.IsFile() || !sourceKey.IsFile() && destinationKey.IsFile())
            {
                throw new UserFriendlyException("参数错误！");
            }
            if (sourceKey.IsFile())
            {
                await RenameFile(sourceBucketName, sourceKey, destinationKey);
            }
            else
            {
                await RenameDirectory(sourceBucketName, sourceKey, destinationKey);
            }
        }


        public async Task<OSSObjectDto> GetFileInfo(int id)
        {
            var obj = await _oSSObjectRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            return OssObjectToOSSObjectDto(obj);
        }
        public async Task<OssObject> GetFile(int id)
        {
            var obj = await _oSSObjectRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            var file = Client.GetObject(obj.BucketName, obj.Key);
            return file;
        }

        /// <summary>
        /// 获取文件夹和文件(全部)（从远程）
        /// </summary>
        public List<OssObjectSummary> GetListObjectAllOfAliyun(string bucketName, OssClient client = null)
        {
            if (client == null)
            {
                client = Client;
            }
            ObjectListing item = client.ListObjects(bucketName);
            var data = item.ObjectSummaries.ToList();
            while (item.IsTruncated)
            {
                item = client.ListObjects(
                    new ListObjectsRequest(bucketName)
                    {
                        MaxKeys = 1000,
                        Marker = item.NextMarker
                    });
                data.AddRange(item.ObjectSummaries);
            }
            return data;
        }

        /// <summary>
        /// 获取文件夹和文件(全部)（从数据库）
        /// </summary>
        private List<OSSObject> GetListObjectAllOfDb(string bucketName)
        {
            return _oSSObjectRepository
                .GetAll()
                .Where(x => x.BucketName == bucketName)
                .ToList();
        }


        /// <summary>
        /// 获取目录下的内容
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="root">目录</param>
        /// <param name="directoryOnly">是否只要目录</param>
        /// <returns></returns>
        public List<OSSObjectDto> GetObjectListOfDirectory(string bucketName, string root, bool directoryOnly = false)
        {
            root = root ?? "";

            var hiddenObjectAndSubs = GetHiddenObjects(bucketName);
            var dirs =
                _oSSObjectRepository
                .GetAll()
                .Where(x => x.BucketName == bucketName)
                .WhereIf(!(root.IsNullOrEmpty() || root == "/"), x => x.Key.StartsWith(root))
                .Where(x => !hiddenObjectAndSubs.Contains(x.Id)) // 不显示隐藏文件
                ;

            var data = new List<OSSObjectDto>();
            root = root.TrimStart('/');
            foreach (var dir in dirs)
            {
                var item = dir.Key?.RemoveStart(root).TrimStart('/');
                if (item.IsNullOrEmpty()) { continue; }
                var isFile = !item.Contains("/");
                var name = item.Split("/")[0];
                var _key = root.TrimEnd('/') + "/" + name + (!isFile ? "/" : "");
                _key = _key.TrimStart('/');

                var dto = new OSSObjectDto(
                    dir.BucketName,
                    dir.ETag,
                    _key,
                    GetUrlOfKey(dir.BucketName, _key),
                    dir.Size,
                    dir.LastModified ?? dir.CreationTime);
                dto.Id = dir.Id;

                if (dto.Name.IsNullOrEmpty() || data.Any(x => x.Name == dto.Name && x.IsFile == dto.IsFile))
                {
                    continue;
                }
                if (directoryOnly && dto.IsFile)
                {
                    continue;
                }
                dto.TagNames = _ObjectTagRepository
                    .GetAll()
                    .Include(x => x.OSSObject)
                    .Include(x => x.Tag)
                    .Where(x => x.OSSObject.Key == dto.Key)
                    .Select(x => x.Tag.Name)
                    .ToArray();
                data.Add(dto);
            }

            return data/*.OrderBy(x => x.IsFile)*/.ToList();
        }
        public IQueryable<int> GetHiddenObjects(string bucketName)
        {
            var hiddenObjectKeys = _oSSObjectRepository
                .GetAll()
                .Where(x => x.BucketName == bucketName)
                .Where(x => x.IsHidden)
                .Select(x => x.Key);
            var hiddenObjectAndSubs = _oSSObjectRepository
                .GetAll()
                .Where(x => x.BucketName == bucketName)
                .Where(x => hiddenObjectKeys.Any(k => x.Key.StartsWith(k)))
                .Select(x => x.Id);
            return hiddenObjectAndSubs;
        }
        public IQueryable<int> GetHiddenObjects(IEnumerable<string> bucketNames)
        {
            if (bucketNames == null || !bucketNames.Any())
            {
                return _oSSObjectRepository
                .GetAll()
                .Where(x => x.IsHidden)
                .Select(x => x.Id);
            }
            var q = _oSSObjectRepository.GetAll().Where(x => false).Select(x => x.Id);

            foreach (var item in bucketNames)
            {
                q = q.Concat(GetHiddenObjects(item));
            }

            return q;
        }


        //public OSSObjectDto OssObjectSummaryToOSSObjectDto(OssObjectSummary ossObjectSummary)
        //{
        //    return new OSSObjectDto
        //    (
        //        ossObjectSummary.BucketName,
        //        SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.Endpoint),
        //        ossObjectSummary.ETag,
        //        ossObjectSummary.Key,
        //        ossObjectSummary.Size,
        //        ossObjectSummary.LastModified
        //    );
        //}

        public OSSObjectDto OssObjectToOSSObjectDto(OSSObject ossObject)
        {
            if (ossObject == null) { return null; }
            return new OSSObjectDto
            (
                ossObject.BucketName,
                ossObject.ETag,
                ossObject.Key,
                GetUrlOfKey(ossObject.BucketName, ossObject.Key),
                ossObject.Size,
                ossObject.LastModified
            )
            {
                TagNames = _ObjectTagRepository
                    .GetAll()
                    .Include(x => x.OSSObject)
                    .Include(x => x.Tag)
                    .Where(x => x.OSSObject.Key == ossObject.Key)
                    .Select(x => x.Tag.Name)
                    .ToArray(),
                Width = ossObject.Width,
                Height = ossObject.Height,
                Id = ossObject.Id,
            };
        }


        /// <summary>
        /// 更新一个对象的标签
        /// </summary>
        public async Task UpdateItemTag(int objId, string[] tagNames)
        {
            var data = await _oSSObjectRepository.GetAsync(objId);
            await _ObjectTagRepository.DeleteAsync(x => x.OSSObjectId == data.Id);

            if (tagNames != null && tagNames.Length > 0)
            {
                var tags = _tagManage.GetTagId(tagNames);
                foreach (var item in tags)
                {
                    data.ObjectTags.Add(new ObjectTag { OSSObjectId = data.Id, TagId = item });
                }
            }
        }

        /// <summary>
        /// 更新一个对象的标签
        /// </summary>
        public async Task UpdateItemTag(string bucketName, string key, string[] tagNames)
        {
            var obj = _oSSObjectRepository.GetAll().Where(x => x.BucketName == bucketName && x.Key == key).FirstOrDefault();
            if (obj == null)
            {
                //throw new UserFriendlyException("找不到指定的Key");
                return;
            }
            await UpdateItemTag(obj.Id, tagNames);
        }
        /// <summary>
        /// 更新文件夹或者文件的标签，如果是文件夹&ApplyAllChild=true 则子文件也会被更新标记
        /// </summary>
        public async Task UpdateDirectoryOrFileTag(UpdateTagInput input)
        {
            await UpdateItemTag(input.BucketName, input.Key, input.TagNames);

            if (!input.Key.IsFile() && input.ApplyAllChild)
            {
                var subs = GetObjectListOfDirectory(input.BucketName, input.Key);
                foreach (var item in subs)
                {
                    await UpdateDirectoryOrFileTag(new UpdateTagInput
                    {
                        BucketName = item.BucketName,
                        Key = item.Key,
                        TagNames = input.TagNames,
                        ApplyAllChild = input.ApplyAllChild
                    });
                }
            }
        }

        /// <summary>
        /// 加标签 (递归)
        /// </summary>
        public async Task AddTag(string bucketName, string key, string[] tagNames)
        {
            await AddOrSubTag(bucketName, key, tagNames, true);
        }

        /// <summary>
        /// 减标签 (递归)
        /// </summary>
        public async Task SubTag(string bucketName, string key, string[] tagNames)
        {
            await AddOrSubTag(bucketName, key, tagNames, false);
        }

        private async Task AddOrSubTag(string bucketName, string key, string[] tagNames, bool IsAdd)
        {
            if (tagNames == null || tagNames.Length < 1)
            {
                return;
            }
            var obj = await _oSSObjectRepository
                .GetAll()
                .Include(x => x.ObjectTags)
                .Include("ObjectTags.Tag")
                .Where(x => x.BucketName == bucketName)
                .WhereIf(key.IsFile(), x => x.Key == key)
                .WhereIf(!key.IsFile(), x => x.Key.StartsWith(key))
                .ToListAsync();
            foreach (var item in obj)
            {
                //if (!item.Key.IsFile())
                //{
                //    // 文件夹不用加标签
                //    continue;
                //}
                var tags = item.ObjectTags.Select(o => o.Tag.Name).ToList();
                var tagsR =
                    IsAdd ?
                    tags.Union(tagNames).Distinct().ToArray() :
                    tags.Except(tagNames).Distinct().ToArray();

                await UpdateItemTag(item.Id, tagsR);
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        /// <summary>
        /// [在实体change时] 更新实体的 Name和ExtensionName
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public async Task HandleEventAsync(EntityChangingEventData<OSSObject> eventData)
        {
            await Task.FromResult(0);
            eventData.Entity.SetNameAndExtensionName();
        }

        /// <summary>
        /// 获取文件的Url
        /// </summary>
        public string GetUrlOfETag(string etag, string bucketName = null)
        {
            var obj = _oSSObjectRepository
                .GetAll()
                .Where(x => x.ETag == etag)
                .WhereIf(bucketName != null, x => x.BucketName == bucketName)
                .FirstOrDefault();
            if (obj == null) { return ""; }
            return GetUrlOfKey(obj.BucketName, obj.Key);
        }
        public string GetUrlOfKey(string bucketName, string key)
        {
            var endpoint = SettingManager.GetSettingValue(AppSettingNames.OSS.Aliyun.Endpoint);
            if (IsFS430(key))
            {
                return _fs430Manage.GetFileUrl(bucketName + "/" + key);
            }
            else
            {
                endpoint = endpoint.Replace("http://", $"http://{bucketName}.");
                endpoint = endpoint.Replace("https://", $"https://{bucketName}.");
                return $"{endpoint}/{key}";
            }
        }
        /// <summary>
        /// 获取文件的缩略图Url
        /// </summary>
        public virtual string GetThumbnail(string etag, string bucketName = null)
        {
            return GetUrlOfETag(etag, bucketName) + "?x-oss-process=image/resize,m_fill,w_120,h_90";
        }

        private string Combine(params string[] path)
        {
            var _path = path[0];
            foreach (var item in path.Skip(1))
            {
                _path = _path.EnsureEndsWith('/') + item.TrimStart('/');
            }
            return _path;
        }

        public OSSObject GetOSSObject(string etag, string bucketName = null)
        {
            var obj = _oSSObjectRepository
                .GetAll()
                .Where(x => x.ETag == etag)
                .WhereIf(bucketName != null, x => x.BucketName == bucketName)
                .FirstOrDefault();
            return obj;
        }
    }
}
