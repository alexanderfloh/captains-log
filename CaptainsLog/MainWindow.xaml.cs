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
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    private readonly List<LogFileMonitor> _logFileMonitors = new List<LogFileMonitor>();

    public MainWindow() {
      InitializeComponent();
    }

    private void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e) {
      LoadFile((string)e.Parameter);
    }

    private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }

    private void FileDropped(object sender, DragEventArgs e) {
      this.Cursor = Cursors.Wait;
      try {
        string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
        if (droppedFilePaths != null) {
          foreach (string droppedFilePath in droppedFilePaths) {
            if (System.IO.File.Exists(droppedFilePath)) {
              LoadFile(droppedFilePath);

              var recentFiles = Properties.Settings.Default.RecentFiles;

              if (recentFiles == null) {
                recentFiles = new System.Collections.Specialized.StringCollection();
              }

              while (recentFiles.Contains(droppedFilePath)) {
                recentFiles.Remove(droppedFilePath);
              }
              recentFiles.Add(droppedFilePath);
            }
            Properties.Settings.Default.Save();
          }
        }
      }
      finally {
        this.Cursor = Cursors.Arrow;
      }
    }

    private void LoadFile(string fileName) {
      var logViewer = new LogViewerControl();
      var logFileMonitor = new LogFileMonitor(fileName, logViewer);

      var tabItem = new TabItem();
      tabItem.HeaderTemplate = (DataTemplate)App.Current.Resources["closableTabTemplate"];
      tabItem.Header = System.IO.Path.GetFileName(fileName);
      tabItem.Content = logViewer;
      tabItem.ToolTip = fileName;
      tabItem.AddHandler(Button.ClickEvent, new RoutedEventHandler(CloseTab));

      mainTab.Items.Add(tabItem);
      mainTab.SelectedItem = tabItem;

      _logFileMonitors.Add(logFileMonitor);
    }

    private void CloseTab(object sender, RoutedEventArgs e) {
      var tabItem = e.Source as TabItem;
      if (tabItem != null) {
        var tabControl = tabItem.Parent as TabControl;
        tabControl.Items.Remove(tabItem);
        LogViewerControl logViewer = tabItem.Content as LogViewerControl;
        if (logViewer != null) {
          // TODO: it's a bit hack-ish to use the tooltip to decide if the elemnt should be removed
          _logFileMonitors.Where((monitor) => monitor.FileName == (string)tabItem.ToolTip).ToList().ForEach((monitor) => monitor.Dispose());
          _logFileMonitors.RemoveAll((monitor) => monitor.FileName == (string)tabItem.ToolTip);
        }
      }
    }

    public void Dispose() {
      _logFileMonitors.ForEach(a => a.Dispose());
    }
  }
}
