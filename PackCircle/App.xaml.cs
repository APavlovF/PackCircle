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
            new RoutedEventHandler(TextBox_GotFocus));

        EventManager.RegisterClassHandler(
            typeof(Control),
            UIElement.PreviewKeyDownEvent,
            new KeyEventHandler(OnControlPreviewKeyDown));

        EventManager.RegisterClassHandler(
            typeof(TextBox),
            UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(TextBox_PreviewMouseLeftButtonDown));

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

    void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox) textBox.SelectAll();
    }

    void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBox { IsKeyboardFocusWithin: false } textBox) return;
        textBox.Focus();
        e.Handled = true;
    }

    private void OnControlPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.OriginalSource is not Control activeControl) return;
        Window? window = Window.GetWindow(activeControl);
        if (window?.FindName("SetupContainer") is not FrameworkElement setupGroup) return;
        if (!activeControl.IsDescendantOf(setupGroup)) return;
        TextBox? firstTextBox = FindChild<TextBox>(setupGroup, t => !t.IsReadOnly);
        TextBox? lastTextBox = null;
        FindLastChild(setupGroup, ref lastTextBox, t => t?.IsReadOnly != true);
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            if (activeControl == lastTextBox)
            {
                firstTextBox?.Focus();
            }
            else
            {
                activeControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
            return;
        }

        if (activeControl is TextBox activeTextBox)
        {
            if (e.Key == Key.Down)
            {
                e.Handled = true;

                if (activeTextBox == lastTextBox)
                {
                    firstTextBox?.Focus();
                }
                else
                {
                    activeTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    if (Keyboard.FocusedElement is not TextBox || ((TextBox)Keyboard.FocusedElement).IsReadOnly)
                    {
                        firstTextBox?.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;

                if (activeTextBox == firstTextBox)
                {
                    lastTextBox?.Focus();
                }
                else
                {
                    activeTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));

                    if (Keyboard.FocusedElement is not TextBox || ((TextBox)Keyboard.FocusedElement).IsReadOnly)
                    {
                        lastTextBox?.Focus();
                    }
                }
            }
        }
    }

    T? FindChild<T>(DependencyObject parent, Predicate<T> filter) where T : DependencyObject
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T target && filter(target)) return target;
            T? childOfChild = FindChild(child, filter);
            if (childOfChild != null) return childOfChild;
        }

        return null;
    }

    void FindLastChild<T>(DependencyObject parent, ref T lastFound, Predicate<T> filter) where T : DependencyObject?
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T target && filter(target)) lastFound = target;
            FindLastChild(child, ref lastFound, filter);
        }
    }
}