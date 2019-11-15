using System.Threading.Tasks;
using OSS.Configuration.Dto;

namespace OSS.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
