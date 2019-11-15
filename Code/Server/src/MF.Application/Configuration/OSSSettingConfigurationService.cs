using Abp.Configuration;
using Abp.Json;
using Abp.Zero.Configuration;
using Newtonsoft.Json;
using MF.Configuration;
using MF.Configuration.Dto;
using MF.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MF.Buckets;
using MF.OSS;

namespace MF.Configuration
{
    public class OSSSettingConfigurationService : ConfigurationService<OSSSettingDto>, IConfigurationService<OSSSettingDto>
    {
        public BucketManage BucketManage { get; set; }
        public OSSManage OSSManage { get; set; }


        public override async Task SetSetting(OSSSettingDto input)
        {
            var changed = typeof(OSSSettingDto)
                .GetProperties()
                .Where(x => x.GetCustomAttributes<KeyAttribute>().FirstOrDefault() != null)
                .Any(x => Convert.ChangeType(SettingManager.GetSettingValue(x.GetCustomAttributes<KeyAttribute>().FirstOrDefault()?.Value), x.PropertyType) != x.GetValue(input));

            if (!changed)
            {
                return;
            }
            await base.SetSetting(input);

            BucketManage.ApplySettingToAllBucket();
            await OSSManage.CreateFolder(input.ContextStore, "ContextStore/", null, true);
        }
    }
}
