using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Auditing.Dto;
using DataExporting.Dto;

namespace MF.Auditing
{
    public interface IAuditLogAppService : IApplicationService
    {
        Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input);
        Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input);
    }
}