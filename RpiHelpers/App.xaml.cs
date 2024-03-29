﻿using RpiHelpers.Helpers;
using RpiHelpers.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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
            IoC.Register<DropDataService>(new DropDataService());
            IoC.Register<RpiFileService>(new RpiFileService(new CommandExecutor()));
            IoC.Register<RpiConfigService>(new RpiConfigService());
            IoC.Register<RpiConnectionService>(new RpiConnectionService(new CommandExecutor()));
            IoC.Register<SnackbarService>(
                new SnackbarService(
                    timerFactory: (action, period) => new Timer(
                        callback: (_) => action(),
                        state: null,
                        dueTime: period,
                        period: period)));
            base.OnStartup(e);
        }
    }
}
