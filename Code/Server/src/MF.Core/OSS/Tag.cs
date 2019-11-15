using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MF.OSS
{
    /// <summary>
    /// 标签
    /// </summary>
    public class Tag: Entity
    {
        public Tag()
        {
            ObjectTags = new List<ObjectTag>();
        }

        [StringLength(1024)]
        public string Name { get; set; }
        public bool IsSystemTag { get; set; }

        public virtual ICollection<ObjectTag> ObjectTags { get; set; }
    }
}
