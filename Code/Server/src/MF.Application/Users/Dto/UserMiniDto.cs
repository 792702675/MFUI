using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using MF.Authorization.Users;

namespace MF.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserMiniDto : EntityDto<long>
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public List<RoleType> RoleTypeList { get; set; }

        /// <summary>
        /// [学生、教师] 昵称 可空 （6字以内，允许与其他昵称重复）
        /// </summary> 
        public string Nickname { get; set; }


        /// <summary>
        /// 知识碎片
        /// </summary>
        public int KnowledgePieces { get; set; }

        /// <summary>
        /// 学生主等级
        /// 任意技能等级升级，则主等级升级
        /// 主等级不能通过获得经验直接升级
        /// </summary>
        public int Level { get; set; }
    }
}
