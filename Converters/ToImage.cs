

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace Telegram_WPF.Converters
{
    internal class ToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var inttext =
                 System.Text.RegularExpressions.Regex.Replace((string)value, "[^.0-9]", "");

                int number;
                return Int32.TryParse(inttext, out number) ? number : 0;
            }
            else
            {
                return value;
            }
        }
    }
}
