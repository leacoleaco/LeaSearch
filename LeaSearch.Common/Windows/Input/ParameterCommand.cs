using System;
using System.Windows.Input;

namespace LeaSearch.Common.Windows.Input
{
    public class ParameterCommand : ICommand
    {

        private readonly Action<object> _action;

        public ParameterCommand(Action<object> action)
        {
            _action = action;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }
    }
}
