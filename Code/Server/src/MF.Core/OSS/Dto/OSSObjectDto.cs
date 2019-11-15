using Aliyun.OSS;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Web;
using Abp.Dependency;
using Abp.Configuration;
using MF.Configuration;

namespace MF.OSS
{
    public class OSSObjectDto
    {
        public OSSObjectDto(string bucketName, string eTag, string key, string url, long size = 0, DateTime? lastModified = null)
        {
            BucketName = bucketName;
            Url = url;
            ETag = eTag;
            Key = key;
            Size = size;
            LastModified = lastModified;
        }
        public int Id { get; set; }
        public string BucketName { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 文件名 或 文件夹名
        /// </summary>
        public string Name => Key.TrimEnd('/').Split("/")?.LastOrDefault();

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string ExtensionName => Path.GetExtension(Key);
        /// <summary>
        /// Id
        /// </summary>
        public string ETag { get; set; }


        /// <summary>
        /// 图片 分辨率 宽
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 图片 分辨率 高
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public string _Size
        {
            get
            {
                if (Size <= 0)
                {
                    return "0B";
                }

                var unit = new string[] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB", "NB", "DB", "CB", "XB", };
                var i = 0;
                while (Size >= Math.Pow(1000, i++)) ;
                var ruler = i - 2;
                return ((float)Size / (Math.Pow(1024, ruler))).ToString("0.00").TrimEnd('0').TrimEnd('.') + unit[ruler];
            }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// 是文件还是文件夹
        /// </summary>
        public bool IsFile => !Key.EndsWith("/");

        /// <summary>
        /// 如果是文件， 文件的Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 如果是Office文件， Office文件的Url
        /// </summary>
        public string OfficeViewUrl => GetOfficeViewUrl();
        private string GetOfficeViewUrl()
        {
            if (!IsFile) { return ""; }
            if (!FileType.IsOffice(ExtensionName)) { return ""; }

            var url = HttpUtility.UrlEncode(Url);
            var officeOnlineServerUrl = IocManager.Instance.Resolve<ISettingManager>().GetSettingValue(AppSettingNames.OSS.OfficeOnlineServerUrl);
            return $"{officeOnlineServerUrl}/op/view.aspx?src={url}";
        }


        /// <summary>
        /// 是否是图片文件
        /// </summary>
        public bool IsImage => FileType.IsImage(ExtensionName);

        /// <summary>
        /// 文件图标
        /// </summary>
        public string Icon => GetIcon();

        public string GetIcon()
        {
            if (!IsFile) { return "folder"; }
            if (IsImage) { return Url + "?x-oss-process=image/resize,w_150,h_120"; }
            return FileType.TryGetIcon(ExtensionName) ?? "file";
        }

        public string[] TagNames { get; set; }
    }

}
