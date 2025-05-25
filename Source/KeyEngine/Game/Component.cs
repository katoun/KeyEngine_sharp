using System;
using System.Collections.Generic;

namespace KeyEngine.Game;

[EditorIcon(Core.ResourceNames.IconComponent)]
public class Component : Core.IIdentifiable
{
	protected Guid id;
    private bool enabled;

    protected Entity entity;

	public Guid Id => id;

	public Component()
    {
        id = Guid.NewGuid();
        enabled = true;
    }

    internal class Serializer: Serialization.Serializer<Component>
	{
		public override void Serialize(ref Component obj, Serialization.ArchiveMode mode)
		{
			var id = obj.Id;
			//TODO!!!
		}
	}
}
