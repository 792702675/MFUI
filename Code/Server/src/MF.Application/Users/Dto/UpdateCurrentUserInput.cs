using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using MF.Authorization.Users;
using Abp.Runtime.Validation;
using Abp.Extensions;

namespace MF.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class UpdateCurrentUserInput : IShouldNormalize
    {
        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [StringLength(User.MaxNameLength)]
        public string Name { get; set; }


        /// <summary>
        /// ����    ���ɲ�ʹ�á�
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// �ֻ���
        /// </summary>
        [StringLength(User.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        public void Normalize()
        {
            if (Surname.IsNullOrWhiteSpace())
            {
                Surname = Name;
            }
        }
    }
}