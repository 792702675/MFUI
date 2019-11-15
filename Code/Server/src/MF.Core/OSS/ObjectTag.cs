using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.OSS
{
    public class ObjectTag: Entity
    {
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public int OSSObjectId { get; set; }
        public virtual OSSObject OSSObject { get; set; }
    }
}
