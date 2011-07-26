using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace CaptainsLog.Converter {
  [ValueConversion(typeof(string), typeof(Brush))]
  class SolidLevelConverter : IValueConverter {

    public static readonly SolidColorBrush TraceBrush = new SolidColorBrush(Colors.Gray);
    public static readonly SolidColorBrush DebugBrush = new SolidColorBrush(Colors.Green);
    public static readonly SolidColorBrush InfoBrush = new SolidColorBrush(Colors.SteelBlue);
    public static readonly SolidColorBrush WarnBrush = new SolidColorBrush(Colors.BurlyWood);
    public static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Colors.IndianRed);
    public static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.WhiteSmoke);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var level = value as string;
      switch (level) {
        case "TRACE":
          return TraceBrush;

        case "DEBUG":
          return DebugBrush;

        case "INFO":
          return InfoBrush;

        case "WARN":
          return WarnBrush;

        case "ERROR":
          return ErrorBrush;

        default:
          return DefaultBrush;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return DependencyProperty.UnsetValue;
    }
  }
}
