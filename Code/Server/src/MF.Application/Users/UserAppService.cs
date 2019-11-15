using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using MF.Authorization;
using MF.Authorization.Accounts;
using MF.Authorization.Roles;
using MF.Authorization.Users;
using MF.Roles.Dto;
using MF.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Notifications;
using MF.Notifications;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using MF.OrganizationUnits;
using MF.Authorization.Permissions;
using Abp.Net.Mail.Smtp;
using MF.SMSs;
using Abp.AutoMapper;
using System;
using Abp.Zero.Configuration;
using MF.OrganizationUnits.Dto;
using DataExporting.Dto;
using DataExporting;

namespace MF.Users
{
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UpdateUserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;


        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IOrganizationUnitAppService _organizationUnitApp;
        private readonly IPermissionAppService _permissionAppService;
        private readonly ISmtpEmailSender _emailSender;
        private readonly ISMSSenderManager _smsSender;
        private readonly IAppFolders _appFolders;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager,

            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IOrganizationUnitAppService organizationUnitApp,
            IPermissionAppService permissionAppService,
            ISmtpEmailSender emailSender,
            IAppFolders appFolders, ISMSSenderManager smsSender)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;

            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _organizationUnitApp = organizationUnitApp;
            _permissionAppService = permissionAppService;
            _emailSender = emailSender;
            _appFolders = appFolders;
            _smsSender = smsSender;
        }


        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Lookup, PermissionNames.Pages_Administration_Users)]
        public async Task<ListResultDto<RoleListDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Lookup, PermissionNames.Pages_Administration_Users)]
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            IQueryable<User> query = QueryUser(input);

            var userCount = await query.CountAsync();
            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userListDtos = users.MapTo<List<UserListDto>>();
            await FillRoleNames(userListDtos);


            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }
        private IQueryable<User> QueryUser(GetUsersInput input)
        {
            var query = _userManager.Users
                .Include(u => u.Roles)
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        //u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter) ||
                        u.PhoneNumber.Contains(input.Filter)
                )
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), u => u.Name.Contains(input.Name))
                .WhereIf(!input.UserName.IsNullOrWhiteSpace(), u => u.UserName.Contains(input.UserName))
                .WhereIf(!input.PhoneNumber.IsNullOrWhiteSpace(), u => u.PhoneNumber.Contains(input.PhoneNumber));

            if (!input.Permission.IsNullOrWhiteSpace())
            {
                query = (from user in query
                         join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                         from ur in urJoined.DefaultIfEmpty()
                         join up in _userPermissionRepository.GetAll() on new { UserId = user.Id, Name = input.Permission } equals new { up.UserId, up.Name } into upJoined
                         from up in upJoined.DefaultIfEmpty()
                         join rp in _rolePermissionRepository.GetAll() on new { RoleId = ur.RoleId, Name = input.Permission } equals new { rp.RoleId, rp.Name } into rpJoined
                         from rp in rpJoined.DefaultIfEmpty()
                         where (up != null && up.IsGranted) || (up == null && rp != null)
                         group user by user into userGrouped
                         select userGrouped.Key);
            }

            return query;
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users)]
        public async Task<FileDto> GetUsersToExcel(GetUsersInput input)
        {
            IQueryable<User> query = QueryUser(input);
            var users = await query.ToListAsync();
            var userListDtos = users.MapTo<List<UserListDto>>();

            await FillRoleNames(userListDtos);
            return new ExcelExporter().ExportToFile(userListDtos, _appFolders.TempFileDownloadFolder);
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = (await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync());

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos
            };

            if (!input.Id.HasValue)
            {
                bool.TryParse(await SettingManager.GetSettingValueAsync(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled), out bool isEnabled);
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    //IsTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled = isEnabled
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == input.Id.Value);

                output.User = user.MapTo<UserEditDto>();
                output.ProfilePictureId = user.ProfilePictureId;
                output.OrganizationIds = await _organizationUnitApp.GetUserOrganizationUnits(new UserIdInput { UserId = input.Id ?? 0 });
                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await _userManager.IsInRoleAsync(user, userRoleDto.RoleName);
                }
            }
            return output;
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            var permissions = _permissionAppService.GetAllPermissionTree();
            var grantedPermissions = await _userManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = permissions,
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await _userManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }


        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);
            if (input.RoleTypeList != null && input.RoleTypeList.Count > 0)
            {
                user.InitRoleType = input.RoleTypeList[0];  //初始角色类型
            }

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
            }

            CurrentUnitOfWork.SaveChanges();


            await SetOrganization(user, input.Organizations);
            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);
            try
            {
                if (input.SendActivationEmail && !string.IsNullOrWhiteSpace(input.EmailAddress))
                {
                    var body = $"您好，系统已为您创建账号，用户名:{input.UserName}, 验证码:{input.Password}，感谢您使用本系统";
                    if (!input.IsActive)
                    {
                        body = $"您好，系统已为您创建账号，用户名:{input.UserName}, 验证码:{input.Password}，" +
                               "首次登陆需要激活帐号，感谢您使用本系统。";
                    }
                    var subject = "账号创建通知";
                    await _emailSender.SendAsync(input.EmailAddress, subject, body);
                }
                if (input.SendActivationMessage && !string.IsNullOrWhiteSpace(input.PhoneNumber))
                {
                    var body = $"您好，系统已为您创建账号，用户名:{input.UserName}, 密码:{input.Password}，感谢您使用本系统";
                    if (!input.IsActive)
                    {
                        body = $"您好，系统已为您创建账号，用户名:{input.UserName}, 密码:{input.Password}，首次登陆需要激活帐号，感谢您使用本系统。";
                    }
                    await _smsSender.Sender(input.PhoneNumber, body);
                }
            }
            catch (Exception)
            {
                //ignore
            }
            return MapToEntityDto(user);
        }

        private async Task SetOrganization(User user, int[] organizations)
        {
            if (organizations == null)
            {
                return;
            }
            await _organizationUnitApp.RemoveAllOrganizationUnit(user.Id);
            UnitOfWorkManager.Current.SaveChanges();
            foreach (var org in organizations)
            {
                await _organizationUnitApp.AddUserToOrganizationUnit(
                    new OrganizationUnits.Dto.UsersToOrganizationUnitInput() { OrganizationUnitId = org, UserIdListStr = user.Id.ToString() });
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Edit)]
        public override async Task<UserDto> UpdateAsync(UpdateUserDto input)
        {
            var user = await Repository.GetAll().Where(x => x.Id == input.Id).FirstAsync();

            MapToEntity(input, user);

            user.CheckInitRoleType();

            if (input.SetRandomPassword)
            {
                input.Password = User.CreateRandomPassword();
            }

            if (!input.Password.IsNullOrEmpty())
            {
                CheckErrors(await _userManager.ChangePasswordAsync(user, input.Password));
            }

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
            }

            await SetOrganization(user, input.Organizations);

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                //await _userEmailer.SendEmailActivationLinkAsync(user, input.User.Password);
            }

            return MapToEntityDto(user);
        }


        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Delete)]
        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }


        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Delete)]
        public async Task BatchDelete(CommonDto.ArrayDto<long> input)
        {
            if (input.Value == null) { return; }
            foreach (var id in input.Value)
            {
                await DeleteAsync(new EntityDto<long>(id));
            }
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Active)]
        public async Task ToggleActiveStatus(EntityDto<long> input)
        {
            var user = await _userManager.FindByIdAsync("" + input.Id);
            user.IsActive = !user.IsActive;
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Unlock)]
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Unlock)]
        public async Task BatchUnlockUser(CommonDto.ArrayDto<long> input)
        {
            foreach (var id in input.Value)
            {
                await UnlockUser(new EntityDto<long> { Id = id });
            }
        }
        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Active)]
        public async Task BatchActiveUser(BatchActiveUserInput input)
        {
            var users = await _userManager.Users.Where(x => input.Ids.Contains(x.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.IsActive = input.IsActive;
            }
        }
        private async Task FillRoleNames<T>(List<T> userListDtos) where T : UserListDto
        {
            /* This method is optimized to fill role names to given list. */

            var distinctRoleIds = (
                from userListDto in userListDtos
                from userListRoleDto in userListDto.Roles
                select userListRoleDto.RoleId
                ).Distinct();

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                roleNames[roleId] = (await _roleManager.GetRoleByIdAsync(roleId)).DisplayName;
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }
        [AbpAuthorize]
        public async Task UpdateCurrentUser(UpdateCurrentUserInput input)
        {
            var id = AbpSession.GetUserId();
            var user = await _userManager.FindByIdAsync("" + id);

            //Update user properties
            input.MapTo(user); //Passwords is not mapped (see mapping configuration)

            CheckErrors(await _userManager.UpdateAsync(user));
        }


        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UpdateUserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to change password.");
            }
            long userId = _abpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Existing Password' did not match the one on record.  Please try again or contact an administrator for assistance in resetting your password.");
            }
            if (!new Regex(AccountAppService.PasswordRegex).IsMatch(input.NewPassword))
            {
                throw new UserFriendlyException("Passwords must be at least 8 characters, contain a lowercase, uppercase, and number.");
            }
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            CurrentUnitOfWork.SaveChanges();
            return true;
        }

        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to reset password.");
            }
            long currentUserId = _abpSession.UserId.Value;
            var currentUser = await _userManager.GetUserByIdAsync(currentUserId);
            var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
            }
            if (currentUser.IsDeleted || !currentUser.IsActive)
            {
                return false;
            }
            var roles = await _userManager.GetRolesAsync(currentUser);
            if (!roles.Contains(StaticRoleNames.Tenants.Admin))
            {
                throw new UserFriendlyException("Only administrators may reset passwords.");
            }

            var user = await _userManager.GetUserByIdAsync(input.UserId);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                CurrentUnitOfWork.SaveChanges();
            }

            return true;
        }


        public async Task<UserDto> GetUserOfUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            var user = await Repository.GetAll().Where(x => x.UserName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            return user.MapTo<UserDto>();
        }
        /// <summary>
        /// 获取全部人员下拉列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserMiniDto>> GetAllList(RoleType[] roleTypes)
        {
            var _roletype = roleTypes?.ToOne();
            var data = await Repository.GetAll()
                .WhereIf(_roletype.HasValue, x => (x.RoleType & _roletype) > 0)
                .ToListAsync();
            return data.MapTo<List<UserMiniDto>>();
        }

    }
}

