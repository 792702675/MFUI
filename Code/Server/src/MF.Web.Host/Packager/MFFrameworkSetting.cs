using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace MF.Web.Packager
{
    public class MFFrameworkSetting
    {
        public string ServerSideTemplatePath { get; set; }
        public string ClientSideTemplatePath { get; set; }
        public string OutputDir { get; set; }
        public List<string> ClientSideIgnoreFolders { get; set; }
        public List<string> ServerSideIgnoreFolders { get; set; }
        public List<string> ClientSideIgnoreFiles { get; set; }
        public List<string> ServerSideIgnoreFiles { get; set; }

        public List<CleanupCodeSetting> ServerSideCleanupCodes { get; set; }
        public List<CleanupCodeSetting> ClientSideCleanupCodes { get; set; }

        public string OutputMappedPath => HttpContext.Current.MapWebPath(OutputDir);


        private static readonly object LockObject = new object();
        private static MFFrameworkSetting _instance = null;
        public static MFFrameworkSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            var filepath = HttpContext.Current.MapPath("/App_Data/MFFrameworkSetting.json");
                            using (var stream = new StreamReader(filepath))
                            {
                                var json = stream.ReadToEnd();
                                _instance = JsonConvert.DeserializeObject<MFFrameworkSetting>(json);

                            }
                        }
                    }
                }
                return _instance;
            }
        }
    }
}