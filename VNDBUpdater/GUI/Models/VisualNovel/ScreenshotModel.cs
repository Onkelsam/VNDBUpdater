using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Communication.VNDB.Entities;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class ScreenshotModel
    {
        public ScreenshotModel() { }

        public ScreenshotModel(string path, bool nsfw, int height, int width)
        {
            Path = path;
            NSFW = nsfw;
            Height = height;
            Width = width;
        }

        public ScreenshotModel(ScreenshotEntity entity)
        {
            Path = entity.Path;
            NSFW = entity.NSFW;
            Height = entity.Height;
            Width = entity.Width;
        }

        public ScreenshotModel(VNScreenshot VNDBEntity)
        {
            Path = VNDBEntity.image;
            NSFW = VNDBEntity.nsfw;
            Height = VNDBEntity.height;
            Width = VNDBEntity.width;
        }

        public string Path
        {
            get;
            set;
        }

        public bool NSFW
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }
    }
}
