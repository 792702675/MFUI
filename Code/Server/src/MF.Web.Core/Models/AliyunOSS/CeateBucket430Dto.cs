using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using Aliyun.OSS;
using MF.Buckets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MF.OSS

{

    /// <summary>
    /// 储存空间
    /// </summary>
    [AutoMap(typeof(Bucket430))]
    public class CeateBucket430Dto 
    {
        public CeateBucket430Dto()
        {
        }


        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [StringLength(64)]
        public string Name { get; set; }

        // 跨域规则

        /// <summary>
        /// 指定允许跨域请求的来源
        /// </summary>
        public string[] AllowedOrigins { get; set; }

        /// <summary>
        /// 指定允许的跨域请求方法(GET/PUT/DELETE/POST/HEAD)
        /// </summary>
        public string[] AllowedMethods { get; set; }

        /// <summary>
        /// 是否允许预取指令（OPTIONS）中Access-Control-Request-Headers头中指定的Header。
        /// </summary>
        public string[] AllowedHeaders { get; set; }

        /// <summary>
        /// 暴露Headers  指定允许用户从应用程序中访问的响应头。
        /// </summary>
        public string[] ExposedHeaders { get; set; }

        /// <summary>
        /// 缓存时间（s） 
        /// </summary>
        public int MaxAgeSeconds { get; set; }


        // 防盗链

        /// <summary>
        /// 允许空Referer
        /// </summary>
        public bool AllowEmptyReferer { get; set; }
        /// <summary>
        ///  添加Referer白名单。Referer参数支持通配符星号（*）和问号（？）
        /// </summary>
        public string[] RefererList { get; set; }



    }
}
