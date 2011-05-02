using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaptainsLog {
  /// <summary>
  /// Interaction logic for HighlightBar.xaml
  /// </summary>
  public partial class HighlightBar : UserControl {
    public HighlightBar() {
      InitializeComponent();
    }

    private void OnMarkerClick(object sender, MouseButtonEventArgs e) {
      var marker = sender as Rectangle;
      //dg.ScrollIntoView(marker.DataContext);
      //dg.SelectedItems.Add(marker.DataContext);
    }

    private void UpdateOutline() {
      //Canvas.Children.Clear();
      //string searchText = Search.Text.ToUpperInvariant();

      //if (searchText.Length > 0) {
      //  ICollection<LogEvent> events = (ICollection<LogEvent>)DataContext;
      //  var matchingEntries = events.Select((logEvent, index) => new { LogEvent = logEvent, Index = index }).Where((elem) => elem.LogEvent.Message.ToUpperInvariant().Contains(searchText));

      //  foreach (var matchingEntry in matchingEntries) {
      //    double offset = (double)matchingEntry.Index / (double)events.Count;
      //    var rectangle = new Rectangle();
      //    rectangle.Style = (Style)Resources["MarkerStyle"];
      //    rectangle.Height = Math.Max(Outline.ActualHeight / events.Count, 5);
      //    rectangle.Width = 15;
      //    rectangle.MouseUp += OnMarkerClick;
      //    rectangle.DataContext = matchingEntry.LogEvent;
      //    rectangle.SetValue(Canvas.TopProperty, (Outline.ActualHeight - 5) * offset);
      //    rectangle.ToolTip = matchingEntry.LogEvent.Message;
      //    Outline.Children.Add(rectangle);
      //  }
      //}
    }
  }
}
