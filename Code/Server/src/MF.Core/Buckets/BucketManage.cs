using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Threading;
using Aliyun.OSS;
using MF.Commons;
using MF.Configuration;
using MF.FS430;
using MF.OSS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MF.Buckets
{
    public class BucketManage : MFDomainServiceBase
    {
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


        private readonly IRepository<Bucket430> _bucketRepository;
        private readonly IRepository<OSSObject> _oSSObjectRepository;
        private readonly FS430Manage _fs430Manger;

        public BucketManage(
            IRepository<Bucket430> BucketRepository,
            IRepository<OSSObject> oSSObjectRepository,
            FS430Manage webClient)
        {
            _bucketRepository = BucketRepository;
            _oSSObjectRepository = oSSObjectRepository;
            _fs430Manger = webClient;
        }

        /// <summary>
        /// 检查是否还能创建存储库
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CanCreateAsync()
        {
            var alreadyBucketCount = await _bucketRepository.GetAll().Where(x => x.Owner == MFConsts.SystemName).CountAsync();
            var settingBucketCount = SettingManager.GetSettingValue<int>(AppSettingNames.OSS.BucketCounct);
            return settingBucketCount > alreadyBucketCount;
        }

        /// <summary>
        /// 创建存储库
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public async Task<Bucket430> CreateAsync(string bucketName, string note)
        {
            if (!await CanCreateAsync())
            {
                throw new Abp.UI.UserFriendlyException("存储库已达到上限，不能再创建。");
            }
            bucketName = SettingManager.GetSettingValue(AppSettingNames.OSS.BucketPrefix) + bucketName;
            if (await _bucketRepository.GetAll().AnyAsync(x => x.Name == bucketName))
            {
                throw new Abp.UI.UserFriendlyException("存储库已存在。");
            }
            var bucket = new Bucket430() { Name = bucketName, Note = note };
            await _bucketRepository.InsertAndGetIdAsync(bucket);
            try
            {
                Client.CreateBucket(bucket.Name, bucket.StorageClass);
            }
            catch (Exception)
            {
                throw new Abp.UI.UserFriendlyException($"创建存储库失败，存储库名称{bucket.Name}无效。 ", "存储库命名规则：\r\n 只能包含小写字母、数字或短划线（ - ）; \r\n 以小写字母或数字开头和结尾; \r\n 长度必须介于3到63之间 ");
            }
            ApplySettingToBucket(bucket);
            return bucket;
        }
        private void UpdateBucketSetting(Bucket430 bucket)
        {
            Client.SetBucketAcl(bucket.Name, bucket.CannedAccessControl);

            var bucketCorsRequest = bucket.ToCORSRules();
            var bucketRefererRequest = new SetBucketRefererRequest(bucket.Name, bucket.RefererList, bucket.AllowEmptyReferer);

            Client.SetBucketCors(bucketCorsRequest);
            Client.SetBucketReferer(bucketRefererRequest);

        }

        public async Task<List<Bucket430>> GetBucketsAsync()
        {
            return
            await _bucketRepository.GetAll().Where(x => x.Owner == MFConsts.SystemName).ToListAsync();
        }
        public List<Bucket430> GetBuckets()
        {
            return
             _bucketRepository.GetAll().Where(x => x.Owner == MFConsts.SystemName).ToList();
        }

        public async Task DeleteAsync(string bucketName)
        {
            if (SettingManager.GetSettingValue(AppSettingNames.OSS.ContextStore).ToLower() == bucketName.ToLower())
            {
                throw new Abp.UI.UserFriendlyException("已被设置为富文本存储库，不允许删除！");
            }
            //if (_oSSObjectRepository.GetAll().Any(x => x.BucketName == bucketName))
            //{
            //    throw new Abp.UI.UserFriendlyException("存储库不为空，不允许删除！");
            //}
            await _bucketRepository.DeleteAsync(x => x.Name == bucketName);
            await _oSSObjectRepository.DeleteAsync(x => x.BucketName == bucketName);
            Client.DeleteBucket(bucketName);
            // TODO  删除430FS 中的文件
            UnitOfWorkManager.Current.SaveChanges();
        }
        //public async Task<Bucket430> UpdateAsync(UpdateBucket430Dto input)
        //{
        //    var bucket = await _bucketRepository.GetAll().FirstOrDefaultAsync(x => x.Name == input.Name);
        //    if (bucket == null)
        //    {
        //        throw new Abp.UI.UserFriendlyException($"存储库{input.Name}不存在");
        //    }
        //    input.MapTo(bucket);
        //    UnitOfWorkManager.Current.SaveChanges();

        //    UpdateBucketSetting(bucket);

        //    return bucket;

        //}
        public void ApplySettingToAllBucket()
        {
            var buckets = GetBuckets();
            if (buckets == null || buckets.Count < 1)
            {
                return;
            }

            foreach (var bucket in buckets)
            {
                ApplySettingToBucket(bucket);
            }

            //  调用430文件系统设置
            var theOne = buckets.FirstOrDefault();
            var bucketCorsRequest = theOne.ToCORSRules();
            var bucketRefererRequest = new SetBucketRefererRequest(theOne.Name, theOne.RefererList, theOne.AllowEmptyReferer);
            _fs430Manger.Update430Setting(new Setting430(bucketCorsRequest.CORSRules?.FirstOrDefault(), bucketRefererRequest));
        }
        private void ApplySettingToBucket(Bucket430 bucket)
        {
            var allowedOrigins = SettingManager.GetSettingValue(AppSettingNames.OSS.AllowedOrigins);
            var allowedMethods = SettingManager.GetSettingValue(AppSettingNames.OSS.AllowedMethods);
            var allowedHeaders = SettingManager.GetSettingValue(AppSettingNames.OSS.AllowedHeaders);
            var exposedHeaders = SettingManager.GetSettingValue(AppSettingNames.OSS.ExposedHeaders);
            var maxAgeSeconds = SettingManager.GetSettingValue<int>(AppSettingNames.OSS.MaxAgeSeconds);
            var allowEmptyReferer = SettingManager.GetSettingValue<bool>(AppSettingNames.OSS.AllowEmptyReferer);
            var referer = SettingManager.GetSettingValue(AppSettingNames.OSS.Referer);

            bucket._AllowedOrigins = allowedOrigins;
            bucket._AllowedMethods = allowedMethods;
            bucket._AllowedHeaders = allowedHeaders;
            bucket._ExposedHeaders = exposedHeaders;
            bucket.MaxAgeSeconds = maxAgeSeconds;
            bucket.AllowEmptyReferer = allowEmptyReferer;
            bucket._RefererList = referer;

            UpdateBucketSetting(bucket);

        }


    }
}
