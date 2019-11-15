using Abp.AutoMapper;
using MF.Storage;
using System;
using System.ComponentModel.DataAnnotations;

namespace MF.CommonDto
{
    /// <summary>
    /// 富文本
    /// </summary>
    [AutoMap(typeof(ContentObject))]
    public class ContentObjectDto
    {
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
    }
}
