using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PackCircle.Services;

public class TextBoxService
{
    public static void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        //if (sender is TextBox textBox) textBox.SelectAll();
        if (sender is TextBox textBox)
        {
            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                textBox.SelectAll();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }

    public static void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not TextBox { IsKeyboardFocusWithin: false } textBox) return;
        textBox.Focus();
        e.Handled = true;
    }
}