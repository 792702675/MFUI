using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace MF.Controllers
{
    public abstract class MFControllerBase: AbpController
    {
        protected MFControllerBase()
        {
            LocalizationSourceName = MFConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
