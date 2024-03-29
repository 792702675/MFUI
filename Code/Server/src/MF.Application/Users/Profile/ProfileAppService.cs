using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Extensions;
using Abp.IO;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using MF.Users.Profile.Dto;
using MF.Configuration;
using MF.Security;
using MF.Storage;
using MF.Timing;
using Newtonsoft.Json;
using MF.Authorization;
using Abp.Application.Services.Dto;
using MF.Authorization.Users;
using Microsoft.AspNetCore.Identity;

namespace MF.Users.Profile
{
    [AbpAuthorize]
    public class ProfileAppService : MFAppServiceBase, IProfileAppService
    {
        private readonly IAppFolders _appFolders;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITimeZoneService _timeZoneService;
        private readonly PasswordComplexityChecker _passwordComplexityChecker;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly LogInManager _logInManager;

        public ProfileAppService(
            IAppFolders appFolders,
            IBinaryObjectManager binaryObjectManager,
            ITimeZoneService timezoneService,
            PasswordComplexityChecker passwordComplexityChecker,
            IPasswordHasher<User> passwordHasher,
             LogInManager logInManager)
        {
            _appFolders = appFolders;
            _binaryObjectManager = binaryObjectManager;
            _timeZoneService = timezoneService;
            _passwordComplexityChecker = passwordComplexityChecker;
            _passwordHasher = passwordHasher;
            _logInManager = logInManager;
        }

        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            var user = await GetCurrentUserAsync();
            var userProfileEditDto = user.MapTo<CurrentUserProfileEditDto>();

            if (Clock.SupportsMultipleTimezone)
            {
                userProfileEditDto.Timezone = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);

                var defaultTimeZoneId = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                if (userProfileEditDto.Timezone == defaultTimeZoneId)
                {
                    userProfileEditDto.Timezone = string.Empty;
                }
            }

            return userProfileEditDto;
        }

        public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            var user = await GetCurrentUserAsync();
            input.MapTo(user);
            CheckErrors(await UserManager.UpdateAsync(user));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.Timezone.IsNullOrEmpty())
                {
                    var defaultValue = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), TimingSettingNames.TimeZone, input.Timezone);
                }
            }
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            await CheckPasswordComplexity(input.NewPassword);

            var user = await GetCurrentUserAsync();
            user.ShouldChangePasswordOnNextLogin = false;
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException("原密码错误");
            }
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            //CheckErrors(await UserManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword));
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Edit)]
        public async Task ChangeUserPassword(ChangeUserPasswordInput input)
        {
            await CheckPasswordComplexity(input.NewPassword);

            var user = await UserManager.GetUserByIdAsync(input.UserId);
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            //CheckErrors(await UserManager.ChangePasswordAsync(user, input.NewPassword));
        }

        [AbpAuthorize(PermissionNames.Pages_Administration_Users_Edit)]
        public async Task<string> ResetUserPassword(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Password = _passwordHasher.HashPassword(user, User.DefaultPassword);
            //CheckErrors(await UserManager.ChangePasswordAsync(user, User.DefaultPassword));
            return User.DefaultPassword;
        }

        public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            var tempProfilePicturePath = Path.Combine(_appFolders.TempFileDownloadFolder, input.FileName);

            byte[] byteArray;

            using (var fsTempProfilePicture = new FileStream(tempProfilePicturePath, FileMode.Open))
            {
                using (var bmpImage = new Bitmap(fsTempProfilePicture))
                {
                    var width = input.Width == 0 ? bmpImage.Width : input.Width;
                    var height = input.Height == 0 ? bmpImage.Height : input.Height;
                    var bmCrop = bmpImage.Clone(new Rectangle(input.X, input.Y, width, height), bmpImage.PixelFormat);

                    using (var stream = new MemoryStream())
                    {
                        bmCrop.Save(stream, bmpImage.RawFormat);
                        stream.Close();
                        byteArray = stream.ToArray();
                    }
                }
            }

            if (byteArray.LongLength > 102400) //100 KB
            {
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit"));
            }

            var user = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            if (user.ProfilePictureId.HasValue)
            {
                await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
            }

            var storedFile = new BinaryObject(AbpSession.TenantId, byteArray);
            await _binaryObjectManager.SaveAsync(storedFile);

            user.ProfilePictureId = storedFile.Id;

            FileHelper.DeleteIfExists(tempProfilePicturePath);
        }

        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            var settingValue = await SettingManager.GetSettingValueAsync(AppSettingNames.Security.PasswordComplexity);
            var setting = JsonConvert.DeserializeObject<PasswordComplexitySetting>(settingValue);

            return new GetPasswordComplexitySettingOutput
            {
                Setting = setting
            };
        }

        private async Task CheckPasswordComplexity(string password)
        {
            await Task.FromResult(0);
            _passwordComplexityChecker.Check(password);
        }
    }
}