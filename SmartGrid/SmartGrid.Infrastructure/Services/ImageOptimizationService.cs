using SmartGrid.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace SmartGrid.Infrastructure.Services
{
    internal class ImageOptimizationService : IImageOptimizationService
    {
        public byte[] Optimize(byte[] imageContent, int maxWidth = 1280, int quality = 75)
        {
            using var image = Image.Load(imageContent);

            if (image.Width > maxWidth)
            {
                var targetHeight = (int)Math.Round(image.Height * (maxWidth / (double)image.Width));
                image.Mutate(x => x.Resize(maxWidth, targetHeight));
            }

            using var ms = new MemoryStream();
            image.Save(ms, new JpegEncoder
            {
                Quality = quality
            });
            return ms.ToArray();
        }
    }
}
