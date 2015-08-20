using System;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    internal class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<object> _action;

        public Command(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_action != null)
            {
                _action(parameter);
            }
        }
    }
}