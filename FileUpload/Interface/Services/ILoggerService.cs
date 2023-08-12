namespace FileUpload.Interface.Services
{
    public interface ILoggerService
    {
        void LogDebug(string message);
        void LogError(string message);
        void LogInfo(string message);
        void LogTrace(string message);
        void LogWarn(string message);
        void LogException(Exception ex, string message);
    }
}