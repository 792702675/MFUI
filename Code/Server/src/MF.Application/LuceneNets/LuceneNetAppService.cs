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
using MF.Demos.Dto;
using MF.CommonDto;
using MF.PreviousAndNexts;
using MF.JPush;
using System;
using MF.LuceneNet;

namespace MF.LuceneNets
{
    public class LuceneNetAppService : IApplicationService
    {
        protected readonly LuceneNetManager _luceneNetManager;
        public LuceneNetAppService(LuceneNetManager luceneNetManager)
        {
            _luceneNetManager = luceneNetManager;
        }

        public void InitIndex()
        {
            _luceneNetManager.InitIndex();
        }

        public object Show(string field, string keyword)
        {
            return _luceneNetManager.ShowFields(new[] { field }, keyword);
        }

        public object ShowFields(string[] field, string keyword)
        {
            return _luceneNetManager.ShowFields(field, keyword);
        }

        public object ShowAdvanced(IEnumerable<MultiFieldInput> input)
        {
            return _luceneNetManager.ShowAdvanced(input);
        }
    }
}
