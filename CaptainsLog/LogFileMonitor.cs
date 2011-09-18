using System;
using System.Linq;
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
      var r = new LogFileReader();
      var data = r.Read(FileName);
      _logViewerControl.DataContext = new ObservableCollection<LogEvent>(data);
      _readEntries = data.Count;
      RegisterWatchers();
    }

    private void RegisterWatchers() {
      _watcher = new FileSystemWatcher {
                     Path = Path.GetDirectoryName(FileName),
                     Filter = Path.GetFileName(FileName),
                     NotifyFilter = NotifyFilters.LastWrite
                   };
      _watcher.Changed += OnChanged;
      _watcher.IncludeSubdirectories = false;
      _watcher.EnableRaisingEvents = true;
    }

    void OnChanged(object sender, FileSystemEventArgs e) {
      var r = new LogFileReader();
      var collection = r.Read(FileName);

      _logViewerControl.Dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() => {
        var dataContext = (ObservableCollection<LogEvent>)_logViewerControl.DataContext;
        var newEntries = collection.Skip(_readEntries);
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
