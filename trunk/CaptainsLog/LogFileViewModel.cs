using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptainsLog {
  class LogFileViewModel {
    public LogViewerControl LogFileViewer { get; set; }
    public LogFileMonitor LogFileMonitor { get; set; }
    public string HighlightedText { get; set; }
  }
}
