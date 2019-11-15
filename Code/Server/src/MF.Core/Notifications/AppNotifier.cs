using System.Threading.Tasks;
using Abp;
using Abp.Localization;
using Abp.Notifications;
using MF.Authorization.Users;
using MF.MultiTenancy;
using Abp.Domain.Services;
using System.Linq;

namespace MF.Notifications
{
    public class AppNotifier : MFDomainServiceBase, IAppNotifier
    {
        private readonly INotificationPublisher _notificationPublisher;

        public AppNotifier(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public async Task WelcomeToTheApplicationAsync(User user)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.WelcomeToTheApplication,
                new MessageNotificationData("欢迎使用本系统"),
                severity: NotificationSeverity.Success,
                userIds: new[] { user.ToUserIdentifier() }
                );
        }

        public async Task NewUserRegisteredAsync(User user)
        {
            var notificationData = new MessageNotificationData("有一个新用户注册了");

            notificationData["userName"] = user.UserName;
            notificationData["emailAddress"] = user.EmailAddress;
            notificationData["phoneNumber"] = user.PhoneNumber;
            notificationData["content"] = $"用户名：【{user.UserName}】，手机号：【{user.PhoneNumber}】，邮箱：【{user.EmailAddress}】。";


            await _notificationPublisher.PublishAsync(AppNotificationNames.NewUserRegistered, notificationData, tenantIds: new[] { user.TenantId });
        }

        public async Task NewTenantRegisteredAsync(Tenant tenant)
        {
            var notificationData = new MessageNotificationData("有一个新租户注册了");

            notificationData["tenancyName"] = tenant.TenancyName;
            await _notificationPublisher.PublishAsync(AppNotificationNames.NewTenantRegistered, notificationData);
        }

        //This is for test purposes
        public async Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info)
        {
            await _notificationPublisher.PublishAsync(
                "App.SimpleMessage",
                new MessageNotificationData(message),
                severity: severity,
                userIds: new[] { user }
                );
        }

        public async Task SendUserNotification(string type, string title, string content, string iconUrl, params long[] userIds)
        {
            await SendUserMessageAsync(AppNotificationNames.UserHtmlNotification, type, title, content, iconUrl, userIds);
        }
        public async Task SendUserMessageAsync(string notificationName,  string type, string title, string content, string iconUrl, params long[] userIds )
        {
            var notificationData = new MessageNotificationData(title);
            notificationData["content"] = content;
            notificationData["type"] = type;
            if (!string.IsNullOrEmpty(iconUrl))
            {
                notificationData["icon"] = iconUrl;
            }
            await _notificationPublisher.PublishAsync(
                notificationName,
                notificationData,
                severity: NotificationSeverity.Success,
                userIds: userIds.Select(x=>new UserIdentifier(AbpSession.TenantId, x)).ToArray()
                );
        }

        private async Task SendUserMessageAsync(string type, long userId, string message, string content, string iconUrl = null)
        {
            await SendUserMessageAsync(AppNotificationNames.HeroReachedAchievement, type,  message,  content,  iconUrl,  userId );
        }
        public async Task HeroReachedAchievementAsync(long userId, string message, string content, string iconUrl = null)
        {
            await SendUserMessageAsync("英雄获得成就", userId, message, content, iconUrl);
        }

        public async Task HeroLevelUpAsync(long userId, string message, string content, string iconUrl = null)
        {
            await SendUserMessageAsync("英雄升级", userId, message, content, iconUrl);
        }

        public async Task HeroSkillLevelUpAsync(long userId, string message, string content, string iconUrl = null)
        {
            await SendUserMessageAsync("英雄技能等级", userId, message, content, iconUrl);
        }
    }
}
