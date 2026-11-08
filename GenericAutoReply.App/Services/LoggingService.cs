using Serilog;
using Serilog.Events;
using platformAutoReply.App.Models;

namespace platformAutoReply.App.Services;

public interface ILoggingService
{
    void Information(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Error(string message, params object[] args);
    void Error(Exception ex, string message, params object[] args);
    void Debug(string message, params object[] args);
}

public class LoggingService : ILoggingService
{
    private readonly ILogger _logger;

    public LoggingService(AppConfig config)
    {
        var loggerConfig = new LoggerConfiguration();

        if (config.Logging.LogToConsole)
        {
            loggerConfig.WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        if (config.Logging.LogToFile)
        {
            var logDir = config.Logging.LogDirectory;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            var logPath = Path.Combine(logDir, $"{config.Logging.LogFilePrefix}-.log");
            loggerConfig.WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: config.Logging.RetainedFileCountLimit,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        loggerConfig.MinimumLevel.Debug();
        loggerConfig.MinimumLevel.Override("Microsoft", LogEventLevel.Information);

        _logger = loggerConfig.CreateLogger();
    }

    public void Information(string message, params object[] args)
    {
        _logger.Information(message, args);
    }

    public void Warning(string message, params object[] args)
    {
        _logger.Warning(message, args);
    }

    public void Error(string message, params object[] args)
    {
        _logger.Error(message, args);
    }

    public void Error(Exception ex, string message, params object[] args)
    {
        _logger.Error(ex, message, args);
    }

    public void Debug(string message, params object[] args)
    {
        _logger.Debug(message, args);
    }

    public void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
