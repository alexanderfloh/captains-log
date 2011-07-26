using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace CaptainsLog.Converter {
  [ValueConversion(typeof(string), typeof(Brush))]
  class LevelConverter : IValueConverter {
    public static readonly LinearGradientBrush TraceBrush = new LinearGradientBrush(Colors.Gray, Colors.Transparent, 0);
    public static readonly LinearGradientBrush DebugBrush = new LinearGradientBrush(Colors.Green, Colors.Transparent, 0);
    public static readonly LinearGradientBrush InfoBrush = new LinearGradientBrush(Colors.SteelBlue, Colors.Transparent, 0);
    public static readonly LinearGradientBrush WarnBrush = new LinearGradientBrush(Colors.BurlyWood, Colors.Transparent, 0);
    public static readonly LinearGradientBrush ErrorBrush = new LinearGradientBrush(Colors.IndianRed, Colors.Transparent, 0);
    public static readonly LinearGradientBrush DefaultBrush = new LinearGradientBrush(Colors.WhiteSmoke, Colors.Transparent, 0);

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
