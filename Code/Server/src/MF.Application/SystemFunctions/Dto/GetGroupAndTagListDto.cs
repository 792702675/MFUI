using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MF.SystemFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.SysFuns.Dto
{
    public class GetGroupAndTagListDto 
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public IEnumerable< string> Tags { get; set; }
    }
}
