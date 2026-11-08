using System;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using platformAutoReply.App.Models;
using platformAutoReply.App.Services;

namespace platformAutoReply.App;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var config = LoadConfiguration();
        var services = ConfigureServices(config);
        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILoggingService>();
        logger.Information("程序启动");

        try
        {
            var form = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(form);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "程序发生致命错误");
            MessageBox.Show($"程序发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    static AppConfig LoadConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var appConfig = new AppConfig();
        configuration.Bind(appConfig);

        return appConfig;
    }

    static IServiceCollection ConfigureServices(AppConfig config)
    {
        var services = new ServiceCollection();

        services.AddSingleton(config);
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<IKnowledgeBaseService, KnowledgeBaseService>();
        services.AddSingleton<IPlatformAutomationService, PlatformAutomationService>();
        services.AddSingleton<MainForm>();

        return services;
    }
}
