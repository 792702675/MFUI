using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Auditing.Dto;
using DataExporting.Dto;
using MF.OSSObjects.Dto;
using System.Collections.Generic;
using MF.CommonDto;
using MF.OSS;

namespace MF.OSSObjects
{
    /// <summary>
    /// OSSObject
    /// </summary>
    public interface IOSSObjectAppService:IApplicationService
    {
        /// <summary>
        /// 文件查询
        /// </summary>
        Task<PagedResultDto<OSSObjectDto>> GetAll(GetAllInput input);
        /// <summary>
        /// BucketList 用作下拉列表
        /// </summary>
        IEnumerable<NameValueDto> GetBucketList();
    }
}
