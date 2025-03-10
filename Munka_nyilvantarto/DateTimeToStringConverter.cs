using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Munka_nyilvantarto
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Ellenőrizzük, hogy az érték megegyezik-e az alapértelmezett értékkel
                if (dateTime == DateTime.MinValue)
                {
                    return "-";
                }
                return dateTime.ToString("yyyy-MM-dd"); // Dátum formátum
            }

            return "-"; // Ha valamiért nem DateTime, "-"-ot adunk vissza
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
