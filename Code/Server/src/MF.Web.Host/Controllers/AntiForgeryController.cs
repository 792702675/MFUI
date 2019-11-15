using Microsoft.AspNetCore.Antiforgery;
using MF.Controllers;

namespace MF.Web.Host.Controllers
{
    public class AntiForgeryController : MFControllerBase
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
