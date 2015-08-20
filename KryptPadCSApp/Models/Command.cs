using System;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _action;
        //private Func<object, bool> _canExecute;

        #region Properties
        private bool _commandCanExecute;

        public bool CommandCanExecute
        {
            get { return _commandCanExecute; }
            set {
                _commandCanExecute = value;
                //raise event
                OnCanExecuteChanged();
            }
        }

        #endregion

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
            if (_action != null)
            {
                _action(parameter);
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