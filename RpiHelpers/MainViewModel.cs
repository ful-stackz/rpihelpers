﻿using RpiHelpers.Models;
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
        private const string EmptyTargetPath = "";
        private const string EmptyDropActionMessage = "Drop file(s) here for actions";

        private readonly DropDataService _dropDataService;
        private readonly RpiFileService _rpiFileService;
        private readonly RpiConnectionService _rpiConnectionService;
        private readonly SnackbarService _snackbarService;

        private bool _isClearEnabled;
        private bool _isDirectoryOptionsEnabled;
        private bool _isRecursiveEnabled;
        private bool _isInError;
        private string _targetPath = EmptyTargetPath;
        private string _dropActionMessage = EmptyDropActionMessage;
        private ObservableCollection<ActionButtonModel> _availableActions;
        private ObservableCollection<PayloadModel> _payloads = new ObservableCollection<PayloadModel>();
        private ICommand _clearCommand;
        private bool _isFileOptionsEnabled;

        public MainViewModel(
            RpiFileService rpiFileService,
            RpiConnectionService rpiConnectionService,
            DropDataService dropDataService,
            SnackbarService snackbarService)
        {
            _rpiFileService = rpiFileService ?? throw new ArgumentNullException(nameof(rpiFileService));
            _rpiConnectionService = rpiConnectionService;
            _rpiConnectionService.OnStatusChanged += HandleRpiConnectionStatusChanged;
            _dropDataService = dropDataService ?? throw new ArgumentNullException(nameof(dropDataService));
            dropDataService.OnDrop += OnDropHandler;
            _snackbarService = snackbarService ?? throw new ArgumentNullException(nameof(snackbarService));
            _snackbarService.OnMessage += HandleSnackbarChanged;
            _snackbarService.OnMessageExpired += HandleSnackbarChanged;

            _rpiConnectionService.StartService();
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

        public string SnackbarText =>
            _snackbarService.Message;

        public string TargetPath
        {
            get => _targetPath;
            set
            {
                if (_targetPath != value)
                {
                    _targetPath = string.IsNullOrEmpty(value) ? EmptyTargetPath : value;
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

        public bool IsDirectoryOptionsEnabled
        {
            get => _isDirectoryOptionsEnabled;
            set
            {
                if (_isDirectoryOptionsEnabled != value)
                {
                    _isDirectoryOptionsEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsFileOptionsEnabled
        {
            get => _isFileOptionsEnabled;
            set
            {
                if (_isFileOptionsEnabled != value)
                {
                    _isFileOptionsEnabled = value;
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

        public bool IsSnackbarVisible =>
            _snackbarService.HasMessage;

        public ObservableCollection<ActionButtonModel> AvailableActions
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
            AvailableActions = new ObservableCollection<ActionButtonModel>(new ActionButtonModel[]
            {
                new ActionButtonModel(caption: "Copy", action: CopySource),
                new ActionButtonModel(caption: "Move", action: MoveSource),
            });

            Payloads.Clear();
            e.FileNames
                .Select(
                    x => DropDataService.IsFilePath(x)
                        ? (PayloadModel)new FilePayloadModel(x, GetDirectoryOrFileName(x))
                        : (PayloadModel)new DirectoryPayloadModel(x, GetDirectoryOrFileName(x)))
                .ToList()
                .ForEach(Payloads.Add);

            const int MaxDisplayedPaths = 10;
            var dropActionMessage = string.Join(
                separator: Environment.NewLine,
                value: e.FileNames.ToArray(),
                startIndex: 0,
                count: e.FileNames.Count > MaxDisplayedPaths ? MaxDisplayedPaths : e.FileNames.Count);
            DropActionMessage = e.FileNames.Count > MaxDisplayedPaths
                ? dropActionMessage + $"{Environment.NewLine}{e.FileNames.Count - MaxDisplayedPaths} more"
                : dropActionMessage;

            IsDirectoryOptionsEnabled = e.HasOnlyDirectories || e.HasDirectoriesAndFiles;
            IsFileOptionsEnabled = e.HasOnlyFiles || e.HasDirectoriesAndFiles;
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

            var rpiConfig = IoC.Get<RpiConfigService>().RpiConfig;

            int totalPayloads = Payloads.Count;
            int processedPayloads = 0;

            foreach (var payload in Payloads)
            {
                if (payload.IsDirectory)
                {
                    _rpiFileService.CopyDirectory(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig);
                    _snackbarService.ShowMessage(
                        $"Directory {payload.Name} copied successfully! ({processedPayloads}/{totalPayloads})");
                }
                else if (payload.IsFile)
                {
                    _rpiFileService.CopyFile(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig);
                    _snackbarService.ShowMessage(
                        $"File {payload.Name} copied successfully! ({processedPayloads}/{totalPayloads})");
                }

                processedPayloads++;
            }

            _snackbarService.ShowMessage($"{totalPayloads} file(s) copied successfully!");
        }

        private void MoveSource()
        {
            if (Payloads.Count == 0)
            {
                return;
            }

            var rpiConfig = IoC.Get<RpiConfigService>().RpiConfig;

            int totalPayloads = Payloads.Count;
            int processedPayloads = 0;

            foreach (var payload in Payloads)
            {
                if (payload.IsDirectory)
                {
                    _rpiFileService.MoveDirectory(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig);
                    _snackbarService.ShowMessage(
                        $"Directory {payload.Name} moved successfully! ({processedPayloads}/{totalPayloads})");
                }
                else if (payload.IsFile)
                {
                    _rpiFileService.MoveFile(
                        sourcePath: payload.FullPath,
                        targetPath: TargetPath,
                        rpiConfig: rpiConfig);
                    _snackbarService.ShowMessage(
                        $"File {payload.Name} moved successfully! ({processedPayloads}/{totalPayloads})");
                }

                processedPayloads++;
            }

            _snackbarService.ShowMessage($"{totalPayloads} file(s) moved successfully!");
        }

        private void Clear()
        {
            AvailableActions.Clear();
            DropActionMessage = EmptyDropActionMessage;
            IsClearEnabled = false;
            IsDirectoryOptionsEnabled = false;
        }

        private void HandleRpiConnectionStatusChanged(object sender, EventArgs e)
        {
            _snackbarService.ShowMessage($"Raspberry Pi {(_rpiConnectionService.IsConnected ? "connected" : "disconnected")}!");
        }

        private void HandleSnackbarChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(nameof(IsSnackbarVisible));
            NotifyPropertyChanged(nameof(SnackbarText));
        }
    }
}
