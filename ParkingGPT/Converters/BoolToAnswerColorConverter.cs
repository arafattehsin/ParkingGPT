using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGPT.Converters
{
    class BoolToAnswerColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var decision = System.Convert.ToString(value);

            if(decision.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb("#008000"); // Green
            }
            else if (decision.Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb("#800000"); // Maroon
            }
            else if (decision.Equals("warning", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb("#FFD600"); // Yellow (bright, visible)
            }
            else
            {
                return Colors.Gray; // Fallback color
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
