using System;
using System.Globalization;
using System.Windows.Data;

namespace ContactsBook
{
    public class ColumnHighlightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string currentProperty && parameter is string columnName)
            {
                return currentProperty == columnName ? "LightBlue" : "Transparent";
            }
            return "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}