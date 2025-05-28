using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KeyEditor.Views;

public partial class PropertyGridView : UserControl
{
    public PropertyGridView()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}