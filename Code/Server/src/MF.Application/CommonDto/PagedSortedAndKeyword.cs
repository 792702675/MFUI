using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MF.CommonDto;
using MF.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.CommonDto
{
    public class PagedSortedAndKeyword : PagedSortedAndFilteredInputDto
    {

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string  Keyword { get; set; }
    }
}
