using System;
using System.Globalization;
using System.Windows.Data;

namespace ContactsBook
{
    public class SortIndicatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Tuple<string, bool> sortDirection && parameter is string columnName)
            {
                if (sortDirection.Item1 == columnName)
                {
                    return sortDirection.Item2 ? " ↓" : " ↑";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}