using System.Collections.Generic;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;

namespace KeyEditor.ViewModels;
using Documents;
using Tools;

public class DockFactory : Factory
{
    public override IRootDock CreateLayout()
    {
        var left = new ToolDock
        {
            Id = "solution",
            Title = "Solution",
            VisibleDockables = new IDockable[]
            {
                new ToolViewModel { Title = "Solution Panel" }
            }
        };

        var right = new ToolDock
        {
            Id = "properties",
            Title = "Properties",
            VisibleDockables = new IDockable[]
            {
                new PropertyGridViewModel()
            }
        };

        var documents = new DocumentDock
        {
            Id = "documents",
            Title = "Documents",
            VisibleDockables = new IDockable[]
            {
                new DocumentViewModel { Title = "Document 1" },
                new DocumentViewModel { Title = "Document 2" }
            },
            ActiveDockable = null
        };

        var layout = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = new List<IDockable>
            {
                left,
                new ProportionalDock
                {
                    Orientation = Orientation.Vertical,
                    VisibleDockables = new List<IDockable>
                    {
                        documents,
                        right
                    }
                }
            }
        };

        var root = new RootDock
        {
            Id = "root",
            Title = "Root",
            VisibleDockables = new IDockable[] { layout },
            ActiveDockable = layout,
            DefaultDockable = documents
        };

        return root;
    }
}