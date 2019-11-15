using Abp.AutoMapper;
using Abp.Extensions;
using Abp.Runtime.Validation;
using MF.CommonDto;

namespace MF.Tasks.Dto
{
    public class PagedSortedDefaultWithOrderAndFilteredInputDto : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "Order";
            }
        }
    }
}
