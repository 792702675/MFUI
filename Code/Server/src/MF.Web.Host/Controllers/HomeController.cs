using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using MF.Controllers;
using MF.Web.Packager;

namespace MF.Web.Controllers
{
    public class HomeController : MFControllerBase
    {

        public IActionResult Index()
        {
            return Redirect("/index.html");
        }

        public FileResult DownloadServerSideCode(string name)
        {
            var serverPackager = new ServerSideCodePackager(name);
            serverPackager.Build();
            var virtualPath = serverPackager.GetVirtualPath();
            return File(virtualPath, "application/x-zip-compressed", Path.GetFileName(virtualPath));
        }


        public FileResult DownloadClientSideCode(string name)
        {
            var clientPackager = new ClientSideCodePackager(name);
            clientPackager.Build();
            var virtualPath = clientPackager.GetVirtualPath();
            return File(virtualPath, "application/x-zip-compressed",Path.GetFileName(virtualPath));
        }


        public string DownloadCode(string name)
        {
            if (Regex.IsMatch(name, "[^a-zA-Z_]"))
            {
                throw new UserFriendlyException("项目名称只能包含英文字母和下划线");
            }
            var clientPackager = new ClientSideCodePackager(name);
            var clientFile = clientPackager.Build();
            var serverPackager = new ServerSideCodePackager(name);
            var serverFile = serverPackager.Build();

            var output = Path.Combine(MFFrameworkSetting.Instance.OutputMappedPath, name);
            var finalFile = output + ".zip";
            Directory.CreateDirectory(output);
            System.IO.File.Move(clientFile, Path.Combine(output, Path.GetFileName(clientFile)));
            System.IO.File.Move(serverFile, Path.Combine(output, Path.GetFileName(serverFile)));
            if (System.IO.File.Exists(finalFile))
            {
                System.IO.File.Delete(finalFile);
            }
            ZipFile.CreateFromDirectory(output,finalFile);
            Directory.Delete(output, true);
            return finalFile.EncryptQueryString();
        }

        public FileResult Download(string token)
        {
            try
            {
                var finalFile = token.DecryptQueryString();
                var fileData= System.IO.File.ReadAllBytes( finalFile);
                System.IO.File.Delete(finalFile);
                return File(fileData, "application/x-zip-compressed",Path.GetFileName(finalFile));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}