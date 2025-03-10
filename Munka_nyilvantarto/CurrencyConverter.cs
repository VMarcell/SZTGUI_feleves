using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Munka_nyilvantarto
{
    internal class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int amount)
            {
                return $"{amount} E FT";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Visszafelé konvertálás nem szükséges ebben az esetben
            throw new NotImplementedException();
        }
    }
}
