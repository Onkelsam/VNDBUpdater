using System;
using VNDBUpdater.BackgroundTasks.Interfaces;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Status;

namespace VNDBUpdater.BackgroundTasks
{
    public abstract class TaskBase : ITask
    {
        protected IStatusService _StatusService;
        protected ILoggerService _Logger;

        protected int _TasksToDo;
        protected int _TasksDone;

        public TaskBase(IStatusService StatusService, ILoggerService LoggerService)
        {
            _StatusService = StatusService;
            _Logger = LoggerService;

            _TasksToDo = 0;
            _TasksDone = 0;
        }

        protected bool IsRunning
        {
            set { _StatusService.TaskIsRunning = value; }
        }

        protected int PercentageCompleted
        {
            set { _StatusService.PercentageOfTaskCompleted = value; }
        }

        protected string CurrentStatus
        {
            set { _StatusService.CurrentMessage = value; }
        }

        protected string CurrentTask
        {
            set { _StatusService.CurrentTask = value; }
        }

        public abstract void Start(Action<bool> OnTaskCompleted);

        protected void UpdateProgess(int amountdone, string message)
        {
            _TasksDone += amountdone;

            CurrentStatus = _TasksDone + " of " + _TasksToDo + " " + message;
            PercentageCompleted = (int)Math.Round((double)(100 * _TasksDone) / _TasksToDo);
        }

        protected static T[] Take<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];

            Array.Copy(data, index, result, 0, length);

            return result;
        }
    }
}
