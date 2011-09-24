using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace CaptainsLog.Converter {
  [ValueConversion(typeof(LocationInfo), typeof(string))]
  class LocationConverter : IValueConverter {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var location = value as LocationInfo;

      return String.Format("{0}.{1} ({2}:{3})", location.Class, location.Method, location.File, location.Line);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return DependencyProperty.UnsetValue;
    }
  }
}
