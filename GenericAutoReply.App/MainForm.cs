using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using platformAutoReply.App.Models;
using platformAutoReply.App.Services;

namespace platformAutoReply.App;

public partial class MainForm : Form
{
    private readonly AppConfig _config;
    private readonly ILoggingService _logger;
    private readonly IPlatformAutomationService _automationService;
    private readonly IKnowledgeBaseService _knowledgeBaseService;
    private List<Question> _questions = new();
    private int _currentQuestionIndex = 0;

    private Button? btnStart;
    private Button? btnStop;
    private Button? btnPrev;
    private Button? btnNext;
    private Button? btnSend;
    private Button? btnSkip;
    private TextBox? txtLog;
    private TextBox? txtQuestion;
    private TextBox? txtAnswer;
    private Label? lblQuestionInfo;
    private Label? lblProgress;

    public MainForm(AppConfig config, ILoggingService logger, 
        IPlatformAutomationService automationService, 
        IKnowledgeBaseService knowledgeBaseService)
    {
        _config = config;
        _logger = logger;
        _automationService = automationService;
        _knowledgeBaseService = knowledgeBaseService;

        InitializeComponent();
        Log("程序已启动，请点击'开始'按钮");
    }

    private void InitializeComponent()
    {
        this.Text = "通用教育平台自动回复程序";
        this.Size = new Size(900, 700);
        this.StartPosition = FormStartPosition.CenterScreen;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 60 };
        var panelMiddle = new Panel { Dock = DockStyle.Fill };
        var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 60 };

        btnStart = new Button { Text = "开始", Location = new Point(10, 10), Size = new Size(100, 40) };
        btnStart.Click += BtnStart_Click;

        btnStop = new Button { Text = "停止", Location = new Point(120, 10), Size = new Size(100, 40), Enabled = false };
        btnStop.Click += BtnStop_Click;

        lblProgress = new Label { Text = "进度: 0/0", Location = new Point(230, 20), Size = new Size(200, 20) };

        panelTop.Controls.AddRange(new Control[] { btnStart, btnStop, lblProgress });

        var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 400 };

        var panelQuestion = new Panel { Dock = DockStyle.Fill };
        var lblQuestionTitle = new Label { Text = "问题内容", Location = new Point(10, 10), Size = new Size(100, 20) };
        txtQuestion = new TextBox { Location = new Point(10, 35), Size = new Size(380, 250), Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };
        lblQuestionInfo = new Label { Text = "", Location = new Point(10, 290), Size = new Size(380, 20) };

        panelQuestion.Controls.AddRange(new Control[] { lblQuestionTitle, txtQuestion, lblQuestionInfo });

        var panelAnswer = new Panel { Dock = DockStyle.Fill };
        var lblAnswerTitle = new Label { Text = "推荐答案", Location = new Point(10, 10), Size = new Size(100, 20) };
        txtAnswer = new TextBox { Location = new Point(10, 35), Size = new Size(380, 200), Multiline = true, ScrollBars = ScrollBars.Vertical };

        btnPrev = new Button { Text = "上一个", Location = new Point(10, 240), Size = new Size(100, 35), Enabled = false };
        btnPrev.Click += BtnPrev_Click;

        btnNext = new Button { Text = "下一个", Location = new Point(120, 240), Size = new Size(100, 35), Enabled = false };
        btnNext.Click += BtnNext_Click;

        btnSend = new Button { Text = "发送回复", Location = new Point(10, 280), Size = new Size(100, 35), Enabled = false };
        btnSend.Click += BtnSend_Click;

        btnSkip = new Button { Text = "跳过", Location = new Point(120, 280), Size = new Size(100, 35), Enabled = false };
        btnSkip.Click += BtnSkip_Click;

        panelAnswer.Controls.AddRange(new Control[] { lblAnswerTitle, txtAnswer, btnPrev, btnNext, btnSend, btnSkip });

        splitContainer.Panel1.Controls.Add(panelQuestion);
        splitContainer.Panel2.Controls.Add(panelAnswer);
        panelMiddle.Controls.Add(splitContainer);

        var lblLogTitle = new Label { Text = "运行日志", Location = new Point(10, 10), Size = new Size(100, 20) };
        txtLog = new TextBox { Location = new Point(10, 30), Size = new Size(860, 200), Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };
        panelBottom.Controls.AddRange(new Control[] { lblLogTitle, txtLog });

        this.Controls.AddRange(new Control[] { panelBottom, panelMiddle, panelTop });
    }

    private void Log(string message)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => Log(message)));
            return;
        }

        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        txtLog.AppendText(logMessage + Environment.NewLine);
        txtLog.ScrollToCaret();
        _logger.Information(message);
    }

    private async void BtnStart_Click(object? sender, EventArgs e)
    {
        if (btnStart == null || btnStop == null || btnPrev == null || btnNext == null || btnSend == null || btnSkip == null || lblProgress == null)
            return;

        btnStart.Enabled = false;
        btnStop.Enabled = true;
        Log("正在初始化浏览器...");

        try
        {
            await _automationService.InitializeBrowserAsync();
            Log("浏览器初始化完成");

            Log("正在登录...");
            var loginSuccess = await _automationService.LoginAsync();
            
            if (!loginSuccess)
            {
                Log("登录失败，请检查");
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                return;
            }

            Log("登录成功，正在获取问题列表...");
            var url = _config.Monitor.DiscussionAreaUrl;
            _questions = await _automationService.FetchQuestionsFromUrlAsync(url);
            
            Log($"成功获取 {_questions.Count} 个问题");
            _currentQuestionIndex = 0;
            
            if (_questions.Count > 0)
            {
                await ShowCurrentQuestionAsync();
                btnPrev.Enabled = _currentQuestionIndex > 0;
                btnNext.Enabled = _currentQuestionIndex < _questions.Count - 1;
                btnSend.Enabled = true;
                btnSkip.Enabled = true;
            }
            else
            {
                Log("没有找到问题");
            }

            UpdateProgress();
        }
        catch (Exception ex)
        {
            Log($"发生错误: {ex.Message}");
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }

    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (btnStart == null || btnStop == null || btnPrev == null || btnNext == null || btnSend == null || btnSkip == null)
            return;

        Log("正在停止...");
        _automationService.Dispose();
        btnStart.Enabled = true;
        btnStop.Enabled = false;
        btnPrev.Enabled = false;
        btnNext.Enabled = false;
        btnSend.Enabled = false;
        btnSkip.Enabled = false;
        Log("已停止");
    }

    private async void BtnPrev_Click(object? sender, EventArgs e)
    {
        if (btnPrev == null || btnNext == null)
            return;

        if (_currentQuestionIndex > 0)
        {
            _currentQuestionIndex--;
            await ShowCurrentQuestionAsync();
            UpdateProgress();
            btnPrev.Enabled = _currentQuestionIndex > 0;
            btnNext.Enabled = _currentQuestionIndex < _questions.Count - 1;
        }
    }

    private async void BtnNext_Click(object? sender, EventArgs e)
    {
        if (btnPrev == null || btnNext == null)
            return;

        if (_currentQuestionIndex < _questions.Count - 1)
        {
            _currentQuestionIndex++;
            await ShowCurrentQuestionAsync();
            UpdateProgress();
            btnPrev.Enabled = _currentQuestionIndex > 0;
            btnNext.Enabled = _currentQuestionIndex < _questions.Count - 1;
        }
    }

    private async void BtnSend_Click(object? sender, EventArgs e)
    {
        if (btnPrev == null || btnNext == null || txtAnswer == null)
            return;

        if (_currentQuestionIndex >= _questions.Count) return;

        var question = _questions[_currentQuestionIndex];
        var answer = txtAnswer.Text;

        if (string.IsNullOrWhiteSpace(answer))
        {
            Log("请输入答案");
            return;
        }

        Log($"正在发送回复...");
        var success = await _automationService.ReplyToQuestionAsync(question, answer);
        
        if (success)
        {
            Log("回复发送成功");
            question.IsReplied = true;
            
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                _currentQuestionIndex++;
                await ShowCurrentQuestionAsync();
                UpdateProgress();
                btnPrev.Enabled = _currentQuestionIndex > 0;
                btnNext.Enabled = _currentQuestionIndex < _questions.Count - 1;
            }
        }
        else
        {
            Log("回复发送失败");
        }
    }

    private async void BtnSkip_Click(object? sender, EventArgs e)
    {
        if (btnPrev == null || btnNext == null)
            return;

        if (_currentQuestionIndex < _questions.Count - 1)
        {
            _currentQuestionIndex++;
            await ShowCurrentQuestionAsync();
            UpdateProgress();
            btnPrev.Enabled = _currentQuestionIndex > 0;
            btnNext.Enabled = _currentQuestionIndex < _questions.Count - 1;
            Log("已跳过当前问题");
        }
        else
        {
            Log("已经是最后一个问题");
        }
    }

    private async Task ShowCurrentQuestionAsync()
    {
        if (_currentQuestionIndex >= _questions.Count || txtQuestion == null || txtAnswer == null || lblQuestionInfo == null) 
            return;

        var question = _questions[_currentQuestionIndex];
        txtQuestion.Text = $"标题: {question.Title}\n\n内容: {question.Content}\n\n作者: {question.Author}\n时间: {question.PublishTime:yyyy-MM-dd HH:mm}";
        lblQuestionInfo.Text = question.IsReplied ? "✓ 已回复" : "未回复";
        lblQuestionInfo.ForeColor = question.IsReplied ? Color.Green : Color.Black;

        var match = await _knowledgeBaseService.FindBestMatchAsync(question.Content);
        txtAnswer.Text = match != null ? match.Answer : "未找到匹配的答案，请手动输入";
    }

    private void UpdateProgress()
    {
        if (lblProgress == null) return;
        lblProgress.Text = $"进度: {_currentQuestionIndex + 1}/{_questions.Count}";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _automationService.Dispose();
        base.OnFormClosing(e);
    }
}
