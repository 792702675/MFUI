using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace MF.Authorization.Users
{
    public class User : AbpUser<User>
    {

        public const string DefaultPassword = "123qwe";

        public new const int MaxPhoneNumberLength = 24;
        public const int MaxNicknameLength = 6;
        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        [Required(AllowEmptyStrings = true)]
        public override string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = true)]
        public override string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public override string Surname { get; set; }


        /// <summary>
        /// 初始角色类型（不可删除）
        /// </summary>
        public RoleType InitRoleType { get; set; }
        /// <summary>
        /// 角色类型  (不可移除初始角色)(只有一个)
        /// </summary>
        public RoleType RoleType { get; set; }

        /// <summary>
        /// 检查用户的初始角色是否还在 （一般是在编辑用户时使用）
        /// </summary>
        public void CheckInitRoleType()
        {
            if (!RoleType.HasFlag(InitRoleType))
            {
                throw new Abp.UI.UserFriendlyException("用户的初始角色不能移除。");
            }
        }
        /// <summary>
        /// 角色类型  (集合形式)
        /// </summary>
        [NotMapped]
        public List<RoleType> RoleTypeList
        {
            get
            {
                return RoleType.ToList();
            }

            set
            {
                RoleType roleType = 0;
                foreach (var item in value)
                {
                    roleType |= item;
                }
                RoleType = roleType;
            }
        }


        /// <summary>
        /// [学生、教师] 昵称 可空 （6字以内，允许与其他昵称重复）
        /// </summary>        
        public string Nickname { get; set; }



        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public User()
        {
            IsLockoutEnabled = true;
            IsTwoFactorEnabled = true;
            EmailAddress = "";
            Name = "";
            Surname = "";
        }
        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }
        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }
    }
}
