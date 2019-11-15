using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using MF.OSS;
using MF.Storage;
using Microsoft.EntityFrameworkCore;

namespace MF.OSS
{
    [AutoMap(typeof(OssETagFile))]
    public class ETagObject
    {
        public string BucketName { get; set; }

        /// <summary>
        /// ETag
        /// </summary>
        public string ETag { get; set; }
    }
}