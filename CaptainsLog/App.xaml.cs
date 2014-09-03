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
      var activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
      if (activationData != null && activationData.Length > 0) {
        try {
          var uri = new Uri(activationData[0]);
          var fileName = uri.LocalPath;
          var mainWindow = (MainWindow) MainWindow;
          mainWindow.LoadFile(fileName);
        }
        catch (Exception) {
        }
      }
    }
  }
}
