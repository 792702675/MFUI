using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using MF.Identity;

using Abp.AspNetCore.SignalR.Hubs;
using System.IO;
using MF.QRLogin;
using MF.Web.Chat.SignalR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using MF.Authentication.JwtBearer;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.Json;
using Abp.Dependency;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace MF
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private readonly IConfiguration _appConfiguration;

        public Startup(IConfiguration configuration)
        {
            _appConfiguration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
                }
            ).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddSignalR();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MF API", Version = "v1" });
                //options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                //options.IncludeXmlComments(GetXmlCommentsPath("Web.Host"));
                //options.IncludeXmlComments(GetXmlCommentsPath("Web.Core"));
                //options.IncludeXmlComments(GetXmlCommentsPath("Application"));
            });

            // Configure Abp and Dependency Injection
            return services.AddAbp<MFWebHostModule>(
                 // Configure Log4Net logging
                 options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                     f => f.UseAbpLog4Net().WithConfig("log4net.config")
                 )
             );
        }
        private static string GetXmlCommentsPath(string subName)
        {
            return Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory).FirstOrDefault(n => n.ToLower().EndsWith(subName.ToLower() + ".xml"));
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings.Add(".less", "text/css");
            contentTypeProvider.Mappings.Add(".properties", "text/x-java-properties");
            contentTypeProvider.Mappings.Add(".bcmap", "image/svg+xml");
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = contentTypeProvider
            });


            app.UseRouting();

            app.UseAuthentication();

            app.UseAbpRequestLocalization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr");
                endpoints.MapHub<ChatHub>("/signalr-chat");
                endpoints.MapHub<QRLoginHub>("/qrLoginHub");

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MF API V1");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("MF.Web.Host.wwwroot.swagger.ui.index.html");
            }); // URL: /swagger


        }

    }
}
