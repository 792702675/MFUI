using Abp.Application.Services;
using MF.DragVerifications.Dto;

namespace MF.DragVerifications
{
    public interface IDragVerificationAppService : IApplicationService
    {
        CheckCodeOutput CheckCode(CheckCodeInput input);
        DragVerificationDto GetDragVerificationCode();
    }
}