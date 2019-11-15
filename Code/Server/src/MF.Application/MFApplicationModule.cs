using Abp.AutoMapper;
using Abp.IO;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using MF.Authorization;
using MF.Configuration;
using MF.Configuration.Dto;
using MF.Menus;

namespace MF
{
    /// <summary>
    /// MFApplicationModule
    /// </summary>
    [DependsOn(
        typeof(MFCoreModule),
        typeof(AbpAutoMapperModule))]
    public class MFApplicationModule : AbpModule
    {
        /// <summary>  /// </summary>
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<MFAuthorizationProvider>();


        }

        /// <summary>  /// </summary>
        public override void Initialize()
        {
            var thisAssembly = typeof(MFApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            //Configuration.Modules.AbpAutoMapper().Configurators.Add(
            //    // Scan the assembly for classes which inherit from AutoMapper.Profile
            //    cfg => cfg.AddProfile(thisAssembly)
            //);
            Configuration.Modules.AbpAutoMapper().Configurators.Add(mapper =>
            {
                CustomDtoMapper.CreateMappings(mapper);
            });




            IocManager.IocContainer.Register(
                Component.For(typeof(IConfigurationService<>))
                .ImplementedBy(typeof(ConfigurationService<>)),

                Component.For(typeof(IConfigurationService<SecuritySettingDto>))
                .Named("SecurityConfigurationService")
                .ImplementedBy(typeof(SecurityConfigurationService)),

                Component.For(typeof(IConfigurationService<OSSSettingDto>))
                .Named("OSSSettingConfigurationService")
                .ImplementedBy(typeof(OSSSettingConfigurationService))
            );


            Configuration.Navigation.Providers.Add<DBNavigationProvider>();
            Configuration.Navigation.Providers.Add<CodeNavigationProvider>();
        }

        /// <summary>  /// </summary>
        public override void PostInitialize()
        {

            var server = HttpContext.Current;
            var appFolders = IocManager.Resolve<AppFolders>();

            appFolders.SampleProfileImagesFolder = server.MapPath("~/Common/Images/SampleProfilePics");
            appFolders.ImagesFolder = server.MapPath("~/Common/Images/UserPics");
            appFolders.TempFileDownloadFolder = server.MapPath("~/Temp/Downloads");
            appFolders.WebLogsFolder = server.MapPath("~/App_Data/Logs");
            appFolders.DragVerificationImageFolder = server.MapPath("~/App_Data/DragVerificationImage");

            try { DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder); } catch { }
        }
    }
}
