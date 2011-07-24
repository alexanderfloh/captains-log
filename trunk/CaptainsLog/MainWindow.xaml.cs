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
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;

namespace CaptainsLog {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    private readonly List<LogFileMonitor> _logFileMonitors = new List<LogFileMonitor>();
    private ObservableCollection<string> RecentFiles { get; set; }

    public MainWindow() {
      InitializeComponent();
      SetVerionsString();
      RecentFiles = new ObservableCollection<string>();
      LBRecentFiles.ItemsSource = RecentFiles;
    }

    private void SetVerionsString() {
      try {
        version.Text = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
      } catch (System.Deployment.Application.InvalidDeploymentException) {
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
      this.Cursor = Cursors.Wait;

      try {
        string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
        if (droppedFilePaths != null) {
          foreach (string droppedFilePath in droppedFilePaths) {
            if (System.IO.File.Exists(droppedFilePath)) {
              LoadFile(droppedFilePath);
              UpdateRecentFiles(droppedFilePath);
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
      var logFileViewModel = LogFileViewModelBuilder.CreateForFile(fileName);
      CreateTab(fileName, logFileViewModel);

      _logFileMonitors.Add(logFileViewModel.LogFileMonitor);
    }

    private void CreateTab(string fileName, LogFileViewModel logFileViewModel) {
      var tabItem = new TabItem();
      tabItem.HeaderTemplate = (DataTemplate)App.Current.Resources["closableTabTemplate"];
      tabItem.Header = System.IO.Path.GetFileName(fileName);
      tabItem.Content = logFileViewModel.LogFileViewer;
      tabItem.AddHandler(Button.ClickEvent, new RoutedEventHandler(CloseTab));
      tabItem.Tag = logFileViewModel.LogFileMonitor;

      mainTab.Items.Add(tabItem);
      mainTab.SelectedItem = tabItem;
    }

    private void CloseTab(object sender, RoutedEventArgs e) {
      var tabItem = e.Source as TabItem;
      if (tabItem != null) {
        var tabControl = tabItem.Parent as TabControl;
        tabControl.Items.Remove(tabItem);

        var monitor = tabItem.Tag as LogFileMonitor;
        if(monitor != null) {
          _logFileMonitors.Remove(monitor);
          monitor.Dispose();
        }
      }
    }

    private void UpdateRecentFiles(string droppedFilePath) {
      var recentFiles = Properties.Settings.Default.RecentFiles ?? new System.Collections.Specialized.StringCollection();

      while (recentFiles.Contains(droppedFilePath)) {
        recentFiles.Remove(droppedFilePath);
      }

      recentFiles.Add(droppedFilePath);
      Properties.Settings.Default.RecentFiles = recentFiles;
      RecentFiles.Clear();
      foreach (var s in recentFiles) {
        RecentFiles.Add(s);
      }
    }

    public void Dispose() {
      _logFileMonitors.ForEach(m => m.Dispose());
    }

    private void OnIssueUrlClick(object sender, RoutedEventArgs e) {
      Process.Start("http://code.google.com/p/captains-log/issues/list");
    }
  }
}
