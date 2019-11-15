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
    public class UpdateUserDto : CreateUserDto, IEntityDto<long>
    {
        public long Id { get; set; }

        public override void CreateValidationRoleTypeList(CustomValidationContext context)
        {
            // 编辑的时候不用检查
        }
    }
}