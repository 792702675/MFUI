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
using MF.Authentication.External;
using MF.Authentication.JwtBearer;
using MF.Authorization;
using MF.Authorization.Users;
using MF.Models.TokenAuth;
using MF.MultiTenancy;
using MF.OSS;
using Aliyun.OSS;
using System.IO;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Extensions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Abp.AutoMapper;
using Abp.AspNetCore.Mvc.Authorization;
using MF.Buckets;
using MF.Wopi;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Abp.Web.Models;
using DataExporting.Net.MimeTypes;
using System.Text;

namespace MF.Controllers
{
    /// <summary>
    /// Wopi
    /// </summary>
    //[AbpMvcAuthorize(PermissionNames.Oss_Aliyun_Manage)]
    public class WopiController : MFControllerBase
    {

        private readonly OSSManage _aliyunOSSManager;
        private readonly BucketManage _bucketManage;
        private readonly IRepository<OSSObject> _oSSObjectRepository;

        public WopiController(
            IRepository<OSSObject> oSSObjectRepository,
            OSSManage aliyunOSSManager,
            BucketManage bucketManage)
        {
            _aliyunOSSManager = aliyunOSSManager;
            _oSSObjectRepository = oSSObjectRepository;
            _bucketManage = bucketManage;
        }
        [HttpGet]
        //[DontWrapResult]
        [Route("[controller]/files/{id}")]
        public async Task FileInfo(string id)
        {
            var fileinfo = new FileInfo(HttpContext.MapPath("word.docx"));
            DateTime? lastModifiedTime = DateTime.Now;
            try
            {
                CheckFileInfoResponse responseData = new CheckFileInfoResponse()
                {
                    BreadcrumbBrandName="",
                    //获取文件名称
                    BaseFileName = "word.docx",//Path.GetFileName(id),
                    Size = fileinfo.Length,
                    Version = (fileinfo.LastWriteTimeUtc).ToFileTimeUtc().ToString(),
                    SupportsLocks = true,
                    SupportsUpdate = true,
                    UserCanNotWriteRelative = true,
                    UserFriendlyName="HI",
                    UserId="0",
                    OwnerId="0",


                    ReadOnly = false,
                    UserCanWrite = true
                };
                var jsonString = JsonConvert.SerializeObject(responseData);

                ReturnSuccess(HttpContext.Response);

                await HttpContext.Response.WriteAsync(jsonString);

            }
            catch (UnauthorizedAccessException )
            {
                ReturnFileUnknown(HttpContext.Response);
            }
        }

        [Route("[controller]/files/{id}/contents")]
        [HttpGet]
        public async Task<IActionResult> Get(string id, string access_token)
        {
            await Task.FromResult(0);
            var fileInfo = new FileInfo("word.docx");
            var stream = fileInfo.OpenRead();
            HttpContext.Response.Headers.Add(WopiHeaders.ItemVersion, (fileInfo.LastWriteTimeUtc).ToFileTimeUtc().ToString());
            return new FileStreamResult(stream, "text/html");

        }

        
        [HttpPost]
        [Route("[controller]/files/{id}/contents")]
        public async Task PutFile(string id)
        {
            await Task.FromResult(0);
            using (var fileStream =System.IO. File.Open("word.docx", FileMode.Truncate, FileAccess.Write, FileShare.None))
            {
                HttpContext.Request.Body.CopyTo(fileStream);
            }
            HttpContext.Response.Headers.Add(WopiHeaders.ItemVersion, (new FileInfo("word.docx") .LastWriteTimeUtc).ToFileTimeUtc().ToString());
        }
      
        private static void ReturnSuccess(HttpResponse response)
        {
            ReturnStatus(response, 200, "Success");
        }

        private static void ReturnInvalidToken(HttpResponse response)
        {
            ReturnStatus(response, 401, "Invalid Token");
        }

        private static void ReturnFileUnknown(HttpResponse response)
        {
            ReturnStatus(response, 404, "File Unknown/User Unauthorized");
        }

        private static void ReturnLockMismatch(HttpResponse response, string existingLock = null, string reason = null)
        {
            response.Headers[WopiHeaders.Lock] = existingLock ?? string.Empty;
            if (!string.IsNullOrEmpty(reason))
            {
                response.Headers[WopiHeaders.LockFailureReason] = reason;
            }

            ReturnStatus(response, 409, "Lock mismatch/Locked by another interface");
        }

        private static void ReturnServerError(HttpResponse response)
        {
            ReturnStatus(response, 500, "Server Error");
        }

        private static void ReturnUnsupported(HttpResponse response)
        {
            ReturnStatus(response, 501, "Unsupported");
        }

        private static void ReturnStatus(HttpResponse response, int code, string description)
        {
            response.StatusCode = code;
            //response.StatusDescription = description;
        }

    }
}
