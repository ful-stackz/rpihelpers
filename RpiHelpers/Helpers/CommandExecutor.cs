using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RpiHelpers.Helpers
{
    internal class CommandExecutor
    {
        private readonly Process _cmdProcess;

        public CommandExecutor()
        {
            _cmdProcess = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd.exe")
                {
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                },
            };

            _cmdProcess.Start();
        }

        public string Execute(string command)
        {
            if (!_cmdProcess.Start())
            {
                throw new InvalidOperationException();
            }

            _cmdProcess.StandardInput.WriteLine(command);
            _cmdProcess.StandardInput.Flush();
            _cmdProcess.StandardInput.Close();
            _cmdProcess.WaitForExit();

            return _cmdProcess.StandardOutput.ReadToEnd();
        }
    }
}
