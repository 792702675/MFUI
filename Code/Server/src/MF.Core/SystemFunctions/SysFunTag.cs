using Abp.Domain.Entities;
using MF.OSS;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.SystemFunctions
{
    public class SysFunTag: Entity
    {
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public int SysFunId { get; set; }
        public virtual SysFun SysFun { get; set; }
    }
}
