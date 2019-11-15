using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Threading;
using MF.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;

namespace MF.FS430
{
    public class FS430Manage : MFDomainServiceBase
    {
        private string UpdateSettingUrl() => SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + "/api/setting/update";

        private string UploadFileUrl(string path) => SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + $"/api/file/create?path={path}";
        //public string GetFileUrl(string path) => SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + $"/api/file/getfilebypath?path={path}";
        private string GetDeleteUrl(string path) => SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + $"/api/file/delete?path={path}";
        private string GetCopyUrl(string path, string newPath) => SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + $"/api/file/copy?path={path}&newPath={newPath}";
        private string GetMoveUrl(string path, string newPath) => SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + $"/api/file/move?path={path}&newPath={newPath}";

        private readonly FS430WebClient _webClient;
        private readonly IRepository<SaltPot> _saltPotRepository;

        /// <summary>
        /// 是否启用加盐路径
        /// </summary>
        public bool EnableSalt { get; set; }

        public FS430Manage(
            FS430WebClient webClient,
            IRepository<SaltPot> saltPotRepository)
        {
            _webClient = webClient;
            _saltPotRepository = saltPotRepository;
            EnableSalt = true;
        }

        public void Update430Setting(Setting430 setting)
        {
            AsyncHelper.RunSync(() => _webClient.PostAsync(UpdateSettingUrl(), setting));
        }
        public async Task<string> UploadFileAsync(string path, Stream stream)
        {
            path = GetSaltPath(path);
            await Upload(UploadFileUrl(path), stream);
            return Guid.NewGuid().ToString("N");
        }

        private async Task Upload(string url, Stream stream)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Token", await SettingManager.GetSettingValueAsync(AppSettingNames.OSS.FS430.AccessKey));

            Directory.CreateDirectory("TempFile");
            var path = $"TempFile/{Guid.NewGuid().ToString()}";
            using (var fs = File.Create(path))
            {
                await stream.CopyToAsync(fs);
            }
            client.UploadFile(new Uri(url), path);
            File.Delete(path);
        }

        public string GetFileUrl(string path)
        {
            path = GetSaltPath(path);
            return SettingManager.GetSettingValue(AppSettingNames.OSS.FS430.Endpoint) + $"/file/{path}";
        }


        public async Task DeleteAsync(params string[] keys)
        {
            foreach (var key in keys)
            {
                var path = GetSaltPath(key);
                await _webClient.PostAsync(GetDeleteUrl(path));
            }
        }

        public async Task CopyAsync(string source, string destination)
        {
            source = GetSaltPath(source);
            destination = GetSaltPath(destination);
            await _webClient.PostAsync(GetCopyUrl(source, destination));
        }

        public async Task MoveAsync(string source, string destination)
        {
            source = GetSaltPath(source);
            destination = GetSaltPath(destination);
            await _webClient.PostAsync(GetMoveUrl(source, destination));
        }

        public string GetSaltPath(string path)
        {
            if (!EnableSalt) { return path; }
            var salt = _saltPotRepository.GetAll().FirstOrDefault(x => x.Key.ToLower() == path.ToLower());
            if (salt == null)
            {
                salt = new SaltPot { Key = path, Salt = Guid.NewGuid().ToString("N") };
                _saltPotRepository.InsertAndGetId(salt);
                UnitOfWorkManager.Current.SaveChanges();
            }

            return InsertSalt(path, salt.Salt);
        }
        private string InsertSalt(string path, string salt)
        {
            var sp = path.CorrectKey().Split('/');
            var _path = "";
            int i = 0;
            foreach (var item in sp)
            {
                if (i == sp.Length - 1)
                {
                    _path += "/" + salt;
                }
                _path += "/" + item;
                i++;
            }
            _path = _path.Trim('/');
            if (path.EndsWith("/")) { _path = _path.EnsureEndsWith('/'); }

            return _path;
        }

    }
}
