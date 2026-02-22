using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Temple.Infrastructure.IO;

public class QuestResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(
        Type type,
        MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization)
            .Where(p => p.PropertyName != "State")
            .Where(p => p.PropertyName != "AreCompletionCriteriaSatisfied")
            .ToList();
    }
}