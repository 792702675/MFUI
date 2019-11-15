using System.Threading.Tasks;
using Abp.Application.Services;
using OSS.Sessions.Dto;

namespace OSS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
