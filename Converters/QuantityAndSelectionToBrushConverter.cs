using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InventoryApp.Converters
{
    public class QuantityAndSelectionToBrushConverter : IMultiValueConverter
    {
        public Brush LowQuantityBrush { get; set; } = Brushes.Salmon;
        public Brush MediumQuantityBrush { get; set; } = Brushes.LightGoldenrodYellow;
        public Brush NormalBrush { get; set; } = Brushes.White;
        public Brush SelectedBrush { get; set; } = Brushes.LightSkyBlue;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] is not int quantity || values[1] is not bool isSelected)
                return NormalBrush;

            if (isSelected)
                return SelectedBrush;

            if (quantity <= 10)
                return LowQuantityBrush;

            if (quantity <= 20)
                return MediumQuantityBrush;

            return NormalBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
