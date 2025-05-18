using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace InventoryApp.Behaviors
{
    public class SelectedItemsBehaviorListBox : Behavior<ListBox>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedListItems), typeof(IList), typeof(SelectedItemsBehaviorListBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IList SelectedListItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
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
