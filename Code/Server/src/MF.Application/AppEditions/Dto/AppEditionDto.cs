using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.AppEditions.Dto
{
    [AutoMap(typeof(IOSAppEdition), typeof(AndroidAppEdition))]
    public class AppEditionDto : UpdateIOSAppEditionInput
    {
        public string CreationTime { get; set; }
        public string AppType { get; set; }
    }
}
