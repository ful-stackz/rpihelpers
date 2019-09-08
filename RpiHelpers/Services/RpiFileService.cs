using RpiHelpers.Configuration;
using RpiHelpers.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RpiHelpers.Services
{
    class RpiFileService
    {
        private const string AllFilesFilter = "*";
        private readonly CommandExecutor _cmdExecutor;

        public RpiFileService(CommandExecutor commandExecutor)
        {
            _cmdExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        }

        public void CopyFile(string sourcePath, string targetPath, RpiConfig rpiConfig)
        {
            const string CopyCommand = "scp";

            _cmdExecutor.Execute($"{CopyCommand} {sourcePath.Replace('/', '\\')} pi@raspberry.local:~\\{targetPath.Replace('/', '\\')}");
        }

        public void MoveFile(string sourcePath, string targetPath, RpiConfig rpiConfig)
        {

        }

        public void CopyDirectory(string sourcePath, string targetPath, RpiConfig rpiConfig, bool recursive = true)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException(sourcePath);
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            var dir = new DirectoryInfo(sourcePath);
            var files = dir.GetFiles(AllFilesFilter, new EnumerationOptions() { RecurseSubdirectories = recursive });
            foreach (var file in files)
            {
                string target = file.FullName.Replace(sourcePath, targetPath);
            }
        }

        public void MoveDirectory(string sourcePath, string targetPath, RpiConfig rpiConfig, bool recursive = true)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException(sourcePath);
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            var dir = new DirectoryInfo(sourcePath);
            var files = dir.GetFiles(AllFilesFilter, new EnumerationOptions() { RecurseSubdirectories = recursive });
            foreach (var file in files)
            {
                string target = file.FullName.Replace(sourcePath, targetPath);
            }
        }
    }
}
