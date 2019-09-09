using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RpiHelpers.Services
{
    internal class DropDataService
    {
        public event EventHandler<DropDataEventArgs> OnDrop;

        public void Drop(string[] fileNames)
        {
            bool onlyFiles = fileNames.Where(IsFilePath).Count() == fileNames.Length;
            bool onlyDirectories = !onlyFiles && fileNames.Where(x => !IsFilePath(x)).Count() == fileNames.Length;
            bool directoriesAndFiles = !onlyFiles && !onlyDirectories;
            OnDrop?.Invoke(this, new DropDataEventArgs(fileNames, onlyDirectories, onlyFiles, directoriesAndFiles));
        }

        public static bool IsFilePath(string path) =>
            Regex.IsMatch(path, @"(\w+\.?)+\.\w+$");
    }
}
