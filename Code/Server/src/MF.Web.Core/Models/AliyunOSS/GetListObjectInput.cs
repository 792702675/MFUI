using Aliyun.OSS;
using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class GetListObjectInput
    {
        [Required]
        public string BucketName { get; set; }
        /// <summary>
        /// 本次查询结果的前缀。
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 标明本次列举文件的起点。
        /// </summary>
        public string Marker { get; set; }
        /// <summary>
        /// 列举文件的最大个数。默认为100，最大值为1000。
        /// </summary>
        public int? MaxKeys { get; set; }
        /// <summary>
        /// 对文件名称进行分组的一个字符。CommonPrefixes是以delimiter结尾，并有共同前缀的文件集合。
        /// </summary>
        public string Delimiter { get; set; }
        public string EncodingType { get; set; }

        public ListObjectsRequest ToRequest()
        {
            return new ListObjectsRequest(BucketName)
            {
                Prefix = Prefix,
                Delimiter = Delimiter,
                EncodingType = EncodingType,
                Marker = Marker,
                MaxKeys = MaxKeys
            };
        }
    }
}