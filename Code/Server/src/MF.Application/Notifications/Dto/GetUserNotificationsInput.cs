using Abp.Notifications;
using MF.CommonDto;

namespace MF.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }
    }
}