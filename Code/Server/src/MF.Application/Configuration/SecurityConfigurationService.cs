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

namespace MF.Configuration
{
    public class SecurityConfigurationService : ConfigurationService<SecuritySettingDto>, IConfigurationService<SecuritySettingDto>
    {

        public override SettingsEditOutput GetSetting()
        {
            var result = base.GetSetting();

            var passwordComplexity = result.Setting.Find(n => n.Name == "PasswordComplexity");
            passwordComplexity.Value = SettingManager.GetSettingValueForApplication(AppSettingNames.Security.PasswordComplexity);

            if (passwordComplexity.Value.ToString() == PasswordComplexitySetting.DefaultPasswordComplexitySetting.ToJsonString())
            {
                var useDefaultPasswordComplexity = result.Setting.Find(n => n.Name == "UseDefaultPasswordComplexity");
                useDefaultPasswordComplexity.Value = true;
            }

            return result;
        }


        public override async Task SetSetting(SecuritySettingDto input)
        {
            await base.SetSetting(input);
            SettingManager.ChangeSettingForApplication(AppSettingNames.Security.PasswordComplexity, JsonConvert.SerializeObject(input.PasswordComplexity));
        }
    }
}
