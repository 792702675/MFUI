using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using MF.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Authorization.Users;
using System.Linq;
using Abp.Collections.Extensions;
using DataExporting.DataImportExporter;

namespace MF.Users.Dto
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [ExportInfo("用户信息")]
    [AutoMapFrom(typeof(User))]
    public class UserListDto : EntityDto<long>, IPassivable
    {
        [ExportInfo("编号")]
        public new long Id { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        [ExportInfo("姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        //[ExportInfo("昵称")]
        public string Surname { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [ExportInfo("用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [ExportInfo("邮件地址")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [ExportInfo("手机号码")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public Guid? ProfilePictureId { get; set; }

        /// <summary>
        /// 邮箱地址是否已验证
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// 邮箱地址是否已验证
        /// </summary>
        [ExportInfo("邮箱地址是否已验证")]
        private string _IsEmailConfirmed => IsEmailConfirmed ? "是" : "否";

        /// <summary>
        /// 手机号码是否已验证
        /// </summary>
        public bool IsPhoneNumberConfirmed { get; set; }

        /// <summary>
        /// 手机号码是否已验证
        /// </summary>
        [ExportInfo("手机号码是否已验证")]
        private string _IsPhoneNumberConfirmed => IsPhoneNumberConfirmed ? "是" : "否";

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<UserListRoleDto> Roles { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        [ExportInfo("角色列表")]
        private string _Roles => Roles.Select(r => r.RoleName).JoinAsString(", ");

        /// <summary>
        /// 上次登录时间
        /// </summary>
        [ExportInfo("上次登录时间")]
        public string LastLoginTime { get; set; }

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ExportInfo("是否启用")]
        private string _IsActive => IsActive ? "是" : "否";

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExportInfo("创建时间")]
        public string CreationTime { get; set; }

        /// <summary>
        /// 角色信息
        /// </summary>
        [AutoMapFrom(typeof(UserRole))]
        public class UserListRoleDto
        {
            public int RoleId { get; set; }

            public string RoleName { get; set; }
        }

        /// <summary>
        /// 是否被锁定
        /// </summary>
        public bool IsLocked => LockoutEndDateUtc.HasValue && LockoutEndDateUtc.Value > DateTime.Now.ToUniversalTime();

        /// <summary>
        /// 锁定超时时间
        /// </summary>
        public DateTime? LockoutEndDateUtc { get; set; }


        /// <summary>
        /// 角色类型  (集合形式)
        /// </summary>

        public List<RoleType> RoleTypeList { get; set; }
        /// <summary>
        /// 初始角色类型（不可删除）
        /// </summary>
        public RoleType InitRoleType { get; set; }

        /// <summary>
        /// [学生、教师] 昵称 可空 （6字以内，允许与其他昵称重复）
        /// </summary> 
        public string Nickname { get; set; }


        /// <summary>
        /// 家长的学生
        /// </summary>
        public long[] StudentIds { get; set; }

    }
    
}