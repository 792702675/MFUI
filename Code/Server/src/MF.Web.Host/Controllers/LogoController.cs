using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Abp;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using MF.Authorization.Users;
using MF.IO;
using MF.Storage;
using Abp.IO;
using DataExporting.Net.MimeTypes;
using MF.Controllers;
using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace MF.Controllers
{
    public class LogoController : MFControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IAppFolders _appFolders;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public LogoController(
            UserManager userManager,
            IBinaryObjectManager binaryObjectManager,
            IAppFolders appFolders,
            IWebHostEnvironment hostingEnvironment
            )
        {
            _userManager = userManager;
            _binaryObjectManager = binaryObjectManager;
            _appFolders = appFolders;
            _hostingEnvironment = hostingEnvironment;
        }

        [DisableAuditing]
        public FileResult GetLogoPicture()
        {
            return File("/Common/logo.png", MimeTypeNames.ImagePng);
        }
        private IActionResult DownLoad(string path)

        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var addrUrl = webRootPath + path;

            var stream = System.IO.File.OpenRead(addrUrl);

            string fileExt = Path.GetExtension(path);

            //��ȡ�ļ���ContentType

            var provider = new FileExtensionContentTypeProvider();

            var memi = provider.Mappings[fileExt];

            return File(stream, memi, Path.GetFileName(addrUrl));

        }

        [AbpMvcAuthorize]
        public JsonResult UploadLogoPicture()
        {
            try
            {
                //Check input
                if (Request.Form.Files.Count <= 0 || Request.Form.Files[0] == null)
                {
                    throw new UserFriendlyException("δ�ҵ��ļ�");
                }

                var file = Request.Form.Files[0];

                if (file.Length > 30 * 1024)
                {
                    throw new UserFriendlyException("logo�ļ����ܴ���30kb");
                }

                //Check file type & format
                var fileImage = Image.FromStream(file.OpenReadStream());
                var acceptedFormats = new List<ImageFormat>
                {
                    ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif
                };

                if (!acceptedFormats.Contains(fileImage.RawFormat))
                {
                    throw new ApplicationException("δ��ʶ������͵��ļ�");
                }
                var path = HttpContext.MapWebPath("Common/logo.png");
                FileHelper.DeleteIfExists(path);
                using (var fileStream = System.IO.File.OpenWrite(path))
                {
                    file.CopyTo(fileStream);
                }

                using (var bmpImage = new Bitmap(path))
                {
                    return Json(new AjaxResponse(new { fileName = "Common/logo.png", width = bmpImage.Width, height = bmpImage.Height }));
                }
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}