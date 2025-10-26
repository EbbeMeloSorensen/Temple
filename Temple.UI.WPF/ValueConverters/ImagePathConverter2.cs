using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Temple.UI.WPF.ValueConverters
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class ImagePathConverter2 : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                var uri = new Uri($"pack://application:,,,/{value}", UriKind.Absolute);
                return new BitmapImage(uri);
            }
            catch (Exception)
            {
                var uri = new Uri("pack://application:,,,/DD/Images/NoPreview.png", UriKind.Absolute);
                return new BitmapImage(uri);
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

