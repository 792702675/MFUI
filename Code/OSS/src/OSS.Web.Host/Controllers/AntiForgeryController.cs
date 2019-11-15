using Microsoft.AspNetCore.Antiforgery;
using OSS.Controllers;

namespace OSS.Web.Host.Controllers
{
    public class AntiForgeryController : OSSControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
