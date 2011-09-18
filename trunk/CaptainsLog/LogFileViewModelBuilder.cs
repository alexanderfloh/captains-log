namespace CaptainsLog {
  class LogFileViewModelBuilder {
    public static LogFileViewModel CreateForFile(string fileName) {
      var logViewer = new LogViewerControl();
      var logFileMonitor = new LogFileMonitor(fileName, logViewer);
      var logFileViewModel = new LogFileViewModel {LogFileMonitor = logFileMonitor, LogFileViewer = logViewer};

      return logFileViewModel;
    }
  }
}
