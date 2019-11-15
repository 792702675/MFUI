using System.IO;
using System.IO.Compression;
namespace MF.Web.Packager
{
    public class ClientSideCodePackager
    {
        private static readonly string TemplatePath = MFFrameworkSetting.Instance.ClientSideTemplatePath;
        private readonly string _target;

        private string FinalOutputDir { get; set; }

        public ClientSideCodePackager(string target)
        {
            _target = target;
            FinalOutputDir = Path.Combine(MFFrameworkSetting.Instance.OutputMappedPath, target+"-client");
        }
        public string GetVirtualPath()
        {
            return Path.Combine(MFFrameworkSetting.Instance.OutputDir, _target + "-client");
        }

        public string Build()
        {
            if (Directory.Exists(FinalOutputDir))
            {
                Directory.Delete(FinalOutputDir, true);
            }
            Directory.CreateDirectory(FinalOutputDir);
            ScanFolder(TemplatePath);
            return PackageCode();
        }

        private void ScanFolder(string path)
        {
            var dir = new DirectoryInfo(path);
            var newPath = path.Replace(TemplatePath, "").Trim('\\');
            Directory.CreateDirectory(Path.Combine(FinalOutputDir, newPath));
            foreach (var subdir in dir.GetDirectories())
            {
                ScanFolder(subdir.FullName);
            }
            foreach (var file in dir.GetFiles())
            {
                var newFilePath = file.FullName.Replace(TemplatePath, "").Trim('\\');
                newFilePath = Path.Combine(FinalOutputDir, newFilePath);
                File.Copy(file.FullName, newFilePath);
            }
        }

        private string PackageCode()
        {
            var output = FinalOutputDir + ".zip";
            if (System.IO.File.Exists(output))
            {
                System.IO.File.Delete(output);
            }
            ZipFile.CreateFromDirectory(FinalOutputDir,output);
            Directory.Delete(FinalOutputDir, true);
            return output;
        }
    }
}