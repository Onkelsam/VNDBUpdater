﻿using System.Collections.Generic;
using VNDBUpdater.Models;
using System.Linq;
using VNDBUpdater.Communication.Database;

namespace VNDBUpdater.Helper
{
    public static class VisualNovelHelper
    {
        private static List<VisualNovel> _LocalVisualNovels;

        public static List<VisualNovel> LocalVisualNovels
        {
            get
            {
                if (_LocalVisualNovels == null || !_LocalVisualNovels.Any())
                {
                    _LocalVisualNovels = new List<VisualNovel>();
                    _LocalVisualNovels.AddRange(RedisCommunication.GetVisualNovelsFromDB());
                }

                return _LocalVisualNovels;
            }
        }

        public static bool VisualNovelExists(int ID)
        {
            return LocalVisualNovels.Any(x => x.Basics.id == ID);

        }

        public static VisualNovel GetVisualNovel(int ID)
        {
            if (VisualNovelExists(ID))
                return LocalVisualNovels.First(x => x.Basics.id == ID);
            else
                return null;
        }

        public static void ResetVisualNovels()
        {
            _LocalVisualNovels = null;
        }
    }
}
