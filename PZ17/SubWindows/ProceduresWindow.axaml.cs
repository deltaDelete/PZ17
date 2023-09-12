using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PZ17.ViewModels;

namespace PZ17.SubWindows;

public partial class ProceduresWindow : Window
{
    public ProceduresWindow()
    {
        InitializeComponent();
        DataContext = new ProceduresWindowViewModel();
    }
}