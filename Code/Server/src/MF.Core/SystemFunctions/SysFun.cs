using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.SystemFunctions
{
    /// <summary>
    /// Tag分组
    /// </summary>
    public class SysFun: Entity
    {
        public SysFun()
        {
            SysFunTags = new List<SysFunTag>();
        }

        public string Name { get; set; }
        public virtual ICollection<SysFunTag> SysFunTags { get; set; }
    }
}
