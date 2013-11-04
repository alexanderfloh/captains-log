using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CaptainsLog {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    private readonly List<LogFileMonitor> _logFileMonitors = new List<LogFileMonitor>();
    private ObservableCollection<string> RecentFiles { get; set; }

    public MainWindow() {
      InitializeComponent();
      SetVersionString();

      RecentFiles = new ObservableCollection<string>();
      welcomeTab.LBRecentFiles.ItemsSource = RecentFiles;

      var recentFiles = Properties.Settings.Default.RecentFiles ?? new StringCollection();
      ConvertRecentFiles(recentFiles);
    }

    private void SetVersionString() {
      try {
        version.Text = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
      }
      catch (System.Deployment.Application.InvalidDeploymentException) {
        // happens for local build
      }
    }

    private void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e) {
      LoadFile((string)e.Parameter);
    }

    private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = File.Exists((string)e.Parameter);
    }

    private void FileDropped(object sender, DragEventArgs e) {
      Cursor = Cursors.Wait;

      try {
        var droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
        if (droppedFilePaths != null) {
          foreach (var droppedFilePath in droppedFilePaths) {
            if (File.Exists(droppedFilePath)) {
              LoadFile(droppedFilePath);
              UpdateRecentFiles(droppedFilePath);
            }
            Properties.Settings.Default.Save();
          }
        }
      }
      finally {
        Cursor = Cursors.Arrow;
      }
    }

    private void LoadFile(string fileName) {
      var logFileViewModel = LogFileViewModelBuilder.CreateForFile(fileName);
      CreateTab(fileName, logFileViewModel);

      _logFileMonitors.Add(logFileViewModel.LogFileMonitor);
    }

    private void CreateTab(string fileName, LogFileViewModel logFileViewModel) {
      var tabItem = new TabItem
      {
        HeaderTemplate = (DataTemplate)Application.Current.Resources["closableTabTemplate"],
        Header = Path.GetFileName(fileName),
        Content = logFileViewModel.LogFileViewer,
        Tag = logFileViewModel.LogFileMonitor,
      };
      mainTab.Items.Add(tabItem);
      mainTab.SelectedItem = tabItem;
    }

    private void CloseTab(object sender, RoutedEventArgs e) {
      var tabItem = e.Source as TabItem;
      if (tabItem != null) {
        var tabControl = tabItem.Parent as TabControl;
        tabControl.Items.Remove(tabItem);

        var monitor = tabItem.Tag as LogFileMonitor;
        if (monitor != null) {
          _logFileMonitors.Remove(monitor);
          monitor.Dispose();
        }
      }
    }

    private void UpdateRecentFiles(string droppedFilePath) {
      var recentFiles = Properties.Settings.Default.RecentFiles ?? new StringCollection();

      while (recentFiles.Contains(droppedFilePath)) {
        recentFiles.Remove(droppedFilePath);
      }

      recentFiles.Insert(0, droppedFilePath);
      Properties.Settings.Default.RecentFiles = recentFiles;
      ConvertRecentFiles(recentFiles);
    }

    private void ConvertRecentFiles(StringCollection recentFiles) {
      RecentFiles.Clear();
      foreach (var s in from string s in recentFiles where File.Exists(s) select s) {
        RecentFiles.Add(s);
      }
    }

    public void Dispose() {
      _logFileMonitors.ForEach(m => m.Dispose());
    }

    private void OnIssueUrlClick(object sender, RoutedEventArgs e) {
      Process.Start("http://code.google.com/p/captains-log/issues/list");
    }

    private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
      CloseTab(sender, e);
    }

    private void CloseCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = true;
    }
  }
}
