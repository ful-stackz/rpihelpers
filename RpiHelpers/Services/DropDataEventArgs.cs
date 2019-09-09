using System;
using System.Collections.Generic;

namespace RpiHelpers.Services
{
    internal class DropDataEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropDataEventArgs"/> class.
        /// </summary>
        /// <param name="fileNames">
        /// A collection with the paths to all dropped files.
        /// </param>
        /// <param name="hasOnlyDirectories">
        /// Indicates whether the <paramref name="fileNames"/> collection contains paths to directories only.
        /// </param>
        /// <param name="hasOnlyFiles">
        /// Indicates whether the <paramref name="fileNames"/> collection contains paths to files only.
        /// </param>
        /// <param name="hasDirectoriesAndFiles">
        /// Indicates whether the <paramref name="fileNames"/> collection contains paths to both directories and files.
        /// </param>
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

        /// <summary>
        /// Gets a collection with the full paths to all dropped files/directories.
        /// </summary>
        public IReadOnlyList<string> FileNames { get; }

        /// <summary>
        /// Gets a value indicating whether <see cref="FileNames"/> contains paths to directories only.
        /// </summary>
        public bool HasOnlyDirectories { get; }

        /// <summary>
        /// Gets a valie indicating whether <see cref="FileNames"/> contains paths to files only.
        /// </summary>
        public bool HasOnlyFiles { get; }

        /// <summary>
        /// Gets a value indicating whther <see cref="FileNames"/> contains paths to both directories and files.
        /// </summary>
        public bool HasDirectoriesAndFiles { get; }
    }
}
