using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers.Services
{
    internal class DropDataEventArgs
    {
        public DropDataEventArgs(IReadOnlyList<string> fileNames, bool isDirectory)
        {
            FileNames = fileNames ?? throw new ArgumentNullException(nameof(fileNames));
            IsDirectory = isDirectory;
        }

        public IReadOnlyList<string> FileNames { get; }

        public bool IsDirectory { get; }
    }
}
