using FileUpload.Interface.Services;
using NLog;
using NLog.Fluent;
using ILogger = NLog.ILogger;

namespace FileUpload.Services
{
    public class LoggerService : ILoggerService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public void LogTrace(string message) => logger.Trace(message);
        public void LogDebug(string message) => logger.Debug(message);
        public void LogError(string message) => logger.Error(message);
        public void LogInfo(string message) => logger.Info(message);
        public void LogWarn(string message) => logger.Warn(message);
        public void LogException(Exception ex, string message) => logger.Error(ex, message);
    }
}
