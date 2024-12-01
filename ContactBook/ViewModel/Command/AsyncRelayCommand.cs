using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContactsBook
{
    public class AsyncRelayCommand : ICommand
    {
        protected ViewModel viewModel;
        private readonly Func<object?, Task> executeAsync;
        private readonly Func<object?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null)
        {
            this.executeAsync = executeAsync;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public async void Execute(object? parameter)
        {
            await executeAsync(parameter);
        }
    }
}
