using System.Threading.Tasks;
using Abp;
using Abp.Notifications;
using MF.Authorization.Users;
using MF.MultiTenancy;

namespace MF.Notifications
{
    public interface IAppNotifier
    {
        Task WelcomeToTheApplicationAsync(User user);

        Task NewUserRegisteredAsync(User user);

        Task NewTenantRegisteredAsync(Tenant tenant);

        Task SendMessageAsync(UserIdentifier user, string message, NotificationSeverity severity = NotificationSeverity.Info);

        Task HeroReachedAchievementAsync(long userId, string message, string content, string iconUrl = null);

        Task HeroLevelUpAsync(long userId, string message, string content, string iconUrl = null);

        Task HeroSkillLevelUpAsync(long userId, string message, string content, string iconUrl = null);
    }
}
