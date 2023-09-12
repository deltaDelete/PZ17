using Avalonia.Controls;
using Avalonia.Interactivity;
using PZ17.SubWindows;

namespace PZ17;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ClientsButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var window = new ClientsWindow();
        await window.ShowDialog(this);
    }
    private async void ProceduresButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var window = new ProceduresWindow();
        await window.ShowDialog(this);
    }
    private async void ProceduresClientsButtonOnClick(object? sender, RoutedEventArgs e)
    {
        var window = new ProceduresClientsWindow();
        await window.ShowDialog(this);
    }
}