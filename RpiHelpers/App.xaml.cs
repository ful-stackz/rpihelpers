using RpiHelpers.Helpers;
using RpiHelpers.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RpiHelpers
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IoT.Register<DropDataService>(new DropDataService());
            IoT.Register<RpiFileService>(new RpiFileService(new CommandExecutor()));
            IoT.Register<RpiConfigService>(new RpiConfigService());
            base.OnStartup(e);
        }
    }
}
