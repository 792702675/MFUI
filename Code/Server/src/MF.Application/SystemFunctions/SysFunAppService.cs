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
using MF.SysFuns.Dto;
using MF.CommonDto;
using MF.PreviousAndNexts;
using MF.JPush;
using System;
using MF.SystemFunctions;
using MF.OSS;
using Microsoft.EntityFrameworkCore;

namespace MF.SysFuns
{
    public class SysFunAppService : AsyncMFCrudAppService<SysFun, GetListSysFunDto, PagedSortedAndFilteredInputDto, CreateSysFunDto, UpdateSysFunDto>, ISysFunAppService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<SysFunTag> _sysFunTagRepository;
        private readonly TagManage _tagManage;
        public SysFunAppService(
            IRepository<SysFun, int> repository,
            IRepository<Tag> tagRepository,
            IRepository<SysFunTag> sysFunTagRepository,
            TagManage tagManage
            ) : base(repository)
        {
            _tagRepository = tagRepository;
            _sysFunTagRepository = sysFunTagRepository;
            _tagManage = tagManage;
        }

        protected override IQueryable<SysFun> CreateFilteredQuery(PagedSortedAndFilteredInputDto input)
        {
            return base.CreateFilteredQuery(input).Include(x => x.SysFunTags).Include("SysFunTags.Tag");
        }

        public override async Task<GetListSysFunDto> CreateAsync(CreateSysFunDto input)
        {
            var entity = input.MapTo<SysFun>();
            await Repository.InsertAndGetIdAsync(entity);
            await UpdateTag(new UpdateTagInput { Id = entity.Id, TagNames = input.TagNames });
            await UnitOfWorkManager.Current.SaveChangesAsync();

            return entity.MapTo<GetListSysFunDto>();
        }

        /// <summary>
        /// 更新标签
        /// </summary>
        public async Task UpdateTag(UpdateTagInput input)
        {
            var data = await Repository.GetAsync(input.Id);
            await _sysFunTagRepository.DeleteAsync(x => x.SysFunId == data.Id);

            var tags = _tagManage.GetTagId(input.TagNames);
            foreach (var item in tags)
            {
                data.SysFunTags.Add(new SysFunTag { SysFunId = data.Id, TagId = item });
            }
        }
        public override async Task<GetListSysFunDto> UpdateAsync(UpdateSysFunDto input)
        {
            var entity = await Repository.GetAsync(input.Id);
            input.MapTo(entity);
            await UpdateTag(new UpdateTagInput { Id = entity.Id, TagNames = input.TagNames });
            await UnitOfWorkManager.Current.SaveChangesAsync();

            return entity.MapTo<GetListSysFunDto>();
        }
        public async Task<List<NameValueDto<int>>> GetDropDownList()
        {
            return await Repository
                .GetAll()
                .OrderBy(x => x.Name)
                .Select(x => new NameValueDto<int>
                {
                    Name = x.Name,
                    Value = x.Id
                })
                .ToListAsync();
        }

        /// <summary>
        /// 获取指定分组下的TagName
        /// </summary>
        /// <param name="input">分组Id</param>
        public async Task<List<string>> GetTagsByGroupId(EntityDto input)
        {
            var tags = await Repository
                .GetAll()
                .Include(x => x.SysFunTags)
                .ThenInclude(x => x.Tag)
                .OrderBy(x => x.Name)
                .Where(x => x.Id == input.Id)
                .SelectMany(x => x.SysFunTags.Select(s => s.Tag.Name).OrderBy(s => s))
                .ToListAsync();
            return tags;
        }
        /// <summary>
        /// 获取带tag的分组
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetGroupAndTagListDto>> GetGroupAndTagList()
        {
            var tags = await Repository
                .GetAll()
                .Include(x => x.SysFunTags)
                .ThenInclude(x => x.Tag)
                .OrderBy(x => x.Name)
                .Select(x => new GetGroupAndTagListDto
                {
                    Name = x.Name,
                    Value = x.Id,
                    Tags = x.SysFunTags.Select(s => s.Tag.Name).OrderBy(s => s)
                })
                .ToListAsync();
            return tags;
        }

    }
}
