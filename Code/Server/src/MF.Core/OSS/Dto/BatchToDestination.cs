using System.ComponentModel.DataAnnotations;

namespace MF.OSS
{
    public class BatchToDestination
    {
        [Required]
        public ObjectKey[] Source { get; set; }
        [Required]
        public ObjectKey Destination { get; set; }
    }
}