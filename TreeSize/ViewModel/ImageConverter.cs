using FileSystem.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TreeSize.ViewModel
{
    
    public class ImageConverter : IValueConverter
    {
        public static ImageConverter Instance = new ImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = "Images/file.png";

            switch ((DataType)value)
            {
                case DataType.Drive:
                    image = "Images/hdd.png";
                    break;
                case DataType.Folder:
                    image = "Images/folder.png";
                    break;                
                case DataType.Empty:
                    return null;
            }

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
