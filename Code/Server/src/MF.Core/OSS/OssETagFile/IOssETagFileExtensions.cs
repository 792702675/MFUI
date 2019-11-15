using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.OSS
{
    public static class IOssETagFileExtensions
    {
        /// <summary>
        /// 获取文件的URL
        /// </summary>
        /// <param name="ossETagIcon"></param>
        /// <returns></returns>
        public static string GetUrl(this IOssETagFile ossETagIcon)
        {
            var ossManager= IocManager.Instance.Resolve<OSSManage>();
            return ossManager.GetUrlOfETag(ossETagIcon.ETag, ossETagIcon.BucketName);
        }

        /// <summary>
        /// 获取图片的缩略图的URL
        /// </summary>
        /// <param name="ossETagIcon"></param>
        /// <returns></returns>
        public static string GetThumbnail(this IOssETagFile ossETagIcon)
        {
            var ossManager= IocManager.Instance.Resolve<OSSManage>();
            return ossManager.GetThumbnail(ossETagIcon.ETag, ossETagIcon.BucketName);
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="ossETagIcon"></param>
        /// <returns></returns>
        public static OSSObject GetInfo(this IOssETagFile ossETagIcon)
        {
            var ossManager = IocManager.Instance.Resolve<OSSManage>();
            return ossManager.GetOSSObject(ossETagIcon.ETag, ossETagIcon.BucketName);
        }
    }
}
