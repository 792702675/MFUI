using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MF.CommonDto;
using MF.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.OSSObjects.Dto
{
    public class GetAllInput: PagedAndSortedInputDto
    {
        /// <summary>
        /// 库名
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标签名
        /// </summary>
        public List< string> TagNames { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string[] ExtensionNames { get; set; }
        /// <summary>
        /// 系统功能
        /// </summary>
        public string SysFunName { get; set; }

        /// <summary>
        /// 本文相关 （与BucketName条件不兼容，Group优先）
        /// </summary>
        public Guid? Group { get; set; }
    }
}
