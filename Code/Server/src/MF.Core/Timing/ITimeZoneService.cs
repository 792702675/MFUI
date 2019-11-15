using System.Threading.Tasks;
using Abp.Configuration;

namespace MF.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);
    }
}
