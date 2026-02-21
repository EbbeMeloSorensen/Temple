using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.Dialogues;

namespace Temple.Infrastructure.IO;

public class SiteComponentCountResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(
        Type type,
        MemberSerialization memberSerialization)
    {
        return base.CreateProperties(type, memberSerialization)
            .Where(p => p.PropertyName != "Length")
            .ToList();
    }
}

public static class SiteDataIO
{
    public static void WriteToFile(
        this IEnumerable<ISiteComponent> siteComponents,
        string fileName)
    {
        var json = JsonConvert.SerializeObject(
            siteComponents,
            Formatting.Indented,
            GetJsonSerializerSettings());

        using var streamWriter = new StreamWriter(fileName);

        streamWriter.WriteLine(json);
    }

    public static IEnumerable<ISiteComponent> ReadSiteComponentListFromFile(
        string fileName)
    {
        using var streamReader = new StreamReader(fileName);
        var json = streamReader.ReadToEnd();
        var settings = GetJsonSerializerSettings();

        return JsonConvert.DeserializeObject<List<ISiteComponent>>(json, settings);
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            //Culture = CultureInfo.InvariantCulture,
            ContractResolver = new SiteComponentCountResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new KnownTypesBinder
            {
                KnownTypes = new[]
                {
                    typeof(Quad),
                    typeof(Temple.Domain.Entities.DD.Exploration.Barrier),
                }
            }
        };

        //if (mode == DialogueIOMode.Read)
        //{
        //    settings.Converters = new List<JsonConverter>
        //    {
        //        new GraphJsonConverter()
        //    };
        //}

        return settings;
    }
}

