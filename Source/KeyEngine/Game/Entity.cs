using System.Diagnostics;

namespace KeyEngine.Game;

[DebuggerTypeProxy(typeof(EntityDebugView))]
[EditorIcon(Core.ResourceNames.IconEntity)]
public class Entity : Core.IIdentifiable
{
    private string name;
    private Guid id;
	private bool enabled;

	Entity parent;
    List<Entity> children = [];
	List<Component> components = [];

	public string Name
	{
		get { return name; }
		set { name = value; }
	}

	public Guid Id => id;

	public Entity(string name)
	{
		this.name = name;
		id = Guid.NewGuid();
        enabled = true;
	}

    internal class EntityDebugView(Entity entity)
    {
        private readonly Entity entity = entity;

        public string Name => entity.name;

        public Guid Id => entity.id;

        public bool Enabled => entity.enabled;

        public Entity Parent => entity.parent;

        public Entity[] Children => [.. entity.children];

        public Component[] Components => [.. entity.components];
    }
}
