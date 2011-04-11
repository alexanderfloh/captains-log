using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CaptainsLog
{
  class LogFileMonitor : IDisposable {

    private readonly string _fileName;
    private FileSystemWatcher _watcher;

    public SortedSet<LogEvent> LogEvents;

    public LogFileMonitor(string fileName) {
      _fileName = fileName;
      OpenFileAndMonitor();
    }

    private void OpenFileAndMonitor() {
      LogFileReader r = new LogFileReader();
      LogEvents = r.Read(_fileName);
      RegisterWatchers();
    }

    private void RegisterWatchers() {
      _watcher = new FileSystemWatcher();
      _watcher.Path = Path.GetDirectoryName(_fileName);
      _watcher.Filter = Path.GetFileName(_fileName);
      _watcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
      _watcher.Changed += new FileSystemEventHandler(OnChanged);
      _watcher.IncludeSubdirectories = false;
      _watcher.EnableRaisingEvents = true;
    }

    void OnChanged(object sender, FileSystemEventArgs e) {
      //LogFileReader r = new LogFileReader();
      //LogEvents.Clear();
      //SortedSet<LogEvent> s = r.Read(_fileName);
      //foreach (LogEvent logEvent in s) {
      //  LogEvents.Add(logEvent);
      //}
    }

    public void Dispose() {
      _watcher.Dispose();
    }
  }
}
