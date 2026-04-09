using System;
using System.Globalization;
using System.Windows.Data;

namespace EightBTenBViewer.Converters
{
    public class FirstRowOnlyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                return string.Empty;
            }

            if (values[1] is int rowIndex && rowIndex == 0)
            {
                return values[0]?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
