using System.Collections.Generic;
using Abp.Application.Services.Dto;
using MF.Authorization.Permissions.Dto;

namespace MF.Users.Dto
{
    /// <summary>
    /// 用户权值
    /// </summary>
    public class GetUserPermissionsForEditOutput
    {
        /// <summary>
        /// 权值
        /// </summary>
        public List<PermissionViewDto> Permissions { get; set; }
        /// <summary>
        /// 已授予的权值
        /// </summary>
        public List<string> GrantedPermissionNames { get; set; }
    }
}