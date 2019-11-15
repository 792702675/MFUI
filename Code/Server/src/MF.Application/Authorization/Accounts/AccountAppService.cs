using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.UI;
using Abp.Zero.Configuration;
using MF.Authorization.Accounts.Dto;
using MF.Authorization.Dto;
using MF.Authorization.Users;
using MF.SMSs;
using MF.Users.Dto;

namespace MF.Authorization.Accounts
{
    public class AccountAppService : MFAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ISMSManager _smsManager;
        private readonly LogInManager _loginManager;
        private readonly UserManager _userManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            ISMSManager smsManager,
            LogInManager loginManager,
            UserManager userManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _smsManager = smsManager;
            _loginManager = loginManager;
            _userManager = userManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }


        /// <inheritdoc />
        public async Task BindingThirdParty(BindingThirdPartyInput input)
        {
            var result = await _loginManager.LoginAsync(input.UserName, input.Password);
            if (result.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("�û����������������");
            }
            if (string.IsNullOrEmpty(input.Token))
            {
                throw new UserFriendlyException("��������֤�������������ʧЧ�������°�");
            }
            await _userRegistrationManager.BindingThirdPartyAsync(input.Token, result.User);
        }


        /// <summary>
        /// ��¼ʱ�����ֻ�֤��
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendPhoneNumberCode(PhoneWithCaptchaInput input)
        {
            //_captchaManager.CheckCaptcha(input.Captcha);
            if ((await _userManager.FindUserByPhoneNumberAsync(input.PhoneNumber)) == null)
            {
                throw new UserFriendlyException("�ֻ�����Ч");
            }
            await _smsManager.SendVerificationCode(input.PhoneNumber);
        }
    }
}
