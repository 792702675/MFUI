using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.Wopi
{
    public class WopiConfig
    {
        public static JObject Pconfig { get; set; }

        public static JObject Config
        {
            get
            {
                if (Pconfig == null)
                {
                    var path = HttpContext.Current.MapPath("/");
                    Pconfig = JObject.Parse(FileTo.ReadText(path, "config.json"));
                }
                return Pconfig;
            }
        }

        public static string WopiPath
        {
            get
            {
                return Config["WopiPath"].ToString();
            }
        }
        public static string FilesRequestPath
        {
            get
            {
                return Config["FilesRequestPath"].ToString();
            }
        }
        public static string FoldersRequestPath
        {
            get
            {
                return Config["FoldersRequestPath"].ToString();
            }
        }
        public static string ContentsRequestPath
        {
            get
            {
                return Config["ContentsRequestPath"].ToString();
            }
        }
        public static string ChildrenRequestPath
        {
            get
            {
                return Config["ChildrenRequestPath"].ToString();
            }
        }
        public static string LocalStoragePath
        {
            get
            {
                var lsp = Config["LocalStoragePath"].ToString();
                if (string.IsNullOrWhiteSpace(lsp))
                {
                    lsp = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").TrimEnd('/') + "/upload/";
                }
                return lsp;
            }
        }


        public static string BreadcrumbBrandName
        {
            get
            {
                return Config["BreadcrumbBrandName"].ToString();
            }
        }
    }
}
