using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using MF.Authorization;
using MF.Authorization.Users;
using Abp.Application.Services;
using MF.OSSObjects.Dto;
using MF.CommonDto;
using MF.PreviousAndNexts;
using MF.JPush;
using System;
using MF.SystemFunctions;
using MF.OSS;
using Microsoft.EntityFrameworkCore;
using Aliyun.OSS;
using Abp.Domain.Uow;
using MF.Buckets;
using Abp.Configuration;
using MF.Configuration;

namespace MF.OSSObjects
{
    public class OSSObjectAppService : IOSSObjectAppService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ObjectTag> _ObjectTagRepository;
        private readonly TagManage _tagManage;
        private readonly IRepository<OSSObject> _oSSObjectRepository;
        private readonly IRepository<SysFunTag> _sysFunTagRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly OSSManage _aliyunOSSManage;
        private readonly BucketManage _bucketManage;
        private readonly ISettingManager _settingManager;
        public OSSObjectAppService(
            IRepository<OSSObject, int> repository,
            IRepository<Tag> tagRepository,
            IRepository<ObjectTag> ObjectTagRepository,
            IRepository<OSSObject> oSSObjectRepository,
            IRepository<SysFunTag> sysFunTagRepository,
            TagManage tagManage,
            IUnitOfWorkManager unitOfWorkManager,
            OSSManage aliyunOSSManage,
            BucketManage bucketManage,
            ISettingManager settingManager
            )
        {
            _tagRepository = tagRepository;
            _ObjectTagRepository = ObjectTagRepository;
            _sysFunTagRepository = sysFunTagRepository;
            _tagManage = tagManage;
            _oSSObjectRepository = oSSObjectRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _aliyunOSSManage = aliyunOSSManage;
            _bucketManage = bucketManage;
            _settingManager = settingManager;
        }
        public async Task<PagedResultDto<OSSObjectDto>> GetAll(GetAllInput input)
        {
            var funTagNames = new List<string>();
            if (!input.SysFunName.IsNullOrEmpty())
            {
                var tags = await _sysFunTagRepository
                    .GetAll()
                    .Include(x => x.SysFun)
                    .Include(x => x.Tag)
                    .Where(x => x.SysFun.Name == input.SysFunName)
                    .Select(x => x.Tag.Name)
                    .ToListAsync();
                funTagNames.AddRange(tags);
            }

            if (input.Group.HasValue)
            {
                input.BucketName = _settingManager.GetSettingValue(AppSettingNames.OSS.ContextStore);
            }


            var thisBuckets = _bucketManage.GetBuckets().Select(x => x.Name).Distinct().ToList();

            var hiddenObjectAndSubs = await _aliyunOSSManage.GetHiddenObjects(thisBuckets).ToListAsync();

            IQueryable<OSSObject> q =
                _oSSObjectRepository
                .GetAll()
                .Include(x => x.ObjectTags)
                .Include("ObjectTags.Tag")
                .Where(x => x.Size > 0)
                .Where(x => thisBuckets.Contains(x.BucketName))
                .WhereIf(!input.BucketName.IsNullOrEmpty(), x => x.BucketName == input.BucketName)
                .WhereIf(!input.Name.IsNullOrEmpty(), x => x.Name.Contains(input.Name))
                .WhereIf(input.Group.HasValue, x => x.Key.Contains("" + input.Group))
                .WhereIf(input.TagNames != null && input.TagNames.Count > 0, x => x.ObjectTags.Select(o => o.Tag.Name.ToLower()).Intersect(input.TagNames.Select(t => t.ToLower())).Any())
                .WhereIf(input.ExtensionNames != null && input.ExtensionNames.Length > 0, x => input.ExtensionNames.Select(t => t.ToLower()).Any(e => x.ExtensionName.ToLower().Contains(e)))
                .WhereIf(!input.SysFunName.IsNullOrEmpty(), x => x.ObjectTags.Select(o => o.Tag.Name).Intersect(funTagNames).Any())
                .WhereIf(!input.Group.HasValue, x => !hiddenObjectAndSubs.Contains(x.Id)) // 不显示隐藏文件
                .GroupBy(x => x.ETag)
                .Select(x => x.FirstOrDefault())
                .OrderBy(x => x.Id)
                ;
            var count = await q.CountAsync();
            q = q.PageBy(input.SkipCount, input.MaxResultCount);

            var data = await q.ToListAsync();
            var dataR = data
                .Select(x => _aliyunOSSManage.OssObjectToOSSObjectDto(x))
                .ToList();

            _unitOfWorkManager.Current.SaveChanges();
            return new PagedResultDto<OSSObjectDto>(count, dataR);
        }

        /// <summary>
        /// 用作下拉列表
        /// </summary>
        public IEnumerable<NameValueDto> GetBucketList()
        {
            return _bucketManage.GetBuckets().Select(x => x.Name).ToNameValueDto();
        }


    }
}
