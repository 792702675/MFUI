using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using OSS.Configuration.Dto;

namespace OSS.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : OSSAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
