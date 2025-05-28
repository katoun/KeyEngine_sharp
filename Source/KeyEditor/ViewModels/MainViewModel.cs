using System.Collections.Generic;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;

namespace KeyEditor.ViewModels;
using Documents;
using Tools;

public class MainViewModel
{
    private readonly IFactory? _factory;
    private IRootDock? _layout;
    
    public IRootDock? Layout
    {
        get => _layout;
        set => _layout = value;
    }

    public MainViewModel()
    {
        _factory = new DockFactory();
        
        _layout = _factory?.CreateLayout();
    }
    
    public void CloseLayout()
    {
        if (_layout is IDock dock)
        {
            if (dock.Close.CanExecute(null))
            {
                dock.Close.Execute(null);
            }
        }
    }
}