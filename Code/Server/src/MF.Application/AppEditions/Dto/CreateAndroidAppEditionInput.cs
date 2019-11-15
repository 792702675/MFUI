using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.AppEditions.Dto
{
    [AutoMap(typeof(AndroidAppEdition))]
    public class CreateAndroidAppEditionInput : CreateAppEditionInput
    {
    }
}
