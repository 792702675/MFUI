using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using MF.Authentication.External;
using MF.Authentication.JwtBearer;
using MF.Authorization;
using MF.Authorization.Users;
using MF.Models.TokenAuth;
using MF.MultiTenancy;
using MF.OSS;
using Aliyun.OSS;
using System.IO;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Extensions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Abp.AutoMapper;
using Abp.AspNetCore.Mvc.Authorization;
using MF.Buckets;

namespace MF.Controllers
{
    /// <summary>
    /// 阿里OSS存储库管理  管理人员使用
    /// </summary>
    [Route("api/[controller]/[action]")]
    [AbpMvcAuthorize(PermissionNames.Oss_Aliyun_Manage)]
    public class AliyunOssController : MFControllerBase
    {

        private readonly OSSManage _aliyunOSSManager;
        private readonly BucketManage _bucketManage;
        private readonly IRepository<OSSObject> _oSSObjectRepository;

        public AliyunOssController(
            IRepository<OSSObject> oSSObjectRepository,
            OSSManage aliyunOSSManager,
            BucketManage bucketManage)
        {
            _aliyunOSSManager = aliyunOSSManager;
            _oSSObjectRepository = oSSObjectRepository;
            _bucketManage = bucketManage;
        }

        [HttpPost]
        public int SetAllObjectResolution()
        {
            return _aliyunOSSManager.SetAllObjectResolution();
        }

        /// <summary>
        /// 从oss服务器更新本地数据(一般是初始化本地数据时使用)
        /// </summary>
        [HttpPost]
        public string PullData(string bucketName)
        {
            return _aliyunOSSManager.PullData(bucketName);
        }

        /// <summary>
        /// 从oss服务器更新本地数据(一般是初始化本地数据时使用)
        /// </summary>
        [HttpPost]
        public async Task<List<string>> PullAllBucketData()
        {
            return await _aliyunOSSManager.PullAllBucketData();
        }
        /// <summary>
        /// 获取存储库列表（List Bucket）
        /// </summary>
        [HttpGet]
        public async Task<List<Bucket430>> GetBuckets()
        {
            return await _bucketManage.GetBucketsAsync();
        }

        /// <summary>
        /// 检查是否还能创建存储库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> CanCreateAsync()
        {
            return await _bucketManage.CanCreateAsync();
        }

        /// <summary>
        /// 创建存储库（Create Bucket）
        /// </summary>
        [HttpPost]
        public async Task<Bucket430> CreateBucket(string bucketName, string note)
        {
            return await _bucketManage.CreateAsync(bucketName, note);
        }


        /// <summary>
        /// 删除存储库（Delete Bucket）
        /// </summary>
        [HttpDelete]
        public async Task DeleteBucket(string bucketName)
        {
            await _bucketManage.DeleteAsync(bucketName);
        }

        ///// <summary>
        ///// 获取文件夹和文件
        ///// </summary>
        //[HttpGet]
        //public ObjectListing GetListObject(GetListObjectInput input)
        //{
        //    return _aliyunOSSManager.Client.ListObjects(input.ToRequest());
        //}
        ///// <summary>
        ///// 获取文件夹和文件(全部)
        ///// </summary>
        //[HttpGet]
        //public IEnumerable<OssObjectSummary> GetListObjectAll(string bucketName)
        //{
        //    return _aliyunOSSManager.GetListObjectAllOfAliyun(bucketName);
        //}

