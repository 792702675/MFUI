using Abp.Domain.Entities.Auditing;
using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MF.Buckets

{

    /// <summary>
    /// 储存空间
    /// </summary>
    public class Bucket430 : FullAuditedEntity
    {
        public Bucket430()
        {
            StorageClass = StorageClass.Standard;
            CannedAccessControl = CannedAccessControlList.PublicRead;
            Owner = MFConsts.SystemName;
        }



        /// <summary>
        /// Name
        /// </summary>
        [StringLength(64)]
        public string Name { get; set; }
        /// <summary>
        /// Note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 所属系统（任何情况下，系统都只能访问自己系统创建的储存空间）
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 存储类型
        /// </summary>
        public StorageClass StorageClass { get; set; }

        /// <summary>
        /// ACL权限
        /// </summary>
        public CannedAccessControlList CannedAccessControl { get; set; }

        // 跨域规则

        public string _AllowedOrigins { get; set; }
        /// <summary>
        /// 指定允许跨域请求的来源
        /// </summary>
        [NotMapped]
        public string[] AllowedOrigins => _AllowedOrigins.ReadAllLine();

        public string _AllowedMethods { get; set; }
        /// <summary>
        /// 指定允许的跨域请求方法(GET/PUT/DELETE/POST/HEAD)
        /// </summary>
        [NotMapped]
        public string[] AllowedMethods => _AllowedMethods.ReadAllLine();

        public string _AllowedHeaders { get; set; }
        /// <summary>
        /// 是否允许预取指令（OPTIONS）中Access-Control-Request-Headers头中指定的Header。
        /// </summary>
        [NotMapped]
        public string[] AllowedHeaders => _AllowedHeaders.ReadAllLine();

        public string _ExposedHeaders { get; set; }
        /// <summary>
        /// 暴露Headers  指定允许用户从应用程序中访问的响应头。
        /// </summary>
        [NotMapped]
        public string[] ExposedHeaders => _ExposedHeaders.ReadAllLine(); 


        /// <summary>
        /// 缓存时间（s） 
        /// </summary>
        public int MaxAgeSeconds { get; set; }

        /// <summary>
        /// 跨域规则
        /// </summary>
        public SetBucketCorsRequest ToCORSRules()
        {
            var request = new SetBucketCorsRequest(Name);
            var rule = new CORSRule();
            rule.MaxAgeSeconds = MaxAgeSeconds;
            if (AllowedOrigins != null && AllowedOrigins.Length > 0)
            {
                AllowedOrigins.ToList().ForEach(x => rule.AddAllowedOrigin(x));
            }
            else
            {
                throw new Abp.UI.UserFriendlyException("AllowedOrigins 不能是空");
            }
            if (AllowedHeaders != null && AllowedHeaders.Length > 0)
            {
                AllowedHeaders.ToList().ForEach(x => rule.AddAllowedHeader(x));
            }
            if (AllowedMethods != null && AllowedMethods.Length > 0)
            {
                AllowedMethods.ToList().ForEach(x => rule.AddAllowedMethod(x));
            }
            else
            {
                throw new Abp.UI.UserFriendlyException("AllowedMethod 不能是空");
            }
            if (ExposedHeaders != null && ExposedHeaders.Length > 0)
            {
                if (ExposedHeaders.Contains("*"))
                {
                    throw new Abp.UI.UserFriendlyException("ExposedHeaders 不能使用*通配符");
                }
                ExposedHeaders.ToList().ForEach(x => rule.AddExposeHeader(x));
            }

            request.AddCORSRule(rule);
            return request;

        }

        // 防盗链

        /// <summary>
        /// 允许空Referer
        /// </summary>
        public bool AllowEmptyReferer { get; set; }
        public string _RefererList { get; set; }
        /// <summary>
        ///  添加Referer白名单。Referer参数支持通配符星号（*）和问号（？）
        /// </summary>
        [NotMapped]
        public string[] RefererList { get { return _RefererList?.Split(','); } set { _RefererList = string.Join(',', value); } }



    }
}
