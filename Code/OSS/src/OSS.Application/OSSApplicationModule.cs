using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using OSS.Authorization;

namespace OSS
{
    [DependsOn(
        typeof(OSSCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class OSSApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<OSSAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(OSSApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
