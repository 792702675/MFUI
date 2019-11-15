using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using OSS.Authentication.External;
using OSS.Authentication.JwtBearer;
using OSS.Authorization;
using OSS.Authorization.Users;
using OSS.Models.TokenAuth;
using OSS.MultiTenancy;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OSS.Net.MimeTypes;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OSS.Models.Setting;
using OSS.Configuration;
using Microsoft.Extensions.Configuration;

namespace OSS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SettingController : OSSControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IConfigurationRoot _appConfiguration;
        public SettingController(IHostingEnvironment hostingEnvironment,
            IApplicationLifetime applicationLifetime)
        {
            _hostingEnvironment = hostingEnvironment;
            _applicationLifetime = applicationLifetime;
            _appConfiguration = hostingEnvironment.GetAppConfiguration();
        }

        protected void CheckToken()
        {
            if (Request.Headers["Token"] != _appConfiguration["App:Token"])
            {
                throw new Abp.UI.UserFriendlyException("token验证失败");
            }
        }

        [HttpPost]
        public void Update(SettingDto input)
        {
            CheckToken();
            string contentPath = _hostingEnvironment.ContentRootPath + @"\";
            var filePath = contentPath + "appsettings.json";
            JObject jsonObject;
            using (StreamReader file = new StreamReader(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jsonObject = (JObject)JToken.ReadFrom(reader);
                if (input.CORSRule != null)
                {
                    jsonObject["App"]["CorsOrigins"] = string.Join(',', input.CORSRule.AllowedOrigins);
                    jsonObject["App"]["CorsHeaders"] = string.Join(',', input.CORSRule.AllowedHeaders);
                    jsonObject["App"]["CorsMethods"] = string.Join(',', input.CORSRule.AllowedMethods);
                    jsonObject["App"]["CorsPreflightMaxAge"] = input.CORSRule.MaxAgeSeconds + "";
                }
                if (input.RefererRule != null)
                {
                    jsonObject["App"]["RefererCanNull"] = input.RefererRule.AllowEmptyReferer.ToString();
                    jsonObject["App"]["Referers"] = string.Join(',', input.RefererRule.RefererList);
                }
            }

            using (var writer = new StreamWriter(filePath))
            using (JsonTextWriter jsonwriter = new JsonTextWriter(writer))
            {
                jsonwriter.Formatting = Formatting.Indented;
                jsonwriter.IndentChar = ' ';
                jsonwriter.Indentation = 2;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonwriter, jsonObject);
            }

            _applicationLifetime.StopApplication();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Pages_Users)]
        public void SetToken(string token)
        {
            SetValue("Token", token);
        }

        private void SetValue(string key, string value)
        {
            string contentPath = _hostingEnvironment.ContentRootPath + @"\";
            var filePath = contentPath + "appsettings.json";
            JObject jsonObject;
            using (StreamReader file = new StreamReader(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jsonObject = (JObject)JToken.ReadFrom(reader);
                jsonObject["App"][key] = value;
            }

            using (var writer = new StreamWriter(filePath))
            using (JsonTextWriter jsonwriter = new JsonTextWriter(writer))
            {
                jsonwriter.Formatting = Formatting.Indented;
                jsonwriter.IndentChar = ' ';
                jsonwriter.Indentation = 2;

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonwriter, jsonObject);
            }

            _applicationLifetime.StopApplication();
        }
    }
}
