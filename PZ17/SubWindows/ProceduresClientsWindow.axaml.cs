using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZ17.ViewModels;

namespace PZ17.SubWindows;

public partial class ProceduresClientsWindow : Window
{
    public ProceduresClientsWindow()
    {
        InitializeComponent();
        DataContext = new ProceduresClientsWindowViewModel();
    }
}