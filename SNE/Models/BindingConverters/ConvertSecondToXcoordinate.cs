using System;
using System.Globalization;
using System.Windows.Data;

namespace SNE.Models.BindingConverters
{
    /// <summary>
    /// 秒→X座標
    /// </summary>
    class ConvertSecondToXcoordinate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sec = (double)value;
            return sec * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var xCoord = (double)value;
            return xCoord / 100;
        }
    }
}
