using RpiHelpers.Models;
using RpiHelpers.Mvvm;
using RpiHelpers.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace RpiHelpers
{
    class MainViewModel : BaseViewModel
    {
        private const string EmptyDropActionMessage = "Drop files here for actions";
        private static readonly TimeSpan SnackbarTimerPeriod = TimeSpan.FromSeconds(2);

        private readonly RpiFileService _rpiFileService;
        private readonly Timer _snackbarTimer;

        private bool _isClearEnabled;
        private bool _isDirectorySelected;
        private bool _isRecursiveEnabled;
        private bool _isInError;
        private string _targetPath;
        private string _dropActionMessage = EmptyDropActionMessage;
        private ObservableCollection<ActionButton> _availableActions;
        private ObservableCollection<PayloadModel> _payloads = new ObservableCollection<PayloadModel>();
        private ICommand _clearCommand;
        private bool _isSnackbarVisible;
        private string _snackbarText;

        public MainViewModel()
        {
            IoT.Get<DropDataService>().OnDrop += OnDropHandler;
            _rpiFileService = IoT.Get<RpiFileService>();
            _snackbarTimer = new Timer(SnackbarHandleTick, null, SnackbarTimerPeriod, SnackbarTimerPeriod);
        }

        public string WindowTitle { get; } = "Raspberry Pi Helpers";

        public string DropActionMessage
        {
            get => _dropActionMessage;
            private set
            {
                if (_dropActionMessage != value)
                {
                    _dropActionMessage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SnackbarText
        {
            get => _snackbarText;
            set
            {
                if (_snackbarText != value)
                {
                    _snackbarText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string TargetPath
        {
            get => _targetPath;
            set
            {
                if (_targetPath != value)
                {
                    _targetPath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new Command(
                        action: _ => Clear(),
                        canExecute: _ => true);
                }

                return _clearCommand;
            }
        }

        public bool AnyActionsAvailable
        {
            get => AvailableActions?.Count > 0;
        }

        public bool IsInError
        {
            get => _isInError;
            set
            {
                if (_isInError != value)
                {
                    _isInError = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsClearEnabled
        {
            get => _isClearEnabled;
            set
            {
                if (_isClearEnabled != value)
                {
                    _isClearEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsDirectorySelected
        {
            get => _isDirectorySelected;
            set
            {
                if (_isDirectorySelected != value)
                {
                    _isDirectorySelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsRecursiveChecked
        {
            get => _isRecursiveEnabled;
            set
            {
                _isRecursiveEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSnackbarVisible
        {
            get => _isSnackbarVisible;
            set
            {
                if (_isSnackbarVisible != value)
                {
                    _isSnackbarVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<ActionButton> AvailableActions
        {
            get => _availableActions;
            set
            {
                _availableActions = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<PayloadModel> Payloads
        {
            get => _payloads;
            set
            {
                if (_payloads != value)
                {
                    _payloads = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void OnDropHandler(object sender, DropDataEventArgs e)
        {
            string target = e.IsDirectory ? "directory" : "file";
            AvailableActions = new ObservableCollection<ActionButton>(new ActionButton[]
            {
                new ActionButton(
                    caption: $"Copy {target}",
                    action: CopySource),
                new ActionButton($"Move {target}", MoveSource),
            });

            Payloads.Clear();
            if (e.IsDirectory)
            {
                Payloads.Add(new DirectoryPayloadModel(e.FileNames[0], GetDirectoryOrFileName(e.FileNames[0])));
            }
            else
            {
                e.FileNames
                    .ToList()
                    .ForEach(
                        f => Payloads.Add(
                            new FilePayloadModel(f, GetDirectoryOrFileName(f))));
            }

            DropActionMessage = e.IsDirectory
                ? GetDirectoryOrFileName(e.FileNames[0])
                : string.Join(Environment.NewLine, e.FileNames);

            IsDirectorySelected = e.IsDirectory;
            IsClearEnabled = true;

            NotifyPropertyChanged(nameof(AnyActionsAvailable));

            static string GetDirectoryOrFileName(string fullPath) =>
                fullPath.Split('\\')[^1];
        }

        private void CopySource()
        {
            if (Payloads.Count == 0)
            {
                return;
            }

            var rpiConfig = IoT.Get<RpiConfigService>().RpiConfig;

            foreach (var payload in Payloads)
            {
                if (payload.IsDirectory)
                {
                    _rpiFileService.CopyDirectory(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig,
                        recursive: IsRecursiveChecked);
                    ShowSnackbarMessage($"Directory {payload.Name} copied successfully!");
                }
                else if (payload.IsFile)
                {
                    _rpiFileService.CopyFile(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig);
                    ShowSnackbarMessage($"File {payload.Name} copied successfully!");
                }
            }

            ShowSnackbarMessage($"Copy operation successful!");
        }

        private void MoveSource()
        {
            if (Payloads.Count == 0)
            {
                return;
            }

            var rpiConfig = IoT.Get<RpiConfigService>().RpiConfig;

            foreach (var payload in Payloads)
            {
                if (payload.IsDirectory)
                {
                    _rpiFileService.MoveDirectory(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig,
                        recursive: IsRecursiveChecked);
                }
                else if (payload.IsFile)
                {
                    _rpiFileService.MoveFile(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig);
                    ShowSnackbarMessage($"File {payload.Name} moved successfully!");
                }
            }
        }

        private void Clear()
        {
            AvailableActions.Clear();
            DropActionMessage = EmptyDropActionMessage;
            IsClearEnabled = false;
            IsDirectorySelected = false;
        }

        private void ShowSnackbarMessage(string message)
        {
            SnackbarText = message;
            IsSnackbarVisible = true;
            _snackbarTimer.Change(SnackbarTimerPeriod, SnackbarTimerPeriod);
        }

        private void SnackbarHandleTick(object state)
        {
            SnackbarText = string.Empty;
            IsSnackbarVisible = false;
        }

        internal class ActionButton
        {
            public ActionButton(string caption, Action action)
            {
                Caption = caption ?? throw new ArgumentNullException(nameof(caption));
                Action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public Action Action { get; }
            public string Caption { get; }
            public ICommand Execute =>
                new Command(_ => Action(), _ => true);
        }
    }
}
