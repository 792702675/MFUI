using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MF.FS430
{
    /// <summary>
    /// 盐罐子
    /// </summary>
    public class SaltPot : Entity
    {

        [StringLength(1024)]
        public string Key { get; set; }
        public string Salt { get; set; }
    }
}
