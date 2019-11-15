using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using Aliyun.OSS;
using MF.Buckets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MF.FS430

{

    /// <summary>
    /// 430文件系统的设置dto
    /// </summary>
    public class Setting430
    {
        public CORSRule CORSRule { get; set; }
        public SetBucketRefererRequest RefererRule { get; set; }

        public Setting430(CORSRule cORSRule, SetBucketRefererRequest refererRule)
        {
            CORSRule = cORSRule;
            RefererRule = refererRule;
        }
    }
}
