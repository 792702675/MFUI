using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MF.OSS;
using MF.SystemFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.SysFuns.Dto
{
    [AutoMap(typeof(SysFun))]
    public class CreateSysFunDto
    {
        public string Name { get; set; }
        public virtual string[] TagNames { get; set; }
    }
}
