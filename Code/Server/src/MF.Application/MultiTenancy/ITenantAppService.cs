using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.MultiTenancy.Dto;

namespace MF.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

