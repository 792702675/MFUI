using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Auditing.Dto;
using DataExporting.Dto;
using System.Collections.Generic;
using MF.CommonDto;
using MF.SysFuns;

namespace MF.Tags
{
    /// <summary>
    /// Tag
    /// </summary>
    public interface ITagAppService:IApplicationService
    {
        /// <summary>
        /// 获取全部的Tag
        /// </summary>
        Task<List<string>> GetAll();

        /// <summary>
        /// 获取全部的系统Tag
        /// </summary>
        Task<List<string>> GetAllSystemTag();

        /// <summary>
        /// 获取全部的非系统Tag
        /// </summary>
        Task<List<string>> GetAllNotSystemTag();
    }
}
