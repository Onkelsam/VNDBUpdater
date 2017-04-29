using System;
using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.Communication.Database.Entities;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class VisualNovelModel
    {
        public VisualNovelModel()
        {
            Characters = new List<CharacterInformationModel>();
        }

        public VisualNovelModel(VisualNovelEntity entity)
        {
            Basics = new BasicInformationModel(entity.Basics);
            Characters = entity.Characters.Select(x => new CharacterInformationModel(x)).ToList();

            PlayTime = entity.Playtime;
            Score = entity.Score;
            Category = entity.Category;
            ExePath = entity.ExePath;
            FolderPath = entity.FolderPath;
        }

        private BasicInformationModel _Basics;

        public BasicInformationModel Basics
        {
            get { return _Basics; }
            set { _Basics = value; }
        }

        private List<CharacterInformationModel> _Characters;

        public List<CharacterInformationModel> Characters
        {
            get { return _Characters; }
            set { _Characters = value; }
        }

        private string _FolderPath;

        public string FolderPath
        {
            get { return _FolderPath; }
            set { _FolderPath = value; }
        }

        private TimeSpan _PlayTime;

        public TimeSpan PlayTime
        {
            get { return _PlayTime; }
            set { _PlayTime = value; }
        }

        private int _Score;

        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        public double ScoreInDouble
        {
            get { return Convert.ToDouble(_Score) / 10; }
        }

        private VisualNovelCatergory _Category;

        public VisualNovelCatergory Category
        {
            get { return _Category; }
            set { _Category = value; }
        }

        public enum VisualNovelCatergory
        {
            Unknown = 0,
            Playing,
            Finished,
            Stalled,
            Dropped,
        };

        private string _ExePath;

        public string ExePath
        {
            get { return _ExePath; }
            set { _ExePath = value; }
        }
    }
}
