using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MF.Configuration;
using System;

namespace MF
{
    [DependsOn(
       typeof(MFWebCoreModule))]
    public class MFWebHostModule: AbpModule
    {
        private readonly IConfiguration _appConfiguration;

        public MFWebHostModule(IConfiguration configuration)
        {
            _appConfiguration = configuration;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MFWebHostModule).GetAssembly());

            HttpContext.ServiceProvider = IocManager.Resolve<IServiceProvider>();
        }

    }
}
