using Xunit;

namespace MF.Tests
{
    public sealed class MultiTenantFactAttribute : FactAttribute
    {
        public MultiTenantFactAttribute()
        {
            if (!MFConsts.MultiTenancyEnabled)
            {
                //Skip = "MultiTenancy is disabled.";
            }
        }
    }
}
