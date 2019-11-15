using System.Threading.Tasks;
using Abp.Application.Services;
using MF.Sessions.Dto;

namespace MF.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
