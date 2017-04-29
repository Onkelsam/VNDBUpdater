using System.Windows.Forms;

namespace VNDBUpdater.Services.Dialogs
{
    public class DialogService : IDialogService
    {
        public string GetPathToExecuteable()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Exe File |*.exe",
                FilterIndex = 1,
                Multiselect = false,
                DefaultExt = "exe"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName.EndsWith(".exe") ? fileDialog.FileName : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetPathToFolder()
        {
            var fileDialog = new FolderBrowserDialog()
            {
                Description = "Select Folder",                
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.SelectedPath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
