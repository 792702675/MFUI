using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace OSS.Controllers
{
    public abstract class OSSControllerBase: AbpController
    {
        protected OSSControllerBase()
        {
            LocalizationSourceName = OSSConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
