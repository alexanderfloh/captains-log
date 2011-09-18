namespace CaptainsLog {
  class LogFileViewModel {
    public LogViewerControl LogFileViewer { get; set; }
    public LogFileMonitor LogFileMonitor { get; set; }
    public string HighlightedText { get; set; }
  }
}
