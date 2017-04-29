using CodeKicker.BBCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.GUI.ViewModels.Interfaces;

namespace VNDBUpdater.GUI.ViewModels.MainView
{
    public class VNInfoViewModel : ViewModelBase, IVisualNovelInfoWindowModel
    {
        private IVisualNovelsGridWindowModel _VisualNovelsGrid;

        private static readonly Dictionary<int?, string> VNLengthMapper = new Dictionary<int?, string>()
        {
            { 0, "Unknown" },
            { 1, "Very short (< 2 hours)" },
            { 2, "Short (2 - 10 hours)" },
            { 3, "Medium (10 - 30 hours)" },
            { 4, "Long (30 - 50 hours)" },
            { 5, "Very long (> 50 hours)" }
        };

        private static readonly Dictionary<string, string> RelationsMapper = new Dictionary<string, string>()
        {
            { "seq", "Sequel" },
            { "set", "Same Setting" },
            { "preq", "Prequel" },
            { "fan", "Fandisc" },
            { "ser", "Same Series" },
            { "orig", "Original Game" },
            { "alt", "Alternative Version" },
            { "char", "Shares Characters" },
            { "side", "Side Story" },
            { "par", "Parent Story" }
        };

        public VNInfoViewModel(IVisualNovelsGridWindowModel VisualNovelsGrid)
            : base()
        {
            _VisualNovelsGrid = VisualNovelsGrid;
            _VisualNovelsGrid.PropertyChanged += OnSelectedVisualNovelPropertyChanged;
        }

        public string Name
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? _VisualNovelsGrid.SelectedVisualNovel.Basics.Title : "Visual Novel Name"; }
        }

        public string Aliases
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? _VisualNovelsGrid.SelectedVisualNovel.Basics.Aliases : string.Empty; }
        }

        public string Length
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics?.Length != null ? VNLengthMapper[_VisualNovelsGrid.SelectedVisualNovel.Basics.Length] : VNLengthMapper[0] ; }
        }

        public string Release
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? _VisualNovelsGrid.SelectedVisualNovel.Basics.Release.ToString("yyyy-MM-dd") : string.Empty; }
        }

        public Dictionary<string, List<RelationModel>> Relations
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? ConvertRelations(_VisualNovelsGrid.SelectedVisualNovel.Basics.Relations) : new Dictionary<string, List<RelationModel>>(); }
        }

        public string PlayTime
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel != null ? ConvertTimeSpan(_VisualNovelsGrid.SelectedVisualNovel.PlayTime) : string.Empty; }
        }

        public string Score
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? _VisualNovelsGrid.SelectedVisualNovel.Basics.Rating.ToString() : string.Empty; }
        }

        public string Popularity
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? _VisualNovelsGrid.SelectedVisualNovel.Basics.Popularity.ToString() : string.Empty; }
        }

        public string OwnScore
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics != null ? _VisualNovelsGrid.SelectedVisualNovel.ScoreInDouble.ToString() : string.Empty; }
        }

        public string Description
        {
            get { return _VisualNovelsGrid.SelectedVisualNovel?.Basics?.Description != null ? BBCode.ToHtml(_VisualNovelsGrid.SelectedVisualNovel.Basics.Description) : string.Empty; }
        }

        private string ConvertTimeSpan(TimeSpan ts)
        {
            if (ts == null)
            {
                return string.Empty;
            }
            else
            {
                return string.Format("{0} day{1}, {2} hour{3}, {4} minute{5}",
                                      ts.Days,
                                      ts.Days == 1 ? "" : "s",
                                      ts.Hours,
                                      ts.Hours == 1 ? "" : "s",
                                      ts.Minutes,
                                      ts.Minutes == 1 ? "" : "s");
            }
        }

        private Dictionary<string, List<RelationModel>> ConvertRelations(List<RelationModel> relations)
        {
            var relationDictionary = new Dictionary<string, List<RelationModel>>();

            if (relations != null)
            {
                foreach (var relation in RelationsMapper)
                {
                    if (relations.Any(x => x.Relation == relation.Key))
                    {
                        relationDictionary.Add(relation.Value + ":", relations.Where(x => x.Relation == relation.Key).ToList());
                    }
                }
            }

            return relationDictionary;
        }

        private void OnSelectedVisualNovelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_VisualNovelsGrid.SelectedVisualNovel))
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Aliases));
                OnPropertyChanged(nameof(Length));
                OnPropertyChanged(nameof(Release));
                OnPropertyChanged(nameof(PlayTime));
                OnPropertyChanged(nameof(Relations));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(Score));
                OnPropertyChanged(nameof(Popularity));
                OnPropertyChanged(nameof(OwnScore));
            }
        }
    }
}
