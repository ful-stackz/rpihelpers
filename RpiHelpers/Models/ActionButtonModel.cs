using RpiHelpers.Mvvm;
using System;
using System.Windows.Input;

namespace RpiHelpers.Models
{
    class ActionButtonModel
    {
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionButtonModel"/> class.
        /// </summary>
        /// <param name="caption">
        /// The caption of the button.
        /// </param>
        /// <param name="action">
        /// The action to be executed when the button is activated.
        /// </param>
        public ActionButtonModel(string caption, Action action)
        {
            Caption = caption ?? throw new ArgumentNullException(nameof(caption));
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// Gets the caption of the button.
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// Gets the command to be executed when the button is activated.
        /// </summary>
        public ICommand Execute =>
            new Command(_ => _action(), _ => true);
    }
}
