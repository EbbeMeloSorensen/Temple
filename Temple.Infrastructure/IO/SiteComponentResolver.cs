using Craft.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Temple.Infrastructure.IO;

public class SiteComponentResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(
        Type type,
        MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);

        if (type == typeof(Vector3D))
        {
            properties = properties
                .Where(p => p.PropertyName != "Length")
                .ToList();
        }

        return properties;
    }
}