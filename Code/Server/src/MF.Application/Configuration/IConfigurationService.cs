using System.Collections.Generic;
using MF.Configuration.Dto;
using Abp.Application.Services;
using Abp.Dependency;
using System.Threading.Tasks;

namespace MF.Configuration
{
    public interface IConfigurationService<T> where T : ISettingDto
    {
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <returns></returns>
        SettingsEditOutput GetSetting();
        /// <summary>
        /// 应用设置
        /// </summary>
        /// <param name="input"></param>
        Task SetSetting(T input);
    }
}