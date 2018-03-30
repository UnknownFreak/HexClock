using System;
using System.Windows.Data;
using System.Windows.Media;
namespace DesktopApplication.Shapes
{
    public class ColorToSolidColorBrushValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is Brush br)
            {
                byte a = ((Color)br.GetValue(SolidColorBrush.ColorProperty)).A;
                byte g = ((Color)br.GetValue(SolidColorBrush.ColorProperty)).G;
                byte r = ((Color)br.GetValue(SolidColorBrush.ColorProperty)).R;
                byte b = ((Color)br.GetValue(SolidColorBrush.ColorProperty)).B;
                return Color.FromRgb(r, g, b);
            }
            // You can support here more source types if you wish
            // For the example I throw an exception

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // If necessary, here you can convert back. Check if which brush it is (if its one),
            // get its Color-value and return it.

            throw new NotImplementedException();
        }
    }
}