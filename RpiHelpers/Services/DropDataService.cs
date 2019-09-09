using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RpiHelpers.Services
{
    internal class DropDataService
    {
        /// <summary>
        /// Raised when the <see cref="DropDataService"/> receives a drop notification
        /// and provides details about the dropped data.
        /// </summary>
        public event EventHandler<DropDataEventArgs> OnDrop;

        /// <summary>
        /// Raises a notification that a collection of specified <paramref name="fileNames"/>
        /// has been dropped.
        /// </summary>
        /// <param name="fileNames">
        /// A collection with the file names of the dropped files.
        /// </param>
        public void Drop(string[] fileNames)
        {
            bool onlyFiles = fileNames.Where(IsFilePath).Count() == fileNames.Length;
            bool onlyDirectories = !onlyFiles && fileNames.Where(x => !IsFilePath(x)).Count() == fileNames.Length;
            bool directoriesAndFiles = !onlyFiles && !onlyDirectories;
            OnDrop?.Invoke(this, new DropDataEventArgs(fileNames, onlyDirectories, onlyFiles, directoriesAndFiles));
        }

        /// <summary>
        /// Checks whether the specified <paramref name="path"/> is a path to a file.
        /// </summary>
        /// <param name="path">
        /// A path to a local file or directory.
        /// </param>
        /// <returns>
        /// A <see cref="bool"/> value indicating whether the specified <paramref name="path"/>
        /// is a path to a file.
        /// </returns>
        public static bool IsFilePath(string path) =>
            Regex.IsMatch(path, @"(\w+\.?)+\.\w+$");
    }
}
