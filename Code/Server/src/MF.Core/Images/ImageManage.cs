using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MF.Images
{
    public class ImageManage : MFDomainServiceBase
    {
        public static string TestImageUrl = "H:/我的酷盘/Image/psb.jpg";

        public ImageInfo GetImageInfo(string path)
        {
            using var file = File.OpenRead(path);
            return GetImageInfo(file);
        }
        public ImageInfo GetImageInfo(Stream stream)
        {
            using var image = Image.Load(stream);
            return new ImageInfo { Size = stream.Length, Width = image.Width, Height = image.Height };
        }

        /// <summary>
        /// 获取文件的缩略图
        /// </summary>
        public virtual Stream GetThumbnail(string path, int w = 120, int h = 90)
        {
            using var file = File.OpenRead(path);
            return GetThumbnail(file, w, h);
        }

        /// <summary>
        /// 获取文件的缩略图
        /// </summary>
        public virtual Stream GetThumbnail(Stream stream, int width = 120, int height = 90)
        {
            return Crop(stream, width, height);
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        public virtual Stream Crop(string path, int w = 120, int h = 90)
        {
            using var file = File.OpenRead(path);
            return Crop(file, w, h);
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        public virtual Stream Crop(Stream stream, int width = 120, int height = 90)
        {
            using var image = Image.Load(stream);
            image.Mutate(x => x
                 .Resize( new ResizeOptions() { Mode= ResizeMode.Crop,Size=new Size { Width= width, Height= height } })
                 );
            image.Save("H:/我的酷盘/Image/psb2.jpg");
            var s = new MemoryStream();
            image.SaveAsJpeg(s);
            return s;
        }
    }
}
