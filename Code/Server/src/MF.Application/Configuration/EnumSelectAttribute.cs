using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Configuration
{
    public class EnumSelectAttribute : TypeAttribute
    {
        public string Method { get; set; }
        public Type ApplicationService { get; set; }
        public EnumSelectAttribute(Type applicationService, string method) : base("EnumSelect")
        {
            ApplicationService = applicationService;
            Method = method;
        }
    }
}
