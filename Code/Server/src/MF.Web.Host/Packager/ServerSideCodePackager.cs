using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MF.Web.Packager
{
    public class ServerSideCodePackager
    {
        private const string Source = "MF";
        private readonly string _target;
        private static readonly string TemplatePath = MFFrameworkSetting.Instance.ServerSideTemplatePath;

        private readonly List<string> _fileExtensions = new List<string> { ".cs", ".sln", ".csproj", ".asax" };

        private string FinalOutputDir { get; set; }

        public ServerSideCodePackager(string target)
        {
            _target = target;
            FinalOutputDir = Path.Combine(MFFrameworkSetting.Instance.OutputMappedPath, _target + "-server");
        }
        public string GetVirtualPath()
        {
            return  Path.Combine(MFFrameworkSetting.Instance.OutputDir, _target + "-server");
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

        private static Encoding GetFileEncodeType(FileStream fs)
        {
            BinaryReader br = new BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            fs.Seek(0, SeekOrigin.Begin);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return Encoding.UTF8;
                }
                if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
                if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }
                return Encoding.Default;
            }
            return Encoding.Default;
        }

        private void ScanFolder(string path)
        {
            var dir = new DirectoryInfo(path);
            var newPath = path.Replace(TemplatePath, "").Replace(Source, _target).Trim('\\');
            Directory.CreateDirectory(Path.Combine(FinalOutputDir, newPath));
            foreach (var subdir in dir.GetDirectories())
            {
                ScanFolder(subdir.FullName);
            }
            foreach (var file in dir.GetFiles())
            {
                var newFilePath = file.FullName.Replace(TemplatePath, "").Replace(Source, _target).Trim('\\');
                newFilePath = Path.Combine(FinalOutputDir, newFilePath);
                
                if (_fileExtensions.Contains(file.Extension.ToLower()))
                {
                    using (var inputStream = File.Open(file.FullName, FileMode.Open))
                    using (var outputStream = File.Open(newFilePath, FileMode.CreateNew))
                    {
                        var encoding = GetFileEncodeType(inputStream);
                        var reader = new StreamReader(inputStream, encoding);
                        var writer = new StreamWriter(outputStream, encoding);
                        
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            line = line?.Replace(Source, _target);
                            writer.WriteLine(line);
                        }
                        writer.Flush();
                    }
                }
                else
                {
                    File.Copy(file.FullName, newFilePath);
                }
            }
        }

        private string PackageCode()
        {
            var output = FinalOutputDir + ".zip";
            if (System.IO.File.Exists(output))
            {
                System.IO.File.Delete(output);
            }
            ZipFile.CreateFromDirectory( FinalOutputDir,output);
            Directory.Delete(FinalOutputDir, true);
            return output;
        }
    }
}