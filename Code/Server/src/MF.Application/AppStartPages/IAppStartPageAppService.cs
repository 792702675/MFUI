using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Auditing.Dto;
using DataExporting.Dto;
using MF.AppStartPages.Dto;
using System.Collections.Generic;
using MF.CommonDto;
using System;

namespace MF.AppStartPages
{
    /// <summary>
    /// AppStartPage
    /// </summary>
    public interface IAppStartPageAppService:IAsyncCrudAppService<GetListAppStartPageDto, int, PagedSortedAndFilteredInputDto, CreateAppStartPageDto, UpdateAppStartPageDto>
    {
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteBatch(ArrayDto<int> input);

        /// <summary>
        /// App端： 获取图片
        /// </summary>
        Task<GetAppStartPageDto> GetAppStartPage(GetAppStartPageInput input);

        /// <summary>
        /// App端： 检查图片是否有更新
        /// </summary>
        Task<bool> IsUpdated(IsUpatedInput input);
    }
}
