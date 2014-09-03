using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace CaptainsLog {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {

    private void OnExit(object sender, ExitEventArgs e) {
      //Properties.Settings.Default.Save();
    }

    private void OnLoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e) {
      
    }
  }
}
