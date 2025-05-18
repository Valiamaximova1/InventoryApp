using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace InventoryApp.Behaviors
{
    public class SelectedItemsBehaviorListBox : Behavior<ListBox>
    {
        public static readonly DependencyProperty SelectedListItemsProperty =
            DependencyProperty.Register(nameof(SelectedListItems), typeof(IList), typeof(SelectedItemsBehaviorListBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IList SelectedListItems
        {
            get => (IList)GetValue(SelectedListItemsProperty);
            set => SetValue(SelectedListItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;

            AssociatedObject.Loaded += (_, _) =>
            {
                if (SelectedListItems != null)
                {
                    AssociatedObject.SelectedItems.Clear();
                    foreach (var item in SelectedListItems)
                    {
                        AssociatedObject.SelectedItems.Add(item);
                    }
                }
            };
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedListItems == null) return;

            foreach (var item in e.RemovedItems)
                SelectedListItems.Remove(item);

            foreach (var item in e.AddedItems)
                if (!SelectedListItems.Contains(item))
                    SelectedListItems.Add(item);
        }
    }
}
