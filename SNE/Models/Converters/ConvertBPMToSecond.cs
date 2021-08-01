using System;
using System.Globalization;
using System.Windows.Data;

namespace SNE.Models.Converters
{
    class ConvertBPMToSecond : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bpm = (double)value;
            return 600 / bpm;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sec = (double)value;
            return 600 * sec;
        }
    }
}
