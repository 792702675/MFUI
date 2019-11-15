using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Auditing.Dto;
using DataExporting.Dto;
using MF.Demos.Dto;
using System.Collections.Generic;
using MF.CommonDto;

namespace MF.Demos
{
    /// <summary>
    /// Demo
    /// </summary>
    public interface IDemoAppService:IAsyncCrudAppService<GetListDemoDto, int, PagedSortedAndFilteredInputDto, CreateDemoDto, UpdateDemoDto>
    {
    }
}
