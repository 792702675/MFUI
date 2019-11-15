using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class SearchInput:ISortedResultRequest, IShouldNormalize
    {
        [Required]
        public string BucketName { get; set; }

        /// <summary>
        /// 搜索目录
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// 文件名\文件夹名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] TagNames { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string[] ExtensionNames { get; set; }

        /// <summary>
        /// 排序 Name、ExtensionName、LastModified、Size
        /// </summary>
        public string Sorting { get; set; }

        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "LastModified";
            }

        }
    }
}