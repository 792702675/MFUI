using System.ComponentModel.DataAnnotations;

namespace OSS.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}