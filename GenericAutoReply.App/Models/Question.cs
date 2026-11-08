namespace platformAutoReply.App.Models;

public class Question
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishTime { get; set; }
    public string CourseId { get; set; } = string.Empty;
    public string DiscussionAreaId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsReplied { get; set; }
}

public class KnowledgeEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<string> Keywords { get; set; } = new List<string>();
    public string QuestionPattern { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int Priority { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

public class KnowledgeBase
{
    public List<KnowledgeEntry> Entries { get; set; } = new List<KnowledgeEntry>();
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}
