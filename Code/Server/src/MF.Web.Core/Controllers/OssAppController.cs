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
using MF.Models.AliyunOSS;

namespace MF.Controllers
{
    /// <summary>
    /// OSS存储库管理  应用使用
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class OssAppController : MFControllerBase
    {

        private readonly OSSManage _aliyunOSSManager;
        private readonly BucketManage _bucketManage;
        private readonly IRepository<OSSObject> _oSSObjectRepository;

        public OssAppController(
            IRepository<OSSObject> oSSObjectRepository,
            OSSManage aliyunOSSManager,
            BucketManage bucketManage)
        {
            _aliyunOSSManager = aliyunOSSManager;
            _oSSObjectRepository = oSSObjectRepository;
            _bucketManage = bucketManage;
        }


        /// <summary>
        /// 创建本文相关的对象
        /// </summary>
        /// <param name="group">第一次不用带，第二次需要带</param>
        [HttpPost]
        public async Task<GroupFileDto> CreateObjectRelatedToThisArticle(Guid? group)
        {
            var files = Request.Form.Files;
            if (files.Count < 1)
            {
                throw new Abp.UI.UserFriendlyException("没有文件！");
            }
            var fileInfo = files
                .Select(x => new
                {
                    fileName = Guid.NewGuid() + Path.GetExtension(x.FileName),
                    Stream = x.OpenReadStream()
                })
                .ToArray();

            var eTagFiles = new List<OssETagFileDto>();
            foreach (var item in fileInfo)
            {
                OssETagFileDto file;
                (group, file) = await _aliyunOSSManager.CreateObjectRelatedToThisArticle(group, item.fileName, item.Stream);
                eTagFiles.Add(file);
            }
            return new GroupFileDto { Group = group.Value, FileInfo = eTagFiles };
        }
    }
}
