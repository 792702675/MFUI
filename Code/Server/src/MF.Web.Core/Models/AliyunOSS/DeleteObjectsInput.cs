using Aliyun.OSS;
using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class DeleteObjectsInput
    {
        [Required]
        public string BucketName { get; set; }
        public string[] FileName { get; set; }

        public DeleteObjectsRequest ToRequset()
        {
            return new DeleteObjectsRequest(BucketName,FileName);
        }
    }
}