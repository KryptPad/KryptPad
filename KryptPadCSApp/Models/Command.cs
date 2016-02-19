using System;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _action;


        #region Properties
        private bool _commandCanExecute;
        /// <summary>
        /// Gets or sets whether or not this command can execute
        /// </summary>
        public bool CommandCanExecute
        {
            get { return _commandCanExecute; }
            set
            {
                _commandCanExecute = value;
                // Raise event
                OnCanExecuteChanged();
            }
        }

        #endregion

        public Command()
        {
            _action = null;
            CommandCanExecute = true;
        }

        public Command(Action<object> action)
        {
            _action = action;
            CommandCanExecute = true;
        }

        public Command(Action<object> action, bool canExecute)
        {
            _action = action;
            CommandCanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return CommandCanExecute;
        }

        public void Execute(object parameter)
        {
            var handler = _action;
            if (handler != null)
            {
                handler(parameter);
            }
        }

        /// <summary>
        /// Raises CanExecuteChanged event
        /// </summary>
        private void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}