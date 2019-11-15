using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    /// <summary>
    /// 加or减标签
    /// </summary>
    public class BatchUpdateTagInput
    {
        [Required]
        public ObjectKey[] Source { get; set; }
        public string[] TagNames { get; set; }
    }
}