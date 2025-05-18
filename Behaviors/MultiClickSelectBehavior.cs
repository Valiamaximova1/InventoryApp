using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace InventoryApp.Behaviors
{
    public class MultiClickSelectBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe &&
        (fe is Button || VisualUpwardSearch<Button>(fe) != null))
                return;


            var listBox = AssociatedObject;
            var item = VisualUpwardSearch<ListBoxItem>((DependencyObject)e.OriginalSource);
            if (item == null) return;

            if (item.IsSelected)
            {
                item.IsSelected = false;
                listBox.SelectedItems.Remove(item.DataContext);
            }
            else
            {
                item.IsSelected = true;
                listBox.SelectedItems.Add(item.DataContext);
            }

            e.Handled = true;
        }

        private static T? VisualUpwardSearch<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T target)
                    return target;

                if (current is not Visual && current is not Visual3D)
                    return null;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
            //while (source != null && source is not T)
            //    source = VisualTreeHelper.GetParent(source);
            //return source as T;
        }
    }
}