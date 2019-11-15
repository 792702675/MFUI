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
using MF.CommonDto;
using MF.PreviousAndNexts;
using MF.JPush;
using System;
using MF.SystemFunctions;
using MF.OSS;
using Microsoft.EntityFrameworkCore;
using MF.SysFuns;

namespace MF.Tags
{
    public class TagAppService : ITagAppService
    {
        private readonly IRepository<ObjectTag> _ObjectTagRepository;
        private readonly IRepository<Tag, int> _repository;
        public TagAppService(
            IRepository<Tag, int> repository,
            IRepository<ObjectTag> ObjectTagRepository
            )
        {
            _repository = repository;
            _ObjectTagRepository = ObjectTagRepository;
        }

        /// <summary>
        /// 获取全部的Tag
        /// </summary>
        public async Task<List<string>> GetAll()
        {
            return await _repository
                .GetAll()
                .Select(x => x.Name)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// 获取全部的系统Tag
        /// </summary>
        public async Task<List<string>> GetAllSystemTag()
        {
            return await _repository
                .GetAll()
                .Where(x=>x.IsSystemTag)
                .Select(x => x.Name)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// 获取全部的非系统Tag
        /// </summary>
        public async Task<List<string>> GetAllNotSystemTag()
        {
            return await _repository
                .GetAll()
                .Where(x => !x.IsSystemTag)
                .Select(x => x.Name)
                .Distinct()
                .ToListAsync();
        }

    }
}