        /// <summary>
        /// 获取目录下的内容
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="root">目录</param>
        /// <param name="directoryOnly">是否只要目录</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        [HttpGet]
        public List<OSSObjectDto> GetObjectListOfDirectory(string bucketName, string root, bool directoryOnly = false, string sorting = "")
        {
            var q = _aliyunOSSManager.GetObjectListOfDirectory(bucketName, root, directoryOnly).AsQueryable();
            q = Order(q, sorting);
            return q.ToList();
        }
        private IQueryable<OSSObjectDto> Order(IQueryable<OSSObjectDto> q, string sorting)
        {
            var sortingLower = sorting.ToLower();
            if (sortingLower.Split(' ').Contains("name"))
            {
                if (sortingLower.Contains("desc"))
                {
                    q = q.OrderByDescending(x => x.Name, new FileNameComparer());
                }
                else
                {
                    q = q.OrderBy(x => x.Name, new FileNameComparer());
                }
            }
            else if (sortingLower == "")
            {   
                // 默认排序
                q = q.OrderBy("IsFile").ThenBy(x => x.Name, new FileNameComparer());
            }
            else
            {
                q = q.OrderBy(sorting);
            }
            return q;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        [HttpGet]
        public async Task<List<OSSObjectDto>> Search(SearchInput input)
        {
            var root = input.Root ?? "";
            var q = _oSSObjectRepository
                .GetAll()
                .Where(x => x.BucketName == input.BucketName)
                .WhereIf(!(root.IsNullOrEmpty() || root == "/"), x => x.Key.StartsWith(root))
                .WhereIf(!input.Name.IsNullOrEmpty(), x => x.Name.Contains(input.Name))
                .WhereIf(input.TagNames != null && input.TagNames.Length > 0, x => x.ObjectTags.Select(o => o.Tag.Name.ToLower()).Intersect(input.TagNames.Select(t => t.ToLower())).Any())
                .WhereIf(input.ExtensionNames != null && input.ExtensionNames.Length > 0, x => input.ExtensionNames.Select(t => t.ToLower()).Any(e => x.ExtensionName.ToLower().Contains(e)))
                .OrderBy(input.Sorting)
                ;

            var data = await q.ToListAsync();
            var dataR = data
                .Select(x => _aliyunOSSManager.OssObjectToOSSObjectDto(x))
                .ToList();
            return dataR;
        }

        [HttpGet]
        public PartListing GetParts(GetPartsInput input)
        {
            return _aliyunOSSManager.Client.ListParts(input.ToRequest());
        }
        /// <summary>
        /// 获取指定对象的Tags
        /// </summary>
        [HttpGet]
        public async Task<string[]> GetTags(string bucketName, string key)
        {
            return await _aliyunOSSManager.GetTags(bucketName, key);
        }

        /// <summary>
        /// 创建文件 支持批量上传
        /// </summary>
        /// <param name="BucketName"></param>
        /// <param name="Path">位置文件夹</param>
        /// <param name="tagNames">标签</param>
        /// <param name="filePath"> 上传文件夹时，带了前缀路径的文件名 如：123/qwe/a.txt</param>
        [HttpPost]
        [RequestSizeLimit(4000_000_000)] //最大4G左右
        public async Task CreateObject(string BucketName, string Path, string[] tagNames, string filePath)
        {
            Path = Path ?? "";
            var files = Request.Form.Files;
            if (files.Count < 1)
            {
                throw new Abp.UI.UserFriendlyException("没有文件！");
            }
            var fileInfo = files
                .Select(x => ((Path + "/" + (filePath ?? x.FileName)).CorrectKey(), x.OpenReadStream()))
                .ToArray();
            await _aliyunOSSManager.CreateObject(BucketName, tagNames, fileInfo);
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="input"></param>
        [HttpPost]
        public async Task CreateFolder(CreateFolderInput input)
        {
            await _aliyunOSSManager.CreateFolder(input.BucketName, input.Folder, input.TagNames);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="input"></param>
        [HttpGet]
        public FileResult GetObject(ObjectKeyInput input)
        {
            var file = _aliyunOSSManager.Client.GetObject(input.BucketName, input.FileName);
            return File(file.Content, file.Metadata.ContentType);
        }

        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="input"></param>
        [HttpDelete]
        public async Task DeleteObject(ObjectKeyInput input)
        {
            await _aliyunOSSManager.DeleteObject(input.BucketName, input.FileName);
        }

        /// <summary>
        /// 删除文件 批量
        /// </summary>
        /// <param name="input"></param>
        [HttpDelete]
        public async Task<bool> DeleteObjects(/*[FromBody]*/DeleteObjectsInput input)
        {
            return await _aliyunOSSManager.DeleteObjects(input.ToRequset());
        }


        /// <summary>
        /// 复制 
        /// </summary>
        [HttpPost]
        public async Task Copy(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationKey)
        {
            await _aliyunOSSManager.CopyBatch(
                new BatchToDestination()
                {
                    Source = new ObjectKey[]
                    {
                        new ObjectKey
                        {
                            BucketName = sourceBucketName, Key = sourceKey
                        }
                    },
                    Destination = new ObjectKey { BucketName = destinationBucketName, Key = destinationKey }
                });
        }
        /// <summary>
        /// 复制 (批量）(递归)
        /// </summary>
        [HttpPost]
        public async Task CopyBatch([FromBody]BatchToDestination input)
        {
            await _aliyunOSSManager.CopyBatch(input);
        }

        /// <summary>
        /// 重命名
        /// </summary>
        [HttpPut]
        public async Task Rename(string sourceBucketName, string sourceKey, string destinationKey)
        {
            await _aliyunOSSManager.Rename(sourceBucketName, sourceKey, destinationKey);
        }

        /// <summary>
        /// 移动  
        /// </summary>
        [HttpPut]
        public async Task Move(string sourceBucketName, string sourceKey, string destinationBucketName, string destinationKey)
        {
            if (destinationKey.IsFile() && _oSSObjectRepository.GetAll().Any(x => x.BucketName == destinationBucketName && x.Key == destinationKey))
            {
                throw new Abp.UI.UserFriendlyException("目标文件已存在，不允许覆盖");
            }
            await _aliyunOSSManager.MoveBatch(
                new BatchToDestination()
                {
                    Source = new ObjectKey[]
                    {
                        new ObjectKey
                        {
                            BucketName = sourceBucketName, Key = sourceKey
                        }
                    },
                    Destination = new ObjectKey { BucketName = destinationBucketName, Key = destinationKey }
                });
        }

        /// <summary>
        /// 移动 (批量）(递归)
        /// </summary>
        [HttpPost]
        public async Task MoveBatch([FromBody]BatchToDestination input)
        {
            await _aliyunOSSManager.MoveBatch(input);
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        [HttpPut]
        public async Task UpdateTag(UpdateTagInput input)
        {
            await _aliyunOSSManager.UpdateDirectoryOrFileTag(input);
        }

        /// <summary>
        /// 加标签 (批量）(递归)
        /// </summary>
        [HttpPut]
        public async Task BatchAddTag([FromBody]BatchUpdateTagInput input)
        {
            foreach (var item in input.Source)
            {
                await _aliyunOSSManager.AddTag(item.BucketName, item.Key, input.TagNames);
            }
        }
        /// <summary>
        /// 减标签 (批量）(递归)
        /// </summary>
        [HttpPut]
        public async Task BatchSubTag([FromBody]BatchUpdateTagInput input)
        {
            foreach (var item in input.Source)
            {
                await _aliyunOSSManager.SubTag(item.BucketName, item.Key, input.TagNames);
            }
        }


    }
}
