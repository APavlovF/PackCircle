using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PackCircle.Services;

public class InputNavigationService
{
    public static void OnControlPreviewKeyDown(object sender, KeyEventArgs e)
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

        if (activeControl is not TextBox activeTextBox) return;
        switch (e.Key)
        {
            case Key.Down:
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

                    break;
                }
            case Key.Up:
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

                    break;
                }
        }
    }

    static T? FindChild<T>(DependencyObject parent, Predicate<T> filter) where T : DependencyObject
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

    static void FindLastChild<T>(DependencyObject parent, ref T lastFound, Predicate<T> filter) where T : DependencyObject?
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            if (child is T target && filter(target)) lastFound = target;
            FindLastChild(child, ref lastFound, filter);
        }
    }
}