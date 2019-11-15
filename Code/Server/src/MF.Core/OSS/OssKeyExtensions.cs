using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Abp.Extensions;

namespace MF
{
    public static class OssKeyExtensions
    {

        /// <summary>
        /// 判断指定的Key是文件 还是文件夹
        /// </summary>
        public static bool IsFile(this string key)
        {
            var fileName = Path.GetFileName(key);
            return !fileName.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// 纠正key
        /// </summary>
        /// <returns></returns>
        public static string CorrectKey(this string key)
        {
            var ck = key.Replace('\\', '/');
            while (ck.IndexOf("//") > 0)
            {
                ck = ck.Replace("//", "/");
            }
            ck = ck.TrimStart('/');
            return ck;
        }
        /// <summary>
        /// 获取指定Key的前导目录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetFolders(this string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                yield break;
            }
            var ck = key.CorrectKey();
            var fs = ck.Split('/').ToList();
            fs.Remove("");
            if (ck.IsFile() && fs.Count > 0)
            {
                fs.RemoveAt(fs.Count - 1);// 最后一个是文件名，要移除掉。
            }
            string pre = "";
            foreach (var item in fs)
            {
                var folder = pre == "" ? item.EnsureEndsWith('/') : pre.EnsureEndsWith('/') + item.EnsureEndsWith('/');
                yield return folder;
                pre = folder;
            }
        }

        public static bool IsImage(this string key)
        {
            var eName = Path.GetExtension(key);
            return   new string[]
            {
                ".jpg", ".jpeg", ".gif",
                ".bmp", ".png", ".webp",
                ".svg", ".ico"
            }
            .Contains(eName.ToLower());
        }

    }
}
