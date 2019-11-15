using Abp.AutoMapper;
using MF.CommonDto;
using MF.PreviousAndNexts;
using MF.SystemFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.SysFuns.Dto
{
    [AutoMap(typeof(SysFun))]
    public class GetSysFunDto:GetListSysFunDto
    {
    }
}
