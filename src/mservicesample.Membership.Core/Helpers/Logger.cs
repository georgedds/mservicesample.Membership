using Serilog;

namespace mservicesample.Membership.Core.Helpers
{
    public class Logger : ILogger
    {
        public void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public void LogError(string message)
        {
            Log.Error(message);
        }

        public void LogInfo(string message)
        {
            Log.Information(message);
        }

        public void LogWarn(string message)
        {
            Log.Warning(message);
        }
    }
}
