using System;
using System.Windows.Input;

namespace PZ17.ViewModels;

public class Command<T> : ICommand {
    private readonly Action<T?> _action;
    private readonly Func<T?, bool>? _canExecute;

    public Command(Action<T?> action, Func<T?, bool>? canExecute = null) {
        _action = action;
        _canExecute = canExecute;
    }
    public bool CanExecute(object? parameter) {
        return _canExecute?.Invoke((T?)parameter) ?? true;
    }

    public void Execute(object? parameter) {
        _action.Invoke((T?)parameter);
    }

    public event EventHandler? CanExecuteChanged;
}