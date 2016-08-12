using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;
using VNDBUpdater.Models.Internal;

namespace VNDBUpdater.Models
{
    public class VisualNovel : INotifyPropertyChanged
    {
        private string _ExePath;
        private int _Score;
        private VisualNovelCatergory _Category;

        public event PropertyChangedEventHandler PropertyChanged;

        public BasicInformation Basics { get; set; }
        public List<CharacterInformation> Characters { get; set; }
        public string FolderPath { get; private set; }
        public bool IsFilteredOut { get; set; }
        public TimeSpan Playtime { get; set; }

        public VisualNovel()
        {
            Characters = new List<CharacterInformation>();
        }

        public bool AlreadyExistsInDatabase
        {
            get { return LocalVisualNovelHelper.VisualNovelExists(Basics.VNDBInformation.id); }
        }

        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        public double ScoreInDouble
        {
            get { return Convert.ToDouble(_Score) / 10; }
        }

        public VisualNovelCatergory Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        
        public string ExePath
        {
            get { return _ExePath; }
            set
            {
                _ExePath = value;

                if (_ExePath != null)
                {
                    if (Path.GetExtension(Path.GetFileName(_ExePath)).ToLower() == ".exe")
                        FolderPath = !string.IsNullOrEmpty(_ExePath) ? _ExePath.Replace(Path.GetFileName(_ExePath), "") : string.Empty;
                    else
                        FolderPath = _ExePath;
                }                
            }
        }

        public bool InstallationPathExists
        {
            get
            {
                if (!string.IsNullOrEmpty(_ExePath))
                    return File.Exists(_ExePath);
                else
                    return false;
            }
        }

        public bool WalkthroughAvailable
        {
            get
            {
                if (!string.IsNullOrEmpty(FolderPath))
                    return File.Exists(FolderPath + Constants.WalkthroughFileName);
                else
                    return false;
            }
        }

        public void StartGame()
        {
            if (InstallationPathExists)
            {
                var proc = new Process();

                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(ProcessExited);
                proc.StartInfo.FileName = _ExePath;

                proc.Start();
            }           
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            Playtime += DateTime.Now - (sender as Process).StartTime;
            LocalVisualNovelHelper.AddVisualNovel(this);
            OnPropertyChanged(nameof(Playtime));
        }

        public void OpenGameFolder()
        {
            if (InstallationPathExists)
                Process.Start(FolderPath);
        }

        public void ViewOnVNDB()
        {
            Process.Start("https://vndb.org/v" + Basics.VNDBInformation.id);
        }
        public void SearchOnGoolge(string parameter)
        {
            if (Basics.VNDBInformation.original != null)
                Process.Start("https://www.google.de/#q=" + Basics.VNDBInformation.original.ToString() + "+" + parameter);
            else
                Process.Start("https://www.google.de/#q=" + Basics.VNDBInformation.title + "+" + parameter);         
        }

        public void OpenWalkthrough()
        {
            if (WalkthroughAvailable)
                Process.Start(FolderPath + Constants.WalkthroughFileName);
        }

        public void SetCategory(string category)
        {
            SetCategory(ExtensionMethods.ParseEnum<VisualNovelCatergory>(category));
        }

        /// <summary>
        /// Sets category locally and on VNDB.
        /// </summary>
        public void SetCategory(VisualNovelCatergory category)
        {
            Category = category;

            VNDBCommunication.SetVNList(this);

            LocalVisualNovelHelper.AddVisualNovel(this);
        }

        /// <summary>
        /// Sets score locally and on VNDB.
        /// </summary>
        public void SetScore(int score)
        {
            Score = score;

            if (Score == 0)
                VNDBCommunication.RemoveFromScoreList(this);
            else
                VNDBCommunication.SetVoteList(this);

            LocalVisualNovelHelper.AddVisualNovel(this);
        }

        public void Update()
        {
            VisualNovel updatedVisualNovel = VNDBCommunication.FetchVisualNovel(Basics.VNDBInformation.id);

            Basics = updatedVisualNovel.Basics;
            Characters = updatedVisualNovel.Characters;

            LocalVisualNovelHelper.AddVisualNovel(this);
        }

        public VNScreenshot GetNextScreenshot(VNScreenshot currentScreenshot)
        {
            if (UserHelper.CurrentUser.Settings.ShowNSFWImages)
            {
                if (!Basics.VNDBInformation.screens.Any() || currentScreenshot == Basics.VNDBInformation.screens.Last() || currentScreenshot == null)
                    return new VNScreenshot() { image = Basics.VNDBInformation.image };
                else
                    return Basics.VNDBInformation.screens.NextOf(currentScreenshot);
            }
            else
            {
                if (!Basics.VNDBInformation.screens.Any() || currentScreenshot == Basics.VNDBInformation.screens.Last() && !Basics.VNDBInformation.image_nsfw || currentScreenshot == null)
                    return new VNScreenshot() { image = Basics.VNDBInformation.image };
                else
                {
                    VNScreenshot screen = Basics.VNDBInformation.screens.NextOf(currentScreenshot);

                    if (screen.nsfw)
                        return GetNextScreenshot(screen);
                    else
                        return screen;
                }
            }            
        }

        public CharacterInformation NextCharacter(CharacterInformation currentCharacter)
        {
            if (Characters != null)
            {
                if (Characters.Any())
                {
                    if (!Characters.Any(x => x.VNDBInformation.image != null))
                        return new CharacterInformation(new VNCharacterInformation());
                    else
                        return Characters.NextOf(currentCharacter);
                }
            }

            return new CharacterInformation(new VNCharacterInformation());
        }

        public bool Delete()
        {
            var dialogResult = MessageBox.Show("Are you sure?", Basics.VNDBInformation.title, MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                LocalVisualNovelHelper.RemoveVisualNovel(this);
                VNDBCommunication.RemoveFromScoreList(this);
                VNDBCommunication.RemoveFromVNList(this);
                return true;
            }
            else
                return false;                
        }

        public void SetExePath()
        {
            string path = FileHelper.GetExePath();

            if (!string.IsNullOrEmpty(path))
                ExePath = path;

            LocalVisualNovelHelper.AddVisualNovel(this);
        }

        public void CreateWalkthrough()
        {
            if (InstallationPathExists && !WalkthroughAvailable)
            {
                File.Create(FolderPath + Constants.WalkthroughFileName);
                LocalVisualNovelHelper.AddVisualNovel(this);
            }                
        }

        public void CrawlExePath()
        {
            if (!InstallationPathExists && (UserHelper.CurrentUser.Settings.InstallFolderPath != null))
            {
                ExePath = FileHelper.SearchForVisualNovelExe(this);
                OnPropertyChanged(nameof(ExePath));
            }
        }

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
