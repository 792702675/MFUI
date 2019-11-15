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
    [AutoMap(typeof(SysFun))]
    public class UpdateSysFunDto:CreateSysFunDto, IEntityDto<int>
    {
        public int Id { get; set; }
    }
}
