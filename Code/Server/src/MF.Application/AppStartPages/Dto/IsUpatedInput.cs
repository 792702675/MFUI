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
    public class IsUpatedInput: GetAppStartPageInput
    {

        /// <summary>
        /// 本地图片的更新时间
        /// </summary>
        public virtual DateTime? UpdateTime { get; set; }
    }
}
