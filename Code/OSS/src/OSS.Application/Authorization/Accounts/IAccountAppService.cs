using System.Threading.Tasks;
using Abp.Application.Services;
using OSS.Authorization.Accounts.Dto;

namespace OSS.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
