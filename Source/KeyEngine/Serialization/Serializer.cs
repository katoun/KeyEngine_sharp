namespace KeyEngine.Serialization;

public enum ArchiveMode
{
	Serialize,
	Deserialize
}

public abstract class Serializer<T>
{
	Type serializationType => typeof(T);

	public abstract void Serialize(ref T obj, ArchiveMode mode);
}
