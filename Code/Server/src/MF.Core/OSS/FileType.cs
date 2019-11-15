using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MF.OSS
{
    public class FileType
    {
        public string Type { get; set; }
        public string[] ExtensionName { get; set; }
        public string Icon { get; set; }
        public FileType(string type, string icon, string[] en)
        {
            Type = type;
            ExtensionName = en;
            Icon = icon;
        }

        public static FileType[] Config = new FileType[]
        {
            new FileType("zip",     "file-zip",     new string[]{ ".rar", ".7z", ".zip", ".iso", ".z", ".tar", ".wim", ".lzh" }),
            new FileType("excel",   "file-excel",   new string[]{ ".xls", ".xlsx", ".xlsm", ".xltx", ".xltm", ".xlsb", ".xlam", ".csv"}),
            new FileType("markdown","file-markdown",new string[]{ ".md" }),
            new FileType("html",    "global",       new string[]{ ".html",".htm"}),
            new FileType("code",    "code",         new string[]{ ".js",".css",".config",".json" ,".xml",".ts",".tsx",".java",".cs",".php",".c",".cpp"}),
            new FileType("text",    "file-text",    new string[]{ ".txt",".log"}),
            new FileType("word",    "file-word",    new string[]{ ".doc", ".docx", ".docm", ".dotx", ".dotm"}),
            new FileType("ppt",     "file-ppt",     new string[]{ ".ppt", ".pptx", ".pptm", ".ppsx", ".potx", ".potm", ".ppam"}),
            new FileType("pdf",     "file-pdf",     new string[]{ ".pdf" }),
            new FileType("video",   "video-camera", new string[]{ ".avi",".wmv",".mpeg", ".mpeg1", ".mpeg2", ".dv",".mkv",".rm",".rmvb",".mp4",".3gp",".flv",".f4v",".mov",".ogg",".mod",".m4v"}),
            new FileType("image",   null,           new string[]{ ".jpg", ".jpeg", ".gif", ".bmp", ".png", ".webp", ".svg", ".ico" }),
        };

        /// <summary>
        /// 判断文件是不是图片
        /// </summary>
        public static bool IsImage(string extensionName)
        {
            return Config.First(x => x.Type == "image").ExtensionName.Contains(extensionName.ToLower());
        }

        /// <summary>
        /// 判断文件是不是Office文件
        /// </summary>
        public static bool IsOffice(string extensionName)
        {
            var officeExtensionNames = Config
                .Where(x => new string[] { "word", "excel", "ppt" }.Contains(x.Type))
                .Select(x => x.ExtensionName)
                .SelectMany(x => x);
            return officeExtensionNames.Contains(extensionName.ToLower());
        }

        /// <summary>
        /// 尝试获取文件的图标，如果没有合适的图标 则返回null
        /// </summary>
        public static string TryGetIcon(string extensionName)
        {
            foreach (var item in Config)
            {
                if (item.ExtensionName.Contains(extensionName.ToLower()) && !item.Icon.IsNullOrEmpty())
                {
                    return item.Icon;
                }
            }
            return null;
        }


    }
}
