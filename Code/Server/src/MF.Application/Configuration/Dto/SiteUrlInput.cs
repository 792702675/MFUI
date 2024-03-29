using System.ComponentModel.DataAnnotations;

namespace MF.Configuration.Dto
{
    /// <summary>
    /// 站点地址
    /// </summary>
    public class SiteUrlInput
    {
        /// <summary>
        /// 站点地址
        /// </summary>
        [Required]
        public string SiteUrl { get; set; }
    }
}