using Abp.Application.Services;

namespace MF.Authorization.ThridParty
{
    public interface IThirdPartyAppService:IApplicationService
    {
        string GetRedirectUrlList();
    }
}