using Abp.Authorization;
using MF.Authorization.Roles;
using MF.Authorization.Users;

namespace MF.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
