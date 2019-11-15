using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace MF.Storage
{
    /// <summary>
    /// 富文本存储库    
    /// 引用删除时，需要同时软删除本对象(引用时，应该是可空的，不然就级联删除了)
    /// 使用该对象，需要注册ContentObjectRelateManager中的事件，使Related自动完成
    /// </summary>
    public class ContentObject : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// 富文本内容(原始内容)
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// 显示内容
        /// </summary>
        public virtual string HtmlContent { get; set; }

        /// <summary>
        /// 用于【本文相关】
        /// </summary>
        public Guid? Group { get; set; }


        /// <summary>
        /// 是否更新过
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 静态化引用地址
        /// </summary>
        public string HtmlUrl { get; set; }

        /// <summary>
        /// 最后一次静态化时间
        /// </summary>
        public DateTime? LastStaticTime { get; set; }

        /// <summary>
        /// 相关联的功能
        /// </summary>
        public string RelatedFunction { get; set; }

        /// <summary>
        /// 相关联的记录Id
        /// </summary>
        public string RelatedId { get; set; }


        public ContentObject()
        {
            IsUpdate = true;
            LastUpdateTime = DateTime.Now;
        }

    }

    
    
}
