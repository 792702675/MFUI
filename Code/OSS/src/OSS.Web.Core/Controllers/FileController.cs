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
using Microsoft.Extensions.Configuration;
using Abp.Extensions;
using Microsoft.Net.Http.Headers;
using OSS.Configuration;

namespace OSS.Controllers
{
    [Route("api/[controller]/[action]")]
    public class FileController : OSSControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _appConfiguration;
        public FileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _appConfiguration = hostingEnvironment.GetAppConfiguration();
        }

        protected void CheckToken()
        {
            if (Request.Headers["Token"] != _appConfiguration["App:Token"])
            {
                throw new Abp.UI.UserFriendlyException("token验证失败");
            }
        }

        protected void CheckReferer()
        {
            var applicationUrl = $"{Request.Scheme}://{Request.Host.Value}";
            var applicationUrls = _appConfiguration["App:Referers"]
                                 .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                 .Select(o => o.RemovePostFix("/"))
                                 .ToList();
            applicationUrls.Add(applicationUrl);
            var headersDictionary = Request.Headers;
            var urlReferrer = headersDictionary[HeaderNames.Referer].ToString();

            if (string.IsNullOrEmpty(urlReferrer))
            {
                if (!bool.Parse(_appConfiguration["App:RefererCanNull"]))
                {
                    throw new Abp.UI.UserFriendlyException("访问失败");
                }
            }
            else
            {
                if (!applicationUrls.Any(x => x == "*" || urlReferrer.StartsWith(x)))
                {
                    throw new Abp.UI.UserFriendlyException("访问失败");
                }
            }
        }

        protected List<string> GetAllFiles(DirectoryInfo dir, List<string> fileList)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                fileList.Add(fi.FullName);
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d, fileList);
            }
            return fileList;
        }
        [HttpPost]
        public List<string> GetAllPath()
        {
            CheckToken();
            List<string> fileList = new List<string>();
            GetAllFiles(new DirectoryInfo(_hostingEnvironment.WebRootPath + "\\File"), fileList);
            return fileList.Select(x => x.Replace(_hostingEnvironment.WebRootPath + "\\File", "").Replace("\\", "/")).ToList();
        }

        [HttpGet]
        public FileResult GetFileByPath(string path = "")
        {
            CheckReferer();
            string addrUrl = _hostingEnvironment.WebRootPath + "\\File\\" + path;

            if (!System.IO.File.Exists(addrUrl))
            {
                throw new Abp.UI.UserFriendlyException("不存在该文件");
            }

            var stream = System.IO.File.OpenRead(addrUrl);

            string fileExt = Path.GetExtension(path);


            var provider = new FileExtensionContentTypeProvider();

            var memi = provider.Mappings[fileExt];

            return File(stream, memi/*, Path.GetFileName(addrUrl)*/);
        }

        [HttpPost]
        public void Create(string path = "")
        {
            CheckToken();
            var files = Request.Form.Files;
            if (files.Count < 1)
            {
                throw new Abp.UI.UserFriendlyException("没有文件！");
            }
            foreach (var fileInfo in files)
            {
                var fullPath = Path.Combine(_hostingEnvironment.WebRootPath + "\\File\\", path.TrimStart('~').TrimStart('\\').TrimStart('/'));
                if (!Directory.Exists(new FileInfo(fullPath).Directory.FullName))
                {
                    Directory.CreateDirectory(new FileInfo(fullPath).Directory.FullName);
                }

                using (var fileStream = System.IO.File.OpenWrite(fullPath))
                {
                    fileInfo.OpenReadStream().CopyTo(fileStream);
                }
            }
        }

        [HttpPost]
        public bool Delete(string path = "")
        {
            CheckToken();
            string addrUrl = _hostingEnvironment.WebRootPath + "\\File\\" + path;

            if (!System.IO.File.Exists(addrUrl))
            {
                return false;
                throw new Abp.UI.UserFriendlyException("不存在该文件");
            }

            System.IO.File.Delete(addrUrl);

            return true;
        }

        [HttpPost]
        public void Copy(string path = "", string newPath = "")
        {
            CheckToken();
            string addrUrl = _hostingEnvironment.WebRootPath + "\\File\\" + path;

            if (!System.IO.File.Exists(addrUrl))
            {
                return;
                throw new Abp.UI.UserFriendlyException("不存在该文件");
            }

            if (newPath.EndsWith('/'))
            {
                if (!Directory.Exists(_hostingEnvironment.WebRootPath + "\\File\\" + newPath))
                {
                    Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "\\File\\" + newPath);
                }

                System.IO.File.Copy(addrUrl, _hostingEnvironment.WebRootPath + "\\File\\" + newPath + new FileInfo(_hostingEnvironment.WebRootPath + "\\File\\" + path).Name);
            }
            else
            {
                if (!Directory.Exists(new FileInfo(_hostingEnvironment.WebRootPath + "\\File\\" + newPath).Directory.FullName))
                {
                    Directory.CreateDirectory(new FileInfo(_hostingEnvironment.WebRootPath + "\\File\\" + newPath).Directory.FullName);
                }

                System.IO.File.Copy(addrUrl, _hostingEnvironment.WebRootPath + "\\File\\" + newPath);
            }

        }

        [HttpPost]
        public void Move(string path = "", string newPath = "")
        {
            CheckToken();
            string addrUrl = _hostingEnvironment.WebRootPath + "\\File\\" + path;

            if (!System.IO.File.Exists(addrUrl))
            {
                return;
                throw new Abp.UI.UserFriendlyException("不存在该文件");
            }

            if (newPath.EndsWith('/'))
            {
                if (!Directory.Exists(_hostingEnvironment.WebRootPath + "\\File\\" + newPath))
                {
                    Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "\\File\\" + newPath);
                }

                System.IO.File.Move(addrUrl, _hostingEnvironment.WebRootPath + "\\File\\" + newPath + new FileInfo(_hostingEnvironment.WebRootPath + "\\File\\" + path).Name);
            }
            else
            {
                if (!Directory.Exists(new FileInfo(_hostingEnvironment.WebRootPath + "\\File\\" + newPath).Directory.FullName))
                {
                    Directory.CreateDirectory(new FileInfo(_hostingEnvironment.WebRootPath + "\\File\\" + newPath).Directory.FullName);
                }

                System.IO.File.Move(addrUrl, _hostingEnvironment.WebRootPath + "\\File\\" + newPath);
            }
        }
    }
}
