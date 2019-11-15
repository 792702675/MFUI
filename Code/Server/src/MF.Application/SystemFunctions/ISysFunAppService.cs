using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Auditing.Dto;
using DataExporting.Dto;
using MF.SysFuns.Dto;
using System.Collections.Generic;
using MF.CommonDto;

namespace MF.SysFuns
{
    /// <summary>
    /// 标签的分组
    /// </summary>
    public interface ISysFunAppService:IAsyncCrudAppService<GetListSysFunDto, int, PagedSortedAndFilteredInputDto, CreateSysFunDto, UpdateSysFunDto>
    {
        /// <summary>
        /// 更新标签
        /// </summary>
        Task UpdateTag(UpdateTagInput input);

        /// <summary>
        /// 获取分组的下拉列表
        /// </summary>
        /// <returns></returns>
        Task<List<NameValueDto<int>>> GetDropDownList();

        /// <summary>
        /// 获取指定分组下的TagName
        /// </summary>
        /// <param name="input">分组Id</param>
        Task<List<string>> GetTagsByGroupId(EntityDto input);

        /// <summary>
        /// 获取带tag的分组
        /// </summary>
        /// <returns></returns>
        Task<List<GetGroupAndTagListDto>> GetGroupAndTagList();
    }
}
