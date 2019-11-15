using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class CreateFolderInput
    {
        [Required]
        public string BucketName { get; set; }
        [Required]
        public string Folder { get; set; }
        public string[] TagNames { get; set; }
    }
}