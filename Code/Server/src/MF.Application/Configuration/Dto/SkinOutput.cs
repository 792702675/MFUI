using System.ComponentModel.DataAnnotations;

namespace MF.Configuration.Dto
{
    public class SkinOutput
    {
        [Required]
        [MaxLength(32)]
        public string Name { get; set; }

        //public string Path => $"/Common/Skin/{Name}.css";
    }
}