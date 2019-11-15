
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using MF.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Authorization.Users;
using Abp.Runtime.Validation;

namespace MF.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    /// <summary>
    /// 
    /// </summary>
    public class BatchActiveUserInput 
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long[] Ids { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

    }
}