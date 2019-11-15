using Aliyun.OSS;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities.Auditing;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Abp.Extensions;

namespace MF.OSS
{
    /// <summary>
    /// 存储库对象
    /// </summary>
    [AutoMap(typeof(OssObjectSummary))]
    public class OSSObject : FullAuditedEntity
    {
        public OSSObject()
        {
            ObjectTags = new List<ObjectTag>();
        }
        /// <summary>
        /// BucketName
        /// </summary>
        [StringLength(64)]
        public string BucketName { get; set; }

        /// <summary>
        /// 是否是隐藏
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// key
        /// </summary>
        [StringLength(1024)]
        public string Key { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }


        [StringLength(32)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(128)]
        public string ETag { get; set; }

        /// <summary>
        /// 图片 分辨率 宽
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 图片 分辨率 高
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModified { get; set; }


        public virtual ICollection<ObjectTag> ObjectTags { get; set; }


        public void SetNameAndExtensionName()
        {
            Name = Key.TrimEnd('/').Split("/")?.LastOrDefault();
            ExtensionName = Path.GetExtension(Key);
        }




    }

    public class OSSObjectEqualityComparer : IEqualityComparer<OSSObject>
    {
        public bool Equals(OSSObject x, OSSObject y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.ETag.Equals(y.ETag);
        }

        public int GetHashCode(OSSObject obj)
        {
            return obj.ETag.GetHashCode();
        }
    }
}
