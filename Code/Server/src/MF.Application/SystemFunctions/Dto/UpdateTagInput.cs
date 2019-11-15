using Abp.Application.Services.Dto;

namespace MF.SysFuns
{
    public class UpdateTagInput: EntityDto
    {
        public string[] TagNames { get; set; }
    }
}