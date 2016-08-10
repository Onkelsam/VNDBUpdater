using System.Collections.Generic;
using VNDBUpdater.Models;
using System.Linq;
using VNDBUpdater.Communication.Database;

namespace VNDBUpdater.Helper
{
    public static class LocalVisualNovelHelper
    {
        private static List<VisualNovel> _LocalVisualNovels;

        public static List<VisualNovel> LocalVisualNovels
        {
            get
            {
                if (_LocalVisualNovels == null)
                {
                    RefreshVisualNovels();
                }

                return _LocalVisualNovels;
            }
        }

        public static void RefreshVisualNovels()
        {
            _LocalVisualNovels = new List<VisualNovel>();
            _LocalVisualNovels.AddRange(RedisCommunication.GetVisualNovelsFromDB());
        }

        public static void AddVisualNovel(VisualNovel vn)
        {
            RemoveVisualNovel(vn);
            _LocalVisualNovels.Add(vn);

            RedisCommunication.AddVisualNovelToDB(vn);
        }

        public static void AddVisualNovels(List<VisualNovel> vns)
        {
            foreach (var vn in vns)
                AddVisualNovel(vn);
        }

        public static void RemoveVisualNovel(VisualNovel vn)
        {
            if (VisualNovelExists(vn.Basics.VNDBInformation.id))
                _LocalVisualNovels.Remove(_LocalVisualNovels.Where(x => x.Basics.VNDBInformation.id == vn.Basics.VNDBInformation.id).First());

            RedisCommunication.DeleteVisualNovel(vn.Basics.VNDBInformation.id);
        }

        public static bool VisualNovelExists(int ID)
        {
            return LocalVisualNovels.Any(x => x.Basics.VNDBInformation.id == ID);
        }

        public static VisualNovel GetVisualNovel(int ID)
        {
            if (VisualNovelExists(ID))
                return LocalVisualNovels.First(x => x.Basics.VNDBInformation.id == ID);
            else
                return null;
        }
    }
}
