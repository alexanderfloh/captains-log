using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace CaptainsLog
{
  class LogFileMonitor : IDisposable {

    public readonly string FileName;
    private readonly LogViewerControl _logViewerControl;
    private int _readEntries;

    private FileSystemWatcher _watcher;

    public LogFileMonitor(string fileName, LogViewerControl logViewerControl)
    {
      FileName = fileName;
      _logViewerControl = logViewerControl;
      OpenFileAndMonitor();
    }

    private void OpenFileAndMonitor() {
      LogFileReader r = new LogFileReader();
      var data = r.Read(FileName);
      _logViewerControl.DataContext = new ObservableCollection<LogEvent>(data);
      _readEntries = data.Count;
      RegisterWatchers();
    }

    private void RegisterWatchers() {
      _watcher = new FileSystemWatcher();
      _watcher.Path = Path.GetDirectoryName(FileName);
      _watcher.Filter = Path.GetFileName(FileName);
      _watcher.NotifyFilter = NotifyFilters.LastWrite;
      _watcher.Changed += new FileSystemEventHandler(OnChanged);
      _watcher.IncludeSubdirectories = false;
      _watcher.EnableRaisingEvents = true;
    }

    void OnChanged(object sender, FileSystemEventArgs e) {
      LogFileReader r = new LogFileReader();
      ICollection<LogEvent> collection = r.Read(FileName);

      _logViewerControl.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() => {
        ObservableCollection<LogEvent> dataContext = (ObservableCollection<LogEvent>)_logViewerControl.DataContext;
        IEnumerable<LogEvent> newEntries = collection.Skip(_readEntries);
        foreach (var newEntry in newEntries) {
          dataContext.Add(newEntry);
        }
        _readEntries = dataContext.Count;
      } ));
    }

    public void Dispose() {
      _watcher.Dispose();
    }
  }
}
