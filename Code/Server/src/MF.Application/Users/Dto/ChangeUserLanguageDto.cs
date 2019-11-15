using System.ComponentModel.DataAnnotations;

namespace MF.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}