using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PackCircle.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PackCircle;

public partial class App
{
    readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                // Services
                services.AddTransient<CircleViewer>();
                services.AddTransient<ExportFileService>();
                services.AddTransient<RectangularPacker>();
                services.AddTransient<TriangularPacker>();
                // ViewModels
                services.AddSingleton<MainViewModel>();
                // Windows
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        EventManager.RegisterClassHandler(
            typeof(TextBox),
            UIElement.GotFocusEvent,
            new RoutedEventHandler(TextBoxService.TextBox_GotFocus));

        EventManager.RegisterClassHandler(
            typeof(Control),
            UIElement.PreviewKeyDownEvent,
            new KeyEventHandler(InputNavigationService.OnControlPreviewKeyDown));

        EventManager.RegisterClassHandler(
            typeof(TextBox),
            UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(TextBoxService.TextBox_PreviewMouseLeftButtonDown));

        MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }

        base.OnExit(e);
    }
}