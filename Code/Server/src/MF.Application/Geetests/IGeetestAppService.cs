using Abp.Application.Services;
using Abp.Web.Models;
using MF.DragVerifications.Dto;
using MF.Geetests.Dto;

namespace MF.Geetests
{
    public interface IGeetestAppService : IApplicationService
    {
        string GetCaptcha();
        CheckCodeOutput Check(GeetestCheckInput input);
        [DontWrapResult]
        GeetestCheckOutput APPGetCaptcha();
        [DontWrapResult]
        string APPCheck(GeetestAppCheckInput input);
    }
}