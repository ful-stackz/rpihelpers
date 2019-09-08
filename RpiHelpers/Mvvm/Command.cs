using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RpiHelpers.Mvvm
{
    class Command : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public Command(Action<object> action, Func<object, bool> canExecute)
        {
            this._action = action;
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter) =>
            _canExecute(parameter);

        public void Execute(object parameter) =>
            _action(parameter);
    }
}
