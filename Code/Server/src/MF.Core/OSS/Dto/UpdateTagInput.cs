using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class UpdateTagInput
    {
        [Required]
        public string BucketName { get; set; }
        [Required]
        public string Key { get; set; }

        public string[] TagNames { get; set; }

        /// <summary>
        /// Key为文件夹时使用
        /// false:只应用于自身
        /// true:应用于自身及全部的子级
        /// </summary>
        public bool ApplyAllChild { get; set; }
    }
}