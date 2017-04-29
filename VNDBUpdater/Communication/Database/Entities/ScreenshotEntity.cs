using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class ScreenshotEntity
    {
        public string Path { get; set; }
        public bool NSFW { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public ScreenshotEntity()
        {
        }

        public ScreenshotEntity(ScreenshotModel model)
        {
            Path = model.Path;
            NSFW = model.NSFW;
            Height = model.Height;
            Width = model.Width;
        }
    }
}
