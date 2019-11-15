using System.Drawing;

namespace MF.Captcha
{
    internal interface IValidateImageBuilder
    {
        Image CreateImage(string code);
        int Height { get; set; }
        int Width { get; set; }
        int Difficulty { get; }
    }
}
