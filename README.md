# 通用教育平台自动回复程序

一款适用于 Windows 系统的通用自动化回复程序，支持自动监测和回复在线教育平台上的用户问题。

## 功能特性

- **自动登录**: 支持通用教育平台账号自动登录
- **问题监测**: 自动监测指定课程或讨论区的新问题
- **智能匹配**: 基于本地知识库自动匹配最佳回复
- **自动回复**: 自动提交回复至目标平台
- **灵活配置**: 支持配置回复范围、时间区间等参数
- **日志记录**: 完整的操作日志记录，便于问题排查

## 技术栈

- **开发语言**: C#
- **框架**: .NET 6.0
- **自动化**: Selenium WebDriver
- **日志**: Serilog
- **配置**: Microsoft.Extensions.Configuration

## 环境要求

- Windows 10/11
- .NET 6.0 SDK 或更高版本
- Google Chrome 浏览器
- ChromeDriver (通过NuGet自动安装)

## 快速开始

### 1. 配置账号信息

编辑 `platformAutoReply.App/appsettings.json` 文件：

```json
{
  "platform": {
    "Username": "your_username",
    "Password": "your_password"
  }
}
```

将 `your_username` 和 `your_password` 替换为您的通用教育平台账号和密码。

### 2. 配置监控范围

在 `appsettings.json` 中配置需要监控的课程ID：

```json
{
  "Monitor": {
    "CourseIds": ["course_id_1", "course_id_2"],
    "CheckInterval": "00:05:00"
  }
}
```

### 3. 配置知识库

编辑 `platformAutoReply.App/Data/knowledge-base.json` 文件，添加常见问题和对应的回复：

```json
{
  "Entries": [
    {
      "Keywords": ["关键词1", "关键词2"],
      "QuestionPattern": "问题示例",
      "Answer": "对应的回复内容",
      "Priority": 1
    }
  ]
}
```

### 4. 构建和运行

```bash
# 还原依赖
dotnet restore

# 构建项目
dotnet build

# 运行程序
cd platformAutoReply.App
dotnet run
```

## 配置说明

### platform 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| Username | 通用教育平台账号 | - |
| Password | 通用教育平台密码 | - |
| LoginUrl | 登录页面URL | https://passport2.chaoxing.com/login |
| PageLoadTimeout | 页面加载超时时间(毫秒) | 30000 |
| HeadlessMode | 无头模式(不显示浏览器) | false |

### Monitor 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| CourseIds | 需要监控的课程ID列表 | [] |
| DiscussionAreaIds | 需要监控的讨论区ID列表 | [] |
| CheckInterval | 检查间隔 | 00:05:00 (5分钟) |
| StartTime | 监控开始时间 | null |
| EndTime | 监控结束时间 | null |
| MaxReplyPerSession | 每次会话最大回复数 | 50 |

### KnowledgeBase 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| DataFilePath | 知识库文件路径 | Data/knowledge-base.json |
| MatchThreshold | 匹配阈值(0-1) | 0.7 |

### Logging 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| LogDirectory | 日志目录 | Logs |
| LogFilePrefix | 日志文件名前缀 | platform-auto-reply |
| LogToConsole | 是否输出到控制台 | true |
| LogToFile | 是否输出到文件 | true |
| RetainedFileCountLimit | 保留日志文件数量 | 30 |

## 知识库条目说明

知识库条目包含以下字段：

- **Keywords**: 关键词列表，用于快速匹配问题
- **QuestionPattern**: 问题模式，用于相似度计算
- **Answer**: 对应的回复内容
- **Priority**: 优先级，数值越大优先级越高
- **CreatedAt**: 创建时间
- **UpdatedAt**: 更新时间

## 注意事项

1. **账号安全**: 请妥善保管配置文件中的账号密码，不要将配置文件提交到公开仓库
2. **合理间隔**: 请设置合理的检查间隔，避免对通用教育平台服务器造成过大压力
3. **手动验证**: 首次使用建议在非无头模式下运行，观察浏览器操作是否正常
4. **页面结构**: 通用教育平台页面结构可能变化，如程序无法正常工作可能需要更新选择器

## 项目结构

```
platformAutoReply/
├── platformAutoReply.sln
├── platformAutoReply.App/
│   ├── Models/              # 数据模型
│   │   ├── AppConfig.cs
│   │   └── Question.cs
│   ├── Services/            # 业务服务
│   │   ├── LoggingService.cs
│   │   ├── KnowledgeBaseService.cs
│   │   ├── PlatformAutomationService.cs
│   │   └── MonitorService.cs
│   ├── Data/                # 数据文件
│   │   └── knowledge-base.json
│   ├── appsettings.json     # 配置文件
│   ├── Program.cs           # 程序入口
│   └── platformAutoReply.App.csproj
└── README.md
```

## 许可证

本项目仅供学习交流使用，请勿用于商业用途。

## 免责声明

使用本程序所产生的一切后果由使用者自行承担，请合理使用自动化工具，遵守通用教育平台平台的使用规则。
