using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;

namespace CaptainsLog
{
  /// <summary>
  /// Interaction logic for LogViewerControl.xaml
  /// </summary>
  public partial class LogViewerControl : UserControl
  {
    private Thread _workerThread;
    private string _lastSearchText;
    private IList<LogEvent> _matchingEntries;
    private int _nthEntry;

    public LogViewerControl()
    {
      InitializeComponent();
      Outline.Margin = new Thickness(5, dg.ColumnHeaderHeight, 5, 0);
    }

    private void SearchForText(object sender, TextChangedEventArgs e)
    {
      UpdateOutline();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateOutline();
    }
        
    private void FindNext() {
      if (_matchingEntries.Count > 0) {
        var firstEntry = _matchingEntries.ElementAtOrDefault(_nthEntry);
        _nthEntry = (_nthEntry + 1) % _matchingEntries.Count;
        dg.ScrollIntoView(firstEntry);
        dg.SelectedItem = firstEntry;
      }
    }

    private void UpdateOutline() {
      var searchText = Search.Text.ToUpperInvariant();

      if (_lastSearchText != searchText) {
        if (_workerThread != null) {
          _workerThread.Abort();
          Outline.Canvas.Children.Clear();
        }
        if (searchText.Length > 0) {
          var events = (ICollection<LogEvent>)DataContext;
          // see if search text is a timestamp
          DateTime dateTime;
          var matchingEntries = DateTime.TryParse(searchText, out dateTime) ? 
          events.Select((logEvent, index) => new { LogEvent = logEvent, Index = index }).Where(elem => {
            var timeOfDay = elem.LogEvent.Timestamp.TimeOfDay;
            if(timeOfDay.Hours == dateTime.Hour && timeOfDay.Minutes == dateTime.Minute) {
              return dateTime.Second == 0 || timeOfDay.Seconds == dateTime.Second;
            }
            return false;
          }) : 
         events.Select((logEvent, index) => new { LogEvent = logEvent, Index = index }).Where((elem) => elem.LogEvent.Message.ToUpperInvariant().Contains(searchText));
        
          _matchingEntries = matchingEntries.Select(pair => pair.LogEvent).ToList();  
          _nthEntry = 0;

          ThreadStart start = delegate() {

            foreach (var matchingEntry in matchingEntries) {
              double offset = (double)matchingEntry.Index / (double)events.Count;

              Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {

                var rectangle = new Rectangle();
                rectangle.Style = (Style)Resources["MarkerStyle"];
                rectangle.Height = Math.Max(Outline.ActualHeight / events.Count, 5);
                rectangle.Width = 15;
                rectangle.MouseUp += OnMarkerClick;
                rectangle.DataContext = matchingEntry.LogEvent;
                rectangle.SetValue(Canvas.TopProperty, (Outline.ActualHeight - 5) * offset);
                rectangle.ToolTip = matchingEntry.LogEvent.Message;

                Outline.Canvas.Children.Add(rectangle);
              }));
            }
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(FindNext));
          };

          _workerThread = new Thread(start);
          _workerThread.Start();
        }
        else {
          _matchingEntries = new List<LogEvent>();
        }
      }
      _lastSearchText = searchText;
    }

    
    private void OnMarkerClick(object sender, MouseButtonEventArgs e) {
      var marker = sender as Rectangle;
      dg.ScrollIntoView(marker.DataContext);
      dg.SelectedItems.Add(marker.DataContext);
    }

    private void CommandBindingCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    private void CommandBindingExecuted(object sender, ExecutedRoutedEventArgs e) {
      if (e.Command == ApplicationCommands.Copy) {
        var logEvent = e.Parameter as LogEvent;
        Clipboard.Clear();

        if (logEvent != null) {
          var logEventAsString = FormatSingleLogEvent(logEvent);
          Clipboard.SetText(logEventAsString);
        }
        else {
          var logEventsFormatted = dg.SelectedItems.OfType<LogEvent>().Select(FormatSingleLogEvent).Aggregate((formatted, next) => formatted + next);
          Clipboard.SetText(logEventsFormatted);
        }
      }
      else if (e.Command == ApplicationCommands.Find) {
        FindNext();
      }
    }

    private string FormatSingleLogEvent(LogEvent logEvent) {
      return String.Format("{0}: {1}\r\nLogger: {2}\r\nThread: {3} ({4})\r\nThrowable: {5}\r\nTime: {6}\r\n",
            logEvent.Level,
            logEvent.Message,
            logEvent.Logger,
            logEvent.Thread,
            logEvent.Location,
            logEvent.Throwable,
            logEvent.Timestamp);
    }
  }
}
