using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Zero.Configuration;
using MF.Authorization.Actives.Dto;
using MF.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.UI;
using Newtonsoft.Json;
using MF.Authorization.ThirdParty;
using MF.Configuration;
using MF.Security;
using MF.SMSs;
using MF.Users.Dto;
using MF.Captcha;
using Abp.Runtime.Caching;
using MF.Authorization.Dto;

namespace MF.Authorization.Actives
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class ActiveAppService : MFAppServiceBase, IActiveAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ISMSManager _smsManager;
        private readonly LogInManager _loginManager;
        private readonly PasswordComplexityChecker _passwordComplexityChecker;
        private readonly ICaptchaManager _captchaManager;
        private readonly ICacheManager _cacheManager;
        private readonly UserManager _userManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ActiveAppService(
            UserRegistrationManager userRegistrationManager,
            ISMSManager smsManager,
            LogInManager loginManager,
            PasswordComplexityChecker passwordComplexityChecker,
            ICacheManager cacheManager,
            UserManager userManager,
            ICaptchaManager captchaManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _smsManager = smsManager;
            _loginManager = loginManager;
            _passwordComplexityChecker = passwordComplexityChecker;
            _cacheManager = cacheManager;
            _userManager = userManager;
            _captchaManager = captchaManager;
        }

       

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendConfirmEmailCode(SendConfirmEmailByCaptchaInput input)
        {
            await _userRegistrationManager.SendConfirmEmailCodeAsync(input.Email, input.Captcha);
        }

        /// <summary>
        /// 发送手机激活验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task SendConfirmPhoneNumberByCode(VerificationCodeInput input)
        {
            await _userRegistrationManager.SendConfirmPhoneNumberByCodeAsync(input.PhoneNumber, input.Code);
        }
        public async Task ConfirmEmailByCode(ConfirmEmailCodeInput input)
        {
            await _userRegistrationManager.ConfirmEmailByCodeAsync(input.Email, input.Code);
        }
        public async Task ConfirmPhoneNumberByCode(VerificationCodeInput input)
        {
            await _userRegistrationManager.ConfirmPhoneNumberByCodeAsync(input.PhoneNumber, input.Code);
        }


    }
}