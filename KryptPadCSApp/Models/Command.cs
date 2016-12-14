using System;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _action;
        private Func<object, bool> _canExecute;

        //#region Properties
        //private bool _commandCanExecute;
        ///// <summary>
        ///// Gets or sets whether or not this command can execute
        ///// </summary>
        //public bool CommandCanExecute
        //{
        //    get { return _commandCanExecute; }
        //    set
        //    {
        //        _commandCanExecute = value;
        //        // Raise event
        //        OnCanExecuteChanged();
        //    }
        //}

        //#endregion

        public Command()
        {
            _action = null;
            _canExecute = CanExecuteHandler;
        }

        public Command(Action<object> action)
        {
            _action = action;
            _canExecute = CanExecuteHandler;
        }

        public Command(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }

        /// <summary>
        /// Raises CanExecuteChanged event
        /// </summary>
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Default handler
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CanExecuteHandler(object p)
        {
            return true;
        }
    }
}