using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MF.WebFiles
{
    public class WebFileManager : IWebFileManager, ITransientDependency
    {
        public const string TempFileFolder = "~\\TempUploadFile";
        public const string UploadFile = "~\\UploadFile";
        private readonly IRepository<WebFile, Guid> _webFileRepository;
        string webRootPath ;

        private readonly IWebHostEnvironment _hostingEnvironment;
        public WebFileManager(IRepository<WebFile, Guid> webFileRepository, IWebHostEnvironment hostingEnvironment)
        {
            _webFileRepository = webFileRepository;
            _hostingEnvironment = hostingEnvironment;
            webRootPath = _hostingEnvironment.WebRootPath;
        }


        public Task<WebFile> GetOrNullAsync(Guid id)
        {
            return _webFileRepository.FirstOrDefaultAsync(id);
        }
        public async Task<FileInfo> GetFileInfoOrNullAsync(Guid id)
        {
            var webFile = await _webFileRepository.FirstOrDefaultAsync(id);
            if (webFile != null)
            {
                if (File.Exists(HttpContext.Current.MapPath(webFile.FilePath)))
                {
                    return new FileInfo(HttpContext.Current.MapPath(webFile.FilePath));
                }
            }
            return null;
        }

        public async Task<Guid> UploadFileAsync()
        {
            var httpFile = HttpContext.Current.Request.Form.Files[0];
            if (httpFile == null)
            {
                throw new Abp.UI.UserFriendlyException("没找到文件");
            }

            var webFile = new WebFile();
            webFile.FileName = httpFile.FileName;
            webFile.TempFilePath = $"{TempFileFolder}\\{DateTime.Now.ToString("yyyyMMdd")}";
            

            Directory.CreateDirectory(HttpContext.Current.MapPath(webFile.TempFilePath));
            webFile.TempFilePath = $"{ webFile.TempFilePath}\\{webFile.Id}";

            using (FileStream fs = System.IO.File.Create(Path.Combine( HttpContext.Current.MapPath(webFile.TempFilePath), httpFile.FileName)))
            {
                httpFile.CopyTo(fs);
                fs.Flush();
            }
            await _webFileRepository.InsertAsync(webFile);
            return webFile.Id;
        }

        public async Task UserFileAsync(Guid id, Guid? oldId = null, string newPath = UploadFile)
        {
            if (id == oldId) { return; }
            var webFile = await _webFileRepository.FirstOrDefaultAsync(id);
            if (webFile == null || !File.Exists(HttpContext.Current.MapPath(webFile.TempFilePath)))
            {
                throw new Abp.UI.UserFriendlyException("没找到文件");
            }
            Directory.CreateDirectory(HttpContext.Current.MapPath(newPath));
            webFile.FilePath = $"{newPath}\\{ webFile.Id}";
            File.Move(HttpContext.Current.MapPath(webFile.TempFilePath), HttpContext.Current.MapPath(webFile.FilePath));
            webFile.TempFilePath = null;

            if (oldId != null) { await DeleteAsync(oldId.Value); }
        }

        public async Task DownloadFileAsync(Guid id)
        {
            WebFile webFile = await GetWebFile(id);

            var Response = HttpContext.Current.Response;


            var path = webFile.FilePath;
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            //文件名必须编码，否则会有特殊字符(如中文)无法在此下载。
            string encodeFilename = HttpUtility.UrlEncode(webFile.FileName, Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
            Response.Body = memoryStream;
            
        }

        private async Task<WebFile> GetWebFile(Guid id)
        {
            var webFile = await _webFileRepository.FirstOrDefaultAsync(id);
            if (webFile == null || !File.Exists(HttpContext.Current.MapPath(webFile.FilePath)))
            {
                throw new Abp.UI.UserFriendlyException("没找到文件");
            }

            return webFile;
        }

        public async Task DeleteAsync(Guid id)
        {
            var webFile = await _webFileRepository.FirstOrDefaultAsync(id);
            if (webFile != null)
            {
                await DeleteAsync(webFile);
            }
        }

        private async Task DeleteAsync(WebFile webFile)
        {
            await _webFileRepository.DeleteAsync(webFile);
            if (File.Exists(HttpContext.Current.MapPath(webFile.FilePath)))
            {
                File.Delete(HttpContext.Current.MapPath(webFile.FilePath));
            }
            if (File.Exists(HttpContext.Current.MapPath(webFile.TempFilePath)))
            {
                File.Delete(HttpContext.Current.MapPath(webFile.TempFilePath));
            }
        }
    }
}
