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
using DataExporting;
using DataExporting.Dto;
using Abp.Application.Services;
using MF.AppStartPages.Dto;
using MF.CommonDto;
using MF.PreviousAndNexts;
using System;

namespace MF.AppStartPages
{
    public class AppStartPageAppService : AsyncMFCrudAppService<AppStartPage, GetListAppStartPageDto, PagedSortedAndFilteredInputDto, CreateAppStartPageDto, UpdateAppStartPageDto>//, IAppStartPageAppService
    {

        public AppStartPageAppService(IRepository<AppStartPage, int> repository) : base(repository)
        {
            DeletePermissionName = PermissionNames.Pages_Administration_AppStartPage;
            CreatePermissionName = PermissionNames.Pages_Administration_AppStartPage;
            UpdatePermissionName = PermissionNames.Pages_Administration_AppStartPage;
        }
        private async Task<AppStartPage> Get(GetAppStartPageInput input)
        {
            Platform? platform = input.Platform;
            int? width_Px = input.Width_Px;
            int? high_Px = input.High_Px;
            var data =
                await Repository.FirstOrDefaultAsync(x => x.Platform == platform && x.Width_Px == width_Px && x.High_Px == high_Px)
                ??
                await Repository.FirstOrDefaultAsync(x => x.Platform == null && x.Width_Px == width_Px && x.High_Px == high_Px)
                ??
                await Repository.FirstOrDefaultAsync(x => x.Platform == platform && x.Width_Px == width_Px && x.High_Px == null)
                ??
                await Repository.FirstOrDefaultAsync(x => x.Platform == platform && x.Width_Px == null && x.High_Px == high_Px)
                ??
                await Repository.FirstOrDefaultAsync(x => x.Platform == platform && x.Width_Px == null && x.High_Px == null)
                ??
                await Repository.FirstOrDefaultAsync(x => x.Platform == null && x.Width_Px == null && x.High_Px == null)
                ;
            return data;
        }
        /// <summary>
        /// App端： 获取图片
        /// </summary>
        public async Task<GetAppStartPageDto> GetAppStartPage(GetAppStartPageInput input)
        {
            var data = await Get(input);
            return data?.MapTo<GetAppStartPageDto>();
        }

        /// <summary>
        /// App端： 检查图片是否有更新
        /// </summary>
        public async Task<bool> IsUpdated(IsUpatedInput input)
        {
            var data = await Get(input);
            if (data == null) { return false; }
            return data.IsUpdated(input.UpdateTime);
        }

    }
}
