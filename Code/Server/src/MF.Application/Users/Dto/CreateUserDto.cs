using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using MF.Authorization.Users;

namespace MF.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize, ICustomValidate
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }


        //[Required]
        //[EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }

        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(User.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 是否发送激活消息
        /// </summary>
        public bool SendActivationMessage { get; set; }

        /// <summary>
        /// 是否发送激活邮件
        /// </summary>
        public bool SendActivationEmail { get; set; }

        /// <summary>
        /// 是否设置随机密码
        /// </summary>
        public bool SetRandomPassword { get; set; }

        /// <summary>
        /// 组织机构
        /// </summary>
        public int[] Organizations { get; set; }

        /// <summary>
        /// 下次登录需要修改密码
        /// </summary>
        public bool ShouldChangePasswordOnNextLogin { get; set; }

        /// <summary>
        /// 角色类型  (不可移除初始角色)(创建时只能选一个)
        /// </summary>
        //[Required]
        public List<RoleType> RoleTypeList { get; set; }

        /// <summary>
        /// [学生、教师] 昵称 可空 （6字以内，允许与其他昵称重复）
        /// </summary> 
        [StringLength(User.MaxNicknameLength)]
        public string Nickname { get; set; }




        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
            EmailAddress = EmailAddress ?? "";
            Name = Name ?? "";
        }

        public virtual void CreateValidationRoleTypeList(CustomValidationContext context)
        {
            //if (RoleTypeList == null || RoleTypeList.Count != 1)
            //{
            //    context.Results.Add(new ValidationResult("类型角色不能空，并且初始类型角色只能有一个！", new string[] { "RoleTypeList" }));
            //}
        }

        public virtual void AddValidationErrors(CustomValidationContext context)
        {
            //if (string.IsNullOrWhiteSpace(EmailAddress) && string.IsNullOrWhiteSpace(PhoneNumber))
            //{
            //    context.Results.Add(new ValidationResult("手机号和邮箱不能都为空！"));
            //}


            CreateValidationRoleTypeList(context);
        }
    }
}
