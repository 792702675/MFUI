using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using MF.OSS;
using MF.Storage;
using Microsoft.EntityFrameworkCore;

namespace MF.OSS
{
    [Owned]
    public class OssETagFile :  IOssETagFile
    {
        public string BucketName { get; set; }

        /// <summary>
        /// ETag
        /// </summary>
        public string ETag { get; set; }

        public string Url => this.GetUrl();
        public OssETagFile()
        {
            // 数据库不允许复杂对象为null
            BucketName = "";
            ETag = "";
        }

    }
}