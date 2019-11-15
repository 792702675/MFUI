using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF
{
    public interface IIdNameEntity  : IEntity
    {
        string Name { get; set; }
    }
}
