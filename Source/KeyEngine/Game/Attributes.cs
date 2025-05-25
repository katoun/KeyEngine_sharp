using System;

namespace KeyEngine.Game;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AllowMultipleComponent : Attribute
{
	public readonly bool Allow = true;

	public AllowMultipleComponent() { }
	public AllowMultipleComponent(bool allow)
	{
		Allow = allow;
	}
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequireComponent : Attribute
{
	public readonly Type ComponentType = null;

	public RequireComponent(Type type)
	{
		ComponentType = type;
	}
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EditorCategoryAttribute : Attribute
{
	public readonly string Category = string.Empty;

	public EditorCategoryAttribute(string category)
	{
		Category = category;
	}
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EditorIconAttribute : Attribute
{
	public readonly string Name = string.Empty;

	public EditorIconAttribute(string name)
	{
		Name = name;
	}
}
