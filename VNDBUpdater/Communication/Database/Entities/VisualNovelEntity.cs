using System;
using System.Collections.Generic;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class VisualNovelEntity
    {
        public VisualNovelEntity()
        {
            Basics = new BasicInformationEntity();
            Characters = new List<CharacterInformationEntity>();
        }

        public VisualNovelEntity(VisualNovelModel model)
        {
            Basics = new BasicInformationEntity(model.Basics);
            Characters = model.Characters.Select(x => new CharacterInformationEntity(x)).ToList();

            Playtime = model.PlayTime;
            Score = model.Score;
            Category = model.Category;
            ExePath = model.ExePath;
            FolderPath = model.FolderPath;
        }

        public BasicInformationEntity Basics { get; set; }
        public List<CharacterInformationEntity> Characters { get; set; }
        public TimeSpan Playtime { get; set; }
        public VisualNovelModel.VisualNovelCatergory Category { get; set; }
        public int Score { get; set; }        
        public string ExePath { get; set; }
        public string FolderPath { get; set; }
    }
}
