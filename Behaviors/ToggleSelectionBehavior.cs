using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Windows;
using System;
using System.Windows.Media;

namespace InventoryApp.Behaviors
{
    public class ToggleSelectionBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext != null)
            {
                var item = fe.DataContext;
                var listBoxItem = FindAncestor<ListBoxItem>(fe);
                if (listBoxItem != null && listBoxItem.IsSelected)
                {
                    AssociatedObject.SelectedItems.Remove(item);
                    e.Handled = true;
                }
            }
        }

        private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T t) return t;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
