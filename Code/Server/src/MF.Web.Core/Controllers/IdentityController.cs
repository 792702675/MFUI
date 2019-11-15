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
using Microsoft.AspNetCore.Authorization;

namespace MF.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize()]
    public class IdentityController : MFControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
