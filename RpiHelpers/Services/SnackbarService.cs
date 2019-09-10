using System;
using System.Threading;

namespace RpiHelpers.Services
{
    class SnackbarService : IDisposable
    {
        private static readonly TimeSpan SnackbarTimerPeriod = TimeSpan.FromSeconds(2);

        private readonly Timer _snackbarTimer;

        private bool _isDisposed;

        /// <summary>
        /// Raised when the message provided by <see cref="SnackbarService"/> has been changed.
        /// </summary>
        public event EventHandler OnMessage;

        /// <summary>
        /// Raised when the duration of the shown message has expired.
        /// </summary>
        public event EventHandler OnMessageExpired;

        /// <summary>
        /// Gets a value indicating whether a message is available.
        /// </summary>
        public bool HasMessage => Message != null;

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnackbarService"/> class.
        /// </summary>
        /// <param name="timerFactory">
        /// Creates a new timer instance.
        /// </param>
        public SnackbarService(Func<Action, TimeSpan, Timer> timerFactory)
        {
            _snackbarTimer = timerFactory?.Invoke(HandleTimerTick, SnackbarTimerPeriod) ??
                throw new ArgumentNullException(nameof(timerFactory));
        }

        /// <summary>
        /// Releases all resources held by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Changes the message provided by <see cref="SnackbarService"/> to the
        /// specified <paramref name="message"/> and notifies all consumers of the update.
        /// </summary>
        /// <param name="message">
        /// The new message that is to be provided by <see cref="SnackbarService"/>.
        /// </param>
        public void ShowMessage(string message)
        {
            Message = message;
            _snackbarTimer.Change(
                dueTime: SnackbarTimerPeriod,
                period: SnackbarTimerPeriod);
            OnMessage?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    _snackbarTimer.Dispose();
                }

                _isDisposed = true;
            }
        }

        private void HandleTimerTick()
        {
            Message = null;
            OnMessageExpired?.Invoke(this, EventArgs.Empty);
        }
    }
}
