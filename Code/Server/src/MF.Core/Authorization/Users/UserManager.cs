using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Caching;
using MF.Authorization.Roles;
using System.Threading.Tasks;
using Abp.Localization;
using Abp.Zero;
using Abp.UI;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp;
using Abp.Threading;
using System.Globalization;
using Abp.Events.Bus.Handlers;
using Abp.Events.Bus;

namespace MF.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public UserManager(
            RoleManager roleManager,
            UserStore store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<User> passwordHasher, 
            IEnumerable<IUserValidator<User>> userValidators, 
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<User>> logger, 
            IPermissionManager permissionManager, 
            IUnitOfWorkManager unitOfWorkManager, 
            ICacheManager cacheManager, 
            IRepository<OrganizationUnit, long> organizationUnitRepository, 
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
            IOrganizationUnitSettings organizationUnitSettings,
            ISettingManager settingManager)
            : base(
                roleManager, 
                store, 
                optionsAccessor, 
                passwordHasher, 
                userValidators, 
                passwordValidators, 
                keyNormalizer, 
                errors, 
                services, 
                logger, 
                permissionManager, 
                unitOfWorkManager, 
                cacheManager,
                organizationUnitRepository, 
                userOrganizationUnitRepository, 
                organizationUnitSettings, 
                settingManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }



        public override async Task<IdentityResult> CheckDuplicateUsernameOrEmailAddressAsync(long? expectedUserId, string userName, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return IdentityResult.Success;
            }
            return await base.CheckDuplicateUsernameOrEmailAddressAsync(expectedUserId, userName, emailAddress);
        }

      
        private new  string  L(string name)
        {
            return LocalizationManager.GetString(AbpZeroConsts.LocalizationSourceName, name);
        }

        public async Task<User> FindUserByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || !phoneNumber.IsPhoneNumber())
            {
                throw new UserFriendlyException("手机号输入有误");
            }
            var result = await Users.Where(u => u.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return result;
        }


        public async Task<User> GetUserOrNullAsync(UserIdentifier userIdentifier)
        {
            using (_unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
                {
                    return await FindByIdAsync(""+userIdentifier.UserId);
                }
            }
        }

        public User GetUserOrNull(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
        }

        public async Task<User> GetUserAsync(UserIdentifier userIdentifier)
        {
            var user = await GetUserOrNullAsync(userIdentifier);
            if (user == null)
            {
                throw new ApplicationException("There is no user: " + userIdentifier);
            }

            return user;
        }

        public User GetUser(UserIdentifier userIdentifier)
        {
            return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
        }

        public override async Task<IdentityResult> UpdateAsync(User user)
        {
            //if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && await Users.AnyAsync(n => n.Id != user.Id && n.PhoneNumber == user.PhoneNumber))
            //{
            //    return IdentityResult.Failed(new IdentityError() {  Description= $"手机号 '{user.PhoneNumber}' 已被占用" });
            //}
            return await base.UpdateAsync(user);
        }

    }
}
