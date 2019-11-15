using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Configuration.Dto
{
    public class SettingProperty
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public string Title { get; set; }
        public IEnumerable<NameValueDto> SelectOptions { get; set; }
    }
}
