using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Runtime.Session;
using Newtonsoft.Json.Linq;
using MF.Authorization;
using MF.Configuration.Dto;

namespace MF.Configuration
{
    public class ConfigurationAppService : MFAppServiceBase, IConfigurationAppService
    {
        private readonly ITypeFinder _typeFinder;

        public ConfigurationAppService(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }
        [AbpAuthorize]
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }

        [AbpAuthorize]
        public async Task ChangeSiteUrl(SiteUrlInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.SiteUrl, input.SiteUrl);
        }

        [AbpAuthorize]
        public async Task<SkinOutput> GetUiTheme()
        {
            var skinOutput = new SkinOutput();
            skinOutput.Name = await SettingManager.GetSettingValueForUserAsync(AppSettingNames.UiTheme, AbpSession.ToUserIdentifier());
            return skinOutput;
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Settings)]
        public List<SettingsEditOutput> GetAllSettings()
        {
            var types = _typeFinder
                .Find(type => typeof(ISettingDto).IsAssignableFrom(type) && type.IsClass)
                .Where(type =>
                {
                    if (AbpSession.TenantId == null)
                    {
                        return true;
                    }
                    string requiredFeatureName = (type.GetCustomAttributes(typeof(TabAttribute),true).FirstOrDefault() as TabAttribute) ?.RequiredFeatureName;
                    if (string.IsNullOrEmpty(requiredFeatureName))
                    {
                        return true;
                    }
                    else
                    {
                        return FeatureChecker.IsEnabled(requiredFeatureName);
                    }
                })
                .OrderBy(type => (type.GetCustomAttributes(typeof(TabAttribute), true).FirstOrDefault() as TabAttribute)?.Order).ToList();

            return types.Select(dtoType =>
            {
                var type = typeof(IConfigurationService<>).MakeGenericType(dtoType);
                var configurationAppService = IocManager.Instance.Resolve(type);
                return type.GetMethod("GetSetting").Invoke(configurationAppService, null) as SettingsEditOutput;
            }).ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Settings)]
        public void UpdateAllSettings(JObject input)
        {
            foreach (var item in input)
            {
                var dtoType = _typeFinder
                    .Find(type => typeof(ISettingDto).IsAssignableFrom(type) && type.IsClass)
                    .FirstOrDefault(type => type.Name == item.Key);
                if (dtoType != null)
                {
                    var type = typeof(IConfigurationService<>).MakeGenericType(dtoType);
                    var configurationAppService = IocManager.Instance.Resolve(type);
                    type.GetMethod("SetSetting").Invoke(configurationAppService, new[] { item.Value.ToObject(dtoType) });
                }
            }
        }
        public ClientSettingDto GetClientSetting()
        {
            return new ClientSettingDto()
            {
                AllowSelfRegistration = SettingManager.GetSettingValue<bool>(AppSettingNames.UserManagement.AllowSelfRegistration),
                WeixinOpenIsEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.OAuth.WeixinOpen.IsEnabled),
                AlipayIsEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.OAuth.Alipay.IsEnabled),
                QQIsEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.OAuth.QQ.IsEnabled),
                WeiboIsEnabled = SettingManager.GetSettingValue<bool>(AppSettingNames.OAuth.Weibo.IsEnabled),
                SystemName = SettingManager.GetSettingValue(AppSettingNames.System.SystemName),
            };
        }
    }
}
