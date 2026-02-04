using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Temple.Infrastructure.Dialogues;

public class IgnoreVertexCountResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(
        Type type,
        MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization)
            .Where(p => p.PropertyName != "VertexCount")
            .ToList();
    }
}