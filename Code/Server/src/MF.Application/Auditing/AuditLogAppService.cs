using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using MF.Auditing.Dto;
using MF.Authorization;
using MF.Authorization.Users;
using DataExporting;
using DataExporting.Dto;
using Microsoft.EntityFrameworkCore;

namespace MF.Auditing
{
    [DisableAuditing]
    [AbpAuthorize(PermissionNames.Pages_Administration_AuditLogs)]
    public class AuditLogAppService : MFAppServiceBase, IAuditLogAppService
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IAppFolders _appFolders;

        public AuditLogAppService(
            IRepository<AuditLog, long> auditLogRepository, 
            IRepository<User, long> userRepository, 
            INamespaceStripper namespaceStripper,
            IAppFolders appFolders)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
            _namespaceStripper = namespaceStripper;
            _appFolders = appFolders;
        }
        
        public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input)
        {
            var query = CreateAuditLogAndUsersQuery(input);

            var resultCount = await query.CountAsync();
            var results = await query
                .AsNoTracking()
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(results);

            return new PagedResultDto<AuditLogListDto>(resultCount, auditLogListDtos);
        }

        private List<AuditLogListDto> ConvertToAuditLogListDtos(List<AuditLogAndUser> results)
        {
            return results.Select(
                result =>
                {
                    var auditLogListDto = result.AuditLog.MapTo<AuditLogListDto>();
                    auditLogListDto.UserName = result.User == null ? null : result.User.UserName;
                    //auditLogListDto.ServiceName = _namespaceStripper.StripNameSpace(auditLogListDto.ServiceName);
                    return auditLogListDto;
                }).ToList();
        }

        private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(GetAuditLogsInput input)
        {
            var query = from auditLog in _auditLogRepository.GetAll()
                join user in _userRepository.GetAll() on auditLog.UserId equals user.Id into userJoin
                from joinedUser in userJoin.DefaultIfEmpty()
                where auditLog.ExecutionTime >= input.StartDate && auditLog.ExecutionTime <= input.EndDate
                select new AuditLogAndUser {AuditLog = auditLog, User = joinedUser};

            query = query
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), item => item.User.UserName.Contains(input.UserName))
                .WhereIf(!input.ServiceName.IsNullOrWhiteSpace(), item => item.AuditLog.ServiceName.Contains(input.ServiceName))
                .WhereIf(!input.MethodName.IsNullOrWhiteSpace(), item => item.AuditLog.MethodName.Contains(input.MethodName))
                .WhereIf(!input.BrowserInfo.IsNullOrWhiteSpace(), item => item.AuditLog.BrowserInfo.Contains(input.BrowserInfo))
                .WhereIf(input.MinExecutionDuration.HasValue && input.MinExecutionDuration > 0, item => item.AuditLog.ExecutionDuration >= input.MinExecutionDuration.Value)
                .WhereIf(input.MaxExecutionDuration.HasValue && input.MaxExecutionDuration < int.MaxValue, item => item.AuditLog.ExecutionDuration <= input.MaxExecutionDuration.Value)
                .WhereIf(input.HasException == true, item => item.AuditLog.Exception != null && item.AuditLog.Exception != "")
                .WhereIf(input.HasException == false, item => item.AuditLog.Exception == null || item.AuditLog.Exception == "");
            return query;
        }


        public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
        {
            var auditLogs = await CreateAuditLogAndUsersQuery(input)
                        .AsNoTracking()
                        .OrderByDescending(al => al.AuditLog.ExecutionTime)
                        .ToListAsync();

            var auditLogListDtos = ConvertToAuditLogListDtos(auditLogs);

            FileDto fileinfo= new ExcelExporter().ExportToFile(auditLogListDtos, _appFolders.TempFileDownloadFolder);
            return fileinfo;
        }

    }
}
