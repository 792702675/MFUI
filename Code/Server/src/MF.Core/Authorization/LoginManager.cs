using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using MF.Authorization.Roles;
using MF.Authorization.Users;
using MF.MultiTenancy;
using MF.Configuration;
using System.Threading.Tasks;
using Abp.UI;

namespace MF.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private readonly IRepository<User, long> _userRepository;
        public LogInManager(
            UserManager userManager, 
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager, 
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository, 
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher, 
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory,
            IRepository<User, long> userRepository) 
            : base(
                  userManager, 
                  multiTenancyConfig,
                  tenantRepository, 
                  unitOfWorkManager, 
                  settingManager, 
                  userLoginAttemptRepository, 
                  userManagementConfig, 
                  iocResolver, 
                  passwordHasher, 
                  roleManager, 
                  claimsPrincipalFactory)
        {
            _userRepository = userRepository;
        }


        public async Task CheckLoginSetting(string usernameOrEmailAddress)
        {
            var isPhoneNumberConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AppSettingNames.UserManagement.IsPhoneNumberConfirmationRequiredForLogin);
            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == usernameOrEmailAddress || x.EmailAddress == usernameOrEmailAddress);

            if (user == null)
            {
                return;
            }

            var needPhoneNumberConfirmation = isPhoneNumberConfirmationRequiredForLogin && !user.IsPhoneNumberConfirmed;
            var needEmailConfirmation = isEmailConfirmationRequiredForLogin && !user.IsEmailConfirmed;

            if (needPhoneNumberConfirmation && needEmailConfirmation)
            {
                throw new UserFriendlyException(
                    (int)(LoginSettingVerificationResult.NeedEmailConfirmation | LoginSettingVerificationResult.NeedPhoneNumberConfirmation),
                    "登录失败",
                    "需要激活手机号和邮箱才能登录。");
            }
            if (needPhoneNumberConfirmation)
            {
                throw new UserFriendlyException(
                    (int)(LoginSettingVerificationResult.NeedPhoneNumberConfirmation),
                    "登录失败",
                    "需要激活手机号才能登录。");
            }
            if (needEmailConfirmation)
            {
                throw new UserFriendlyException(
                    (int)(LoginSettingVerificationResult.NeedEmailConfirmation),
                    "登录失败",
                    "需要激活邮箱才能登录。");
            }

        }
    }
}
