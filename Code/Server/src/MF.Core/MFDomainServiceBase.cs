using Abp.Domain.Services;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF
{
    public class MFDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */
        public IAbpSession AbpSession { get; set; }

        protected MFDomainServiceBase()
        {
            LocalizationSourceName = MFConsts.LocalizationSourceName;
        }
    }
}
