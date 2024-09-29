using System.Drawing;
using System.Drawing.Drawing2D;

namespace CarDealer.Service
{
    public static class ImageHelper
    {
        public static void ResizeImage(Stream inputStream, string outputPath, int width, int height)
        {
            using (var image = Image.FromStream(inputStream))
            {
                var aspectRatio = (double)image.Width / image.Height;
                var newHeight = (int)(width / aspectRatio);
            
                   using(var thumbnail = new Bitmap(width, newHeight))
                {
                   using(var graphics = Graphics.FromImage(thumbnail))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, width, newHeight);
                        thumbnail.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png );
                    }

                }
            }
        }
    }
}
