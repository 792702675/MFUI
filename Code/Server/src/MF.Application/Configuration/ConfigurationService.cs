using MF.Configuration.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Abp.Configuration;
using Abp.Authorization;
using MF.Authorization;
using Abp.Application.Services.Dto;
using Abp.Dependency;

namespace MF.Configuration
{
    public class ConfigurationService<T> : MFAppServiceBase, IConfigurationService<T> where T : ISettingDto
    {
        private IEnumerable<NameValueDto> GetSelectOptions(PropertyInfo propertyInfo)
        {
            var enumSelect = propertyInfo.GetCustomAttributes<EnumSelectAttribute>().FirstOrDefault();
            if (enumSelect == null)
            {
                return null;
            }

            var appService = IocManager.Instance.Resolve(enumSelect.ApplicationService);
            var result = enumSelect.ApplicationService.GetMethod(enumSelect.Method).Invoke(appService, null) as IEnumerable<NameValueDto>;
            if (result == null) {
                throw new Exception($"类型 {enumSelect.ApplicationService.FullName} 的方法 {enumSelect.Method} 必须返回 IEnumerable<NameValueDto> 类型的结果。");
            }
            return result;
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Settings)]
        public virtual SettingsEditOutput GetSetting()
        {
            var list = typeof(T).GetProperties().Select(n => new SettingProperty()
            {
                Name = n.Name,
                DisplayName = n.GetCustomAttributes<DisplayNameAttribute>().FirstOrDefault()?.DisplayName,
                Description = n.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description,
                Type = (n.GetCustomAttributes<TypeAttribute>().FirstOrDefault()?.Value ?? n.PropertyType.Name).ToLower(),
                Value = n.GetCustomAttributes<KeyAttribute>().FirstOrDefault() == null ? null : Convert.ChangeType(SettingManager.GetSettingValue(n.GetCustomAttributes<KeyAttribute>().FirstOrDefault()?.Value), n.PropertyType),
                Title = n.GetCustomAttributes<TitleAttribute>().FirstOrDefault()?.Title,
                SelectOptions = GetSelectOptions(n)
            }).ToList();

            return new SettingsEditOutput()
            {
                Setting = list,
                Name = typeof(T).Name,
                TabName = typeof(T).GetCustomAttributes<TabAttribute>().FirstOrDefault()?.TabName,
            };
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Settings)]
        public virtual async Task SetSetting(T input)
        {
            await Task.FromResult(0);
            typeof(T).GetProperties().ToList().ForEach(n =>
            {
                var name = n.GetCustomAttributes<KeyAttribute>().FirstOrDefault()?.Value;
                if (!string.IsNullOrEmpty(name))
                {
                    var value = n.GetValue(input).ToString();
                    if (n.PropertyType.Name == typeof(bool).Name)
                    {
                        value = value.ToLower();
                    }
                    SettingManager.ChangeSettingForApplication(name, value);
                }
            });
        }
    }
}
