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

namespace CaptainsLog
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly List<LogFileMonitor> _logFileMonitors = new List<LogFileMonitor>();

    public MainWindow()
    {
      InitializeComponent();
    }

    private void FileDropped(object sender, DragEventArgs e)
    {
      this.Cursor = Cursors.Wait;

      string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];

      foreach (string droppedFilePath in droppedFilePaths)
      {
        LoadFile(droppedFilePath);
        
        if(Properties.Settings.Default.RecentFiles == null) {
          Properties.Settings.Default.RecentFiles = new System.Collections.Specialized.StringCollection();
        }  
        Properties.Settings.Default.RecentFiles.Add(droppedFilePath);
      }
      Properties.Settings.Default.Save();
      this.Cursor = Cursors.Arrow;
    }

    private void CloseTab(object sender, RoutedEventArgs e)
    {
      var tabItem = e.Source as TabItem;
      if (tabItem != null)
      {
        var tabControl = tabItem.Parent as TabControl;
        tabControl.Items.Remove(tabItem);
      }
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
      
    }

    private void LoadFile(string fileName) {
      var logFileMonitor = new LogFileMonitor(fileName);
      var logViewer = new LogViewerControl();
      logViewer.DataContext = logFileMonitor.LogEvents;

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

    private void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e)
    {
      //String command, targetobj;
      //command = ((RoutedCommand)e.Command).Name;
      //targetobj = ((FrameworkElement)target).Name;
      LoadFile((string)e.Parameter);
      //MessageBox.Show("The " + command + " command has been invoked on target object " + targetobj);
    }

    private void OpenCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    public void Dispose() {
      _logFileMonitors.ForEach(a => a.Dispose());
    }
  }
}
