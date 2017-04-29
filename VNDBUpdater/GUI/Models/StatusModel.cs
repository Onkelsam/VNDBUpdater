namespace VNDBUpdater.GUI.Models
{
    public class StatusModel
    {
        public StatusModel()
        {
            TaskName = string.Empty;
            CurrentUser = string.Empty;
            Message = string.Empty;
            ErrorMessage = string.Empty;
            TaskIsRunning = false;
            PercentageTaskCompleted = 0;
        }

        public string TaskName
        {
            get;
            set;
        }

        public string CurrentUser
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public bool TaskIsRunning
        {
            get;
            set;
        }

        public int PercentageTaskCompleted
        {
            get;
            set;
        }        
    }
}
