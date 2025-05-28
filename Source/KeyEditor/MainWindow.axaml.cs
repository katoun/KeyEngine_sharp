using Avalonia.Controls;

namespace KeyEditor;

using ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        DataContext = new MainViewModel();
    }
}