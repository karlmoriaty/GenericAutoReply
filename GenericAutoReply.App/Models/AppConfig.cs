namespace platformAutoReply.App.Models;

public class AppConfig
{
    public platformConfig platform { get; set; } = new platformConfig();
    public MonitorConfig Monitor { get; set; } = new MonitorConfig();
    public KnowledgeBaseConfig KnowledgeBase { get; set; } = new KnowledgeBaseConfig();
    public LoggingConfig Logging { get; set; } = new LoggingConfig();
}

public class platformConfig
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string LoginUrl { get; set; } = "https://passport2.chaoxing.com/login";
    public int PageLoadTimeout { get; set; } = 30000;
    public bool HeadlessMode { get; set; } = false;
    public bool UseExistingBrowser { get; set; } = false;
    public int DebuggingPort { get; set; } = 9222;
}

public class MonitorConfig
{
    public List<string> CourseIds { get; set; } = new List<string>();
    public List<string> DiscussionAreaIds { get; set; } = new List<string>();
    public string DiscussionAreaUrl { get; set; } = string.Empty;
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(5);
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int MaxReplyPerSession { get; set; } = 50;
}

public class KnowledgeBaseConfig
{
    public string DataFilePath { get; set; } = "Data/knowledge-base.json";
    public double MatchThreshold { get; set; } = 0.7;
}

public class LoggingConfig
{
    public string LogDirectory { get; set; } = "Logs";
    public string LogFilePrefix { get; set; } = "platform-auto-reply";
    public bool LogToConsole { get; set; } = true;
    public bool LogToFile { get; set; } = true;
    public int RetainedFileCountLimit { get; set; } = 30;
}

