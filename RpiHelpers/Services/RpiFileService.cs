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
        private const string CopyCommand = "scp";
        private const string AllFilesFilter = "*";
        private readonly CommandExecutor _cmdExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpiFileService"/> class.
        /// </summary>
        /// <param name="commandExecutor">
        /// Used to execute shell commands.
        /// </param>
        public RpiFileService(CommandExecutor commandExecutor)
        {
            _cmdExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        }

        /// <summary>
        /// Copies a file from the specified <paramref name="sourcePath"/> to the specified
        /// <paramref name="targetPath"/> on the Raspberry Pi device with the specified
        /// <paramref name="rpiConfig"/>.
        /// </summary>
        /// <param name="sourcePath">
        /// The path to the file to be copied.
        /// </param>
        /// <param name="targetPath">
        /// The path to which the file will be copied.
        /// </param>
        /// <param name="rpiConfig">
        /// The configuration of the device to which the file will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// When one or more of the required parameters are <c>null</c>.
        /// </exception>
        public void CopyFile(string sourcePath, string targetPath, RpiConfig rpiConfig)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException(sourcePath);
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            var source = EscapeWindowsPath(sourcePath);
            var target = EscapeUnixPath($"{rpiConfig.ConnectionString}:~/{targetPath}");

            _cmdExecutor.Execute($"{CopyCommand} {source} {target}");
        }

        /// <summary>
        /// Moves a file from the specified <paramref name="sourcePath"/> to the specified
        /// <paramref name="targetPath"/> on the Raspberry Pi device with the specified
        /// <paramref name="rpiConfig"/>. The local file with path <paramref name="sourcePath"/>
        /// will be deleted.
        /// </summary>
        /// <param name="sourcePath">
        /// The path to the file to be moved.
        /// </param>
        /// <param name="targetPath">
        /// The path to which the file will be moved.
        /// </param>
        /// <param name="rpiConfig">
        /// The configuration of the device to which the file will be moved.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// When one or more of the required parameters are <c>null</c>.
        /// </exception>
        public void MoveFile(string sourcePath, string targetPath, RpiConfig rpiConfig)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }
        }

        /// <summary>
        /// Copies a directory and all its subdirectories and files from the specified <paramref name="sourcePath"/>
        /// to the specified <paramref name="targetPath"/> on the Raspberry Pi device with the
        /// specified <paramref name="rpiConfig"/>.
        /// </summary>
        /// <param name="sourcePath">
        /// The path to the directory to be copied.
        /// </param>
        /// <param name="targetPath">
        /// The path to which the directory will be copied.
        /// </param>
        /// <param name="rpiConfig">
        /// The configuration of the device to which the directory will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// When one or more of the required parameters are <c>null</c>.
        /// </exception>
        public void CopyDirectory(string sourcePath, string targetPath, RpiConfig rpiConfig)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentNullException(sourcePath);
            }

            if (string.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentNullException(nameof(targetPath));
            }

            var source = EscapeWindowsPath(sourcePath);
            var target = EscapeUnixPath($"{rpiConfig.ConnectionString}:~/{targetPath}");

            _cmdExecutor.Execute($"{CopyCommand} -r {source} {target}");
        }

        /// <summary>
        /// Moves a directory from the specified <paramref name="sourcePath"/> to the specified
        /// <paramref name="targetPath"/> with all of its subdirectories and files to the
        /// specifeid <paramref name="targetPath"/> on the Rasperry Pi device with the
        /// specified <paramref name="rpiConfig"/>.
        /// </summary>
        /// <param name="sourcePath">
        /// The path to the directory to be moved.
        /// </param>
        /// <param name="targetPath">
        /// The path to which the directory will be moved.
        /// </param>
        /// <param name="rpiConfig">
        /// The configuration of the device to which the directory will be moved.
        /// </param>
        /// <remarks>
        /// This operation will delete the directory and all subdirectories and files from <paramref name="sourcePath"/>.
        /// </remarks>
        public void MoveDirectory(string sourcePath, string targetPath, RpiConfig rpiConfig)
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
            var files = dir.GetFiles(AllFilesFilter, new EnumerationOptions() { RecurseSubdirectories = true });
            foreach (var file in files)
            {
                string target = file.FullName.Replace(sourcePath, targetPath);
            }
        }

        private string EscapeWindowsPath(string path)
        {
            string result = path.Trim().Replace('/', '\\');
            if (result.Contains(' '))
            {
                result = $"\"{result}\"";
            }

            return result;
        }

        private string EscapeUnixPath(string input)
        {
            var result = input.Trim().Replace('\\', '/');
            if (result.Contains(' '))
            {
                result = $"\"{result}\"";
            }

            return result;
        }
    }
}
