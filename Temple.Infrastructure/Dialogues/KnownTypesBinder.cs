using Newtonsoft.Json.Serialization;

namespace Temple.Infrastructure.Dialogues;

public class KnownTypesBinder : ISerializationBinder
{
    public IList<Type> KnownTypes { get; set; }

    public Type BindToType(string assemblyName, string typeName)
        => KnownTypes.Single(t => t.FullName == typeName);

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.FullName;
    }
}