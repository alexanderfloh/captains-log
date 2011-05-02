using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptainsLog {
  class LogFileViewModelBuilder {
    public static LogFileViewModel CreateForFile(string fileName) {
      var logViewer = new LogViewerControl();
      var logFileMonitor = new LogFileMonitor(fileName, logViewer);
      var logFileViewModel = new LogFileViewModel();
      logFileViewModel.LogFileMonitor = logFileMonitor;
      logFileViewModel.LogFileViewer = logViewer;

      return logFileViewModel;
    }
  }
}
