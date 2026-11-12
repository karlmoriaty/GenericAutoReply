# Generic Auto Reply

A Windows automation tool for online education platforms: it watches course
discussion areas for new questions, matches them against a local knowledge base, and
posts the best answer automatically.

## Features

- **Automatic login** to the target platform
- **Question monitoring** across configured courses and discussion areas
- **Local matching** against a JSON knowledge base, with a similarity threshold
- **Automatic replies**, capped per session
- **Scheduling window** — only run between a start and end time
- **Structured logs** to console and rotating files

## Stack

C# / .NET 6, Selenium WebDriver (Chrome), Serilog, Microsoft.Extensions.Configuration.

## Requirements

- Windows 10/11
- .NET 6 SDK or newer
- Google Chrome
- ChromeDriver (pulled in through NuGet)

## Setup

### 1. Account

`platformAutoReply.App/appsettings.json`:

```json
{
  "platform": {
    "Username": "your_username",
    "Password": "your_password"
  }
}
```

### 2. What to monitor

```json
{
  "Monitor": {
    "CourseIds": ["course_id_1", "course_id_2"],
    "CheckInterval": "00:05:00"
  }
}
```

### 3. Knowledge base

`platformAutoReply.App/Data/knowledge-base.json`:

```json
{
  "Entries": [
    {
      "Keywords": ["keyword1", "keyword2"],
      "QuestionPattern": "example question",
      "Answer": "the reply to post",
      "Priority": 1
    }
  ]
}
```

### 4. Build and run

```bash
dotnet restore
dotnet build
cd platformAutoReply.App
dotnet run
```

## Configuration

**platform** — `Username`, `Password`, `LoginUrl`
(default `https://passport2.chaoxing.com/login`), `PageLoadTimeout` (ms, default
30000), `HeadlessMode` (default `false`).

**Monitor** — `CourseIds`, `DiscussionAreaIds`, `CheckInterval` (default
`00:05:00`), `StartTime`, `EndTime`, `MaxReplyPerSession` (default 50).

**KnowledgeBase** — `DataFilePath` (default `Data/knowledge-base.json`),
`MatchThreshold` (0–1, default 0.7).

**Logging** — `LogDirectory` (`Logs`), `LogFilePrefix`
(`platform-auto-reply`), `LogToConsole`, `LogToFile`,
`RetainedFileCountLimit` (default 30).

Knowledge-base entries carry `Keywords`, `QuestionPattern`, `Answer`, `Priority`
(higher wins), `CreatedAt` and `UpdatedAt`.

## Caveats

- **Credentials** live in `appsettings.json` — keep that file out of public
  repositories.
- Use a sane `CheckInterval`; this drives a real browser against the platform.
- Run without headless mode the first time so you can watch what it does.
- The platform's DOM changes break selectors; expect to update them.
- Intended for personal, educational use only. You are responsible for complying
  with the platform's rules.

## Layout

```
platformAutoReply/
├── platformAutoReply.sln
└── platformAutoReply.App/
    ├── Models/AppConfig.cs, Question.cs
    ├── Services/LoggingService.cs, KnowledgeBaseService.cs,
    │            PlatformAutomationService.cs, MonitorService.cs
    ├── Data/knowledge-base.json
    ├── appsettings.json
    └── Program.cs
```

## License

Provided for learning and personal use; commercial use is not permitted. Use at your
own risk.
