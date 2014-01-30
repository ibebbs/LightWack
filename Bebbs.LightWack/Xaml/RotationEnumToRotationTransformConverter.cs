using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Bebbs.LightWack.Xaml
{
    public class RotationEnumToRotationTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Rotation)
            {
                switch((Rotation)value)
                {
                    case Rotation.Rotate180: return 180;
                    case Rotation.Rotate270: return 270;
                    case Rotation.Rotate90: return 90;
                    default: return 0;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
