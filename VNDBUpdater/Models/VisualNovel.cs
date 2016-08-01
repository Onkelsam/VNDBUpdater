﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Data;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models
{
    public class VisualNovel : INotifyPropertyChanged
    {
        private string _ExePath;
        private int _Score;
        private VisualNovelCatergory _Category;

        private const string walkthroughfilename = "walkthrough.txt";

        public event PropertyChangedEventHandler PropertyChanged;

        public VNInformation Basics { get; set; }
        public List<VNCharacterInformation> Characters { get; set; }
        public string FolderPath { get; private set; }

        public bool AlreadyExistsInDatabase
        {
            get { return RedisCommunication.VisualNovelExistsInDatabase(Basics.id); }
        }

        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
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
                    if (_ExePath.Contains(".exe"))
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
                    return File.Exists(FolderPath + walkthroughfilename);
                else
                    return false;
            }
        }

        public void StartGame()
        {
            if (InstallationPathExists)
                Process.Start(_ExePath);
        }

        public void OpenGameFolder()
        {
            if (InstallationPathExists)
                Process.Start(FolderPath);
        }

        public void ViewOnVNDB()
        {
            Process.Start("https://vndb.org/v" + Basics.id);
        }
        public void SearchOnGoolge(string parameter)
        {
            if (Basics.original != null)
                Process.Start("https://www.google.de/#q=" + Basics.original.ToString() + "+" + parameter);
            else
                Process.Start("https://www.google.de/#q=" + Basics.title + "+" + parameter);         
        }

        public void OpenWalkthrough()
        {
            if (WalkthroughAvailable)
                Process.Start(FolderPath + walkthroughfilename);
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

            if (Category == VisualNovelCatergory.Wish)
                VNDBCommunication.SetWishList(this);
            else
                VNDBCommunication.SetVNList(this);
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
        }

        public void Update()
        {
            VisualNovel updatedVisualNovel = VNDBCommunication.FetchVisualNovel(Basics.id);

            Basics = updatedVisualNovel.Basics;
            Characters = updatedVisualNovel.Characters;

            RedisCommunication.AddVisualNovelToDB(this);
        }

        public string NextScreenshot(string currentScreenshot)
        {
            if (!Basics.screens.Any() || currentScreenshot == Basics.screens.Last().image)
                return Basics.image;
            else if (currentScreenshot == Basics.image)
                return Basics.screens[0].image;
            else
                return Basics.screens[Basics.screens.IndexOf(Basics.screens.Where(x => x.image == currentScreenshot).Select(x => x).First()) + 1].image;
        }

        public string NextCharacter(string currentCharacter)
        {
            if (Characters != null)
            {
                if (Characters.Any())
                {
                    if (!Characters.Any(x => x.image != null))
                        return Basics.image;
                    else if (currentCharacter == Characters.Last().image)
                        return Characters[0].image;
                    else
                        return Characters[Characters.IndexOf(Characters.Where(x => x.image == currentCharacter).Select(x => x).First()) + 1].image;
                }
                else
                    return Basics.image;
            }
            else
                return Basics.image;
        }

        public bool Delete()
        {
            var dialogResult = MessageBox.Show("Are you sure?", Basics.title, MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                RedisCommunication.DeleteVisualNovel(Basics.id);
                VNDBCommunication.RemoveFromScoreList(this);
                VNDBCommunication.RemoveFromWishList(this);
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
        }

        public void CreateWalkthrough()
        {
            if (InstallationPathExists && !WalkthroughAvailable)
            {
                File.Create(FolderPath + walkthroughfilename);
                OpenWalkthrough();
            }                
        }

        public void CrawlExePath()
        {
            if (!InstallationPathExists && (RedisCommunication.GetInstallFolder() != null))
            {
                ExePath = FileHelper.SearchForVisualNovelExe(this);
                RedisCommunication.AddVisualNovelToDB(this);
            }
        }
    }
}
