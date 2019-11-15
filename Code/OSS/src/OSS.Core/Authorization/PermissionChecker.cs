using Abp.Authorization;
using OSS.Authorization.Roles;
using OSS.Authorization.Users;

namespace OSS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
