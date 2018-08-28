using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Warframe_Toolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            


        }
        protected override void OnExit(ExitEventArgs e)
        {
            Warframe_Toolbox.Properties.Settings.Default.Save();
        }
    }
}
