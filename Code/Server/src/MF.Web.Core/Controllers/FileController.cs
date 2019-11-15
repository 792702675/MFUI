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
using System.ComponentModel.DataAnnotations;
using Abp.Web.Models;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.SignalR;
using MF.QRLogin;
using Abp.Auditing;
using DataExporting.Dto;
using System.IO;

namespace MF.Controllers
{
    [Route("api/[controller]/[action]")]
    public class FileController : MFControllerBase
    {
        private readonly IAppFolders _appFolders;

        public FileController(IAppFolders appFolders)
        {
            _appFolders = appFolders;
        }

        [DisableAuditing]
        [HttpGet]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileToken);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException(L("RequestedFileDoesNotExists"));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, file.FileType, file.FileName);
        }
    }
}
