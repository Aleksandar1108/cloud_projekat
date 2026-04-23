namespace SmartGrid.Application.Interfaces
{
    public interface IImageOptimizationService
    {
        byte[] Optimize(byte[] imageContent, int maxWidth = 1280, int quality = 75);
    }
}
