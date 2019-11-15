using Abp.Zero.Configuration;
using MF.OSSObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Configuration.Dto
{
    [Tab("文件存储设置", 4)]
    public class OSSSettingDto : ISettingDto
    {
        [DisplayName("OfficeOnlineServerUrl")]
        [Key(AppSettingNames.OSS.OfficeOnlineServerUrl)]
        public string OfficeOnlineServerUrl { get; set; }

        [DisplayName("最多允许的存储库")]
        [Key(AppSettingNames.OSS.BucketCounct)]
        public int BucketCounct { get; set; }

        [DisplayName("存储库名称前缀")]
        [Key(AppSettingNames.OSS.BucketPrefix)]
        public string BucketPrefix { get; set; }

        [DisplayName("富文本存储库")]
        [Key(AppSettingNames.OSS.ContextStore)]
        [EnumSelect(typeof(IOSSObjectAppService), "GetBucketList")]
        public string ContextStore { get; set; }


        [Title("禁止存储文件设置")]
        [DisplayName("文件类型（扩展名）")]
        [Key(AppSettingNames.OSS.ProhibitedFileType)]
        [TextArea]
        public string ProhibitedFileType { get; set; }

        [Title("跨域规则")]
        [DisplayName("来源")]
        [Key(AppSettingNames.OSS.AllowedOrigins)]
        [TextArea]
        public string AllowedOrigins { get; set; }

        [DisplayName("允许 Methods")]
        [Key(AppSettingNames.OSS.AllowedMethods)]
        [TextArea]
        public string AllowedMethods { get; set; }

        [DisplayName("允许 Headers")]
        [Key(AppSettingNames.OSS.AllowedHeaders)]
        [TextArea]
        public string AllowedHeaders { get; set; }

        [DisplayName("暴露 Headers")]
        [Key(AppSettingNames.OSS.ExposedHeaders)]
        [TextArea]
        public string ExposedHeaders { get; set; }

        [DisplayName("缓存时间（秒）")]
        [Key(AppSettingNames.OSS.MaxAgeSeconds)]
        public int MaxAgeSeconds { get; set; }

        [Title("防盗链")]
        [DisplayName("允许空 Referer")]
        [Key(AppSettingNames.OSS.AllowEmptyReferer)]
        public bool AllowEmptyReferer { get; set; }

        [DisplayName("允许空 Referer")]
        [Key(AppSettingNames.OSS.Referer)]
        [TextArea]
        public string Referer { get; set; }


        [Title("独立文件服务器设置")]
        [DisplayName("Endpoint")]
        [Key(AppSettingNames.OSS.FS430.Endpoint)]
        public string FS430Endpoint { get; set; }

        [DisplayName("AccessKey")]
        [Key(AppSettingNames.OSS.FS430.AccessKey)]
        public string FS430AccessKey { get; set; }

        [DisplayName("文件类型(扩展名)")]
        [Key(AppSettingNames.OSS.FS430.FileType)]
        [TextArea]
        public string FS430FileType { get; set; }




        [Title("阿里OSS设置")]
        [DisplayName("Endpoint")]
        [Key(AppSettingNames.OSS.Aliyun.Endpoint)]
        public string AliyunEndpoint { get; set; }

        [DisplayName("AccessKeyId")]
        [Key(AppSettingNames.OSS.Aliyun.AccessKeyId)]
        public string AliyunAccessKeyId { get; set; }

        [DisplayName("AccessKeySecret")]
        [Key(AppSettingNames.OSS.Aliyun.AccessKeySecret)]
        public string AliyunAccessKeySecret { get; set; }
    }
}
