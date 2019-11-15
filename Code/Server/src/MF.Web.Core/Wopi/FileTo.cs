using System.IO;
using System.Text;

namespace MF.Wopi
{
    /// <summary>
    /// 文件读写
    /// </summary>
    public class FileTo
    {
        /// <summary>
        /// 流写入
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="path">物理目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="e">编码</param>
        /// <param name="isAppend">默认追加，false覆盖</param>
        public static void WriteText(string content, string path, string fileName, Encoding e, bool isAppend = true)
        {
            FileStream fs;

            //检测目录
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                fs = new FileStream(path + fileName, FileMode.Create);
            }
            else
            {
                //文件是否存在 创建 OR 追加
                if (!File.Exists(path + fileName))
                {
                    fs = new FileStream(path + fileName, FileMode.Create);
                }
                else
                {
                    FileMode fm = isAppend ? FileMode.Append : FileMode.Truncate;
                    fs = new FileStream(path + fileName, fm);
                }
            }

            //流写入
            StreamWriter sw = new StreamWriter(fs, e);
            sw.WriteLine(content);
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="isAppend"></param>
        public static void WriteText(string content, string path, string fileName, bool isAppend = true)
        {
            WriteText(content, path, fileName, Encoding.UTF8, isAppend);
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="path">物理目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="e">编码 默认UTF8</param>
        /// <returns></returns>
        public static string ReadText(string path, string fileName, Encoding e = null)
        {
            string result = string.Empty;

            try
            {
                if (e == null)
                {
                    e = Encoding.UTF8;
                }

                using (var sr = new StreamReader(Path.Combine( path , fileName), Encoding.Default))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (System.Exception)
            {
            }
            return result;
        }
    }
}
