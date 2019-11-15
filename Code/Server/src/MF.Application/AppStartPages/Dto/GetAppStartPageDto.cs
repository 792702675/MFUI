using Abp.AutoMapper;
using MF.CommonDto;
using MF.PreviousAndNexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.AppStartPages.Dto
{
    [AutoMap(typeof(AppStartPage))]
    public class GetAppStartPageDto:GetListAppStartPageDto
    {
    }
}
