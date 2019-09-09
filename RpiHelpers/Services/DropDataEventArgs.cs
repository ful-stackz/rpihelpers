using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers.Services
{
    internal class DropDataEventArgs
    {
        public DropDataEventArgs(
            IReadOnlyList<string> fileNames,
            bool hasOnlyDirectories,
            bool hasOnlyFiles,
            bool hasDirectoriesAndFiles)
        {
            FileNames = fileNames ?? throw new ArgumentNullException(nameof(fileNames));
            HasOnlyDirectories = hasOnlyDirectories;
            HasOnlyFiles = hasOnlyFiles;
            HasDirectoriesAndFiles = hasDirectoriesAndFiles;
        }

        public IReadOnlyList<string> FileNames { get; }

        public bool HasOnlyDirectories { get; }
        public bool HasOnlyFiles { get; }
        public bool HasDirectoriesAndFiles { get; }
    }
}
