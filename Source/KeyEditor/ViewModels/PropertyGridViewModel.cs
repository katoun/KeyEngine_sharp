using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;

namespace KeyEditor.ViewModels;
using Tools;

public class PropertySection
{
    public string Title { get; }
    public ObservableCollection<PropertyItem> Properties { get; }

    public PropertySection(string title, IEnumerable<PropertyItem> items)
    {
        Title = title;
        Properties = new ObservableCollection<PropertyItem>(items);
    }
}

public class PropertyItem
{
    public string Key { get; set; }
    public string Value { get; set; }

    public PropertyItem(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

public class PropertyGridViewModel : ToolViewModel
{
    public ObservableCollection<PropertySection> Sections { get; set; }
    
    public PropertyGridViewModel()
    {
        Title = "Property Grid";

        Sections = new ObservableCollection<PropertySection>
        {
            new PropertySection("Transform", new[]
            {
                new PropertyItem("Position", "(0, 0, 0)"),
                new PropertyItem("Rotation", "(0, 0, 0)"),
                new PropertyItem("Scale", "(1, 1, 1)")
            }),
            new PropertySection("Rendering", new[]
            {
                new PropertyItem("Material", "Standard"),
                new PropertyItem("Visible", "True")
            })
        };
    }
}