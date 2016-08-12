using System.Threading;
using System.Threading.Tasks;
using VNDBUpdater.ViewModels;

namespace VNDBUpdater.BackgroundTasks
{
    public abstract class BackgroundTask
    {
        protected static CancellationToken _CancelToken;
        protected static CancellationTokenSource _CancelTokenSource;

        protected static MainViewModel _MainScreen;

        protected Task _BackgroundTask;

        public virtual void Start(MainViewModel MainScreen)
        {
            _MainScreen = MainScreen;

            _CancelTokenSource = new CancellationTokenSource();
            _CancelToken = _CancelTokenSource.Token;            
        }
    }
}
