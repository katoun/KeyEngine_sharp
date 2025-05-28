using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Dock.Avalonia.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;

namespace KeyEditor;
using ViewModels;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var mainWindowViewModel = new MainViewModel();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            
            mainWindow.Closing += (_, _) =>
            {
                mainWindowViewModel.CloseLayout();
            };
            
            desktopLifetime.MainWindow = mainWindow;
            
            desktopLifetime.Exit += (_, _) =>
            {
                mainWindowViewModel.CloseLayout();
            };
        }

        base.OnFrameworkInitializationCompleted();
#if DEBUG
        this.AttachDevTools();
#endif        
    }
}