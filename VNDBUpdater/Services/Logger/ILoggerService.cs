using System;
using System.Runtime.CompilerServices;

namespace VNDBUpdater.Services.Logger
{
    public interface ILoggerService
    {
        void Log(string message, [CallerMemberName]string memberName = "");
        void Log(Exception ex, [CallerMemberName]string memberName = "");
    }
}
