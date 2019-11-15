using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MF.Configuration.Dto;

namespace MF.Configuration
{
    public interface IConfigurationAppService
    {
        /// <summary>
        /// 设置皮肤
        /// </summary>
        Task ChangeUiTheme(ChangeUiThemeInput input);
        /// <summary>
        /// 获取用户设置的皮肤
        /// </summary>
        /// <returns></returns>
        Task<SkinOutput> GetUiTheme();
        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        List<SettingsEditOutput> GetAllSettings();
        /// <summary>
        /// 更新所有配置
        /// </summary>
        /// <param name="input"></param>
        void UpdateAllSettings(JObject input);
        /// <summary>
        /// 获取客户端设置
        /// </summary>
        /// <returns></returns>
        ClientSettingDto GetClientSetting();
    }
}
