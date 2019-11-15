using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class ObjectKey
    {
        [Required]
        public string BucketName { get; set; }
        public string Key { get; set; }

    }
}