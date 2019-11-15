using Abp.AutoMapper;
using MF.CommonDto;
using MF.PreviousAndNexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Demos.Dto
{
    [AutoMap(typeof(Demo))]
    public class GetDemoDto:GetListDemoDto
    {
        /// <summary>
        /// 上一个 下一个
        /// </summary>
        public PreviousAndNext<Demo> PreviousAndNext { get; set; }
    }
}
