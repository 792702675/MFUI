using Abp.Application.Services;
using Abp.Application.Services.Dto;
using OSS.MultiTenancy.Dto;

namespace OSS.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

