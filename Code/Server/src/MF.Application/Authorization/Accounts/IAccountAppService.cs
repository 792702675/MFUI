using System.Threading.Tasks;
using Abp.Application.Services;
using MF.Authorization.Accounts.Dto;
using MF.Authorization.Dto;
using MF.Users.Dto;

namespace MF.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        /// <summary>
        /// 绑定账号
        /// （无调用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task BindingThirdParty(BindingThirdPartyInput input);

        /// <summary>
        /// 登录时发送手机证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task SendPhoneNumberCode(PhoneWithCaptchaInput input);
    }
}
