﻿using RpiHelpers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RpiHelpers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void DropActionHandler(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData("FileDrop");
            if (files.Length > 0)
            {
                IoT.Get<DropDataService>().Drop(
                    fileNames: files,
                    isDirectory: !IsFilePath(files[0]));
            }

            Activate();
        }

        private static bool IsFilePath(string path) =>
            Regex.IsMatch(path, @"(\w+\.?)+\.\w+$");
    }
}