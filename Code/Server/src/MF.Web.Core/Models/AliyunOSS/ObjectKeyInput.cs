using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class ObjectKeyInput
    {
        [Required]
        public string BucketName { get; set; }
        public string FileName { get; set; }
    }
}