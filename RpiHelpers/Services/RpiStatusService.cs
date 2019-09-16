using RpiHelpers.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace RpiHelpers.Services
{
    internal class RpiConnectionService
    {
        private static readonly TimeSpan ConnectionCheckInterval = TimeSpan.FromSeconds(2);

        private readonly CommandExecutor _commandExecutor;
        private readonly Thread _checkingThread;

        private bool _isConnected;

        public event EventHandler OnStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpiConnectionService"/> class.
        /// </summary>
        /// <param name="commandExecutor">
        /// Used to execute CLI commands.
        /// </param>
        public RpiConnectionService(CommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
            _checkingThread = new Thread(CheckConnectionStatus)
            {
                IsBackground = true,
                Name = "RPi Connection Status Thread",
                Priority = ThreadPriority.Normal
            };
        }

        ~RpiConnectionService()
        {
            StopService();
        }

        /// <summary>
        /// Gets a value indicating whether the Raspberry Pi device is connected.
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Starts checking whether the Raspberry Pi device is connected and notifying consumers.
        /// </summary>
        public void StartService()
        {
            if (!_checkingThread.IsAlive)
            {
                _checkingThread.Start();
            }
        }

        /// <summary>
        /// Stops checking whether the Raspberry Pi device is connected.
        /// </summary>
        public void StopService()
        {
            if (_checkingThread.IsAlive)
            {
                // It's safe to abort since no information can be lost
                _checkingThread.Abort();
            }
        }

        private void CheckConnectionStatus()
        {
            const string PingCommand = "ping raspberrypi.local -n 2";
            const int SentPackagesIndex = 1;
            const int ReceivedPackagesIndex = 2;
            var getPingStatsRegex = new Regex(@"Sent = (\d), Received = (\d)");

            while (true)
            {
                var result = _commandExecutor.Execute(PingCommand);
                var pingStats = getPingStatsRegex.Match(result);

                bool isConnected = false;

                if (pingStats.Success)
                {
                    int sent = int.Parse(pingStats.Groups[SentPackagesIndex].Value);
                    int received = int.Parse(pingStats.Groups[ReceivedPackagesIndex].Value);
                    isConnected = sent == received;
                }

                if (_isConnected != isConnected)
                {
                    _isConnected = isConnected;
                    OnStatusChanged?.Invoke(this, EventArgs.Empty);
                }

                Thread.Sleep(ConnectionCheckInterval);
            }
        }
    }
}
