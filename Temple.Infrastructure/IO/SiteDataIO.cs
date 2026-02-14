using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Temple.Domain.Entities.DD.Exploration;

namespace Temple.Infrastructure.IO;

public static class SiteDataIO
{
    public static void WriteToFile(
        this SiteData siteData,
        string fileName)
    {
        var json = JsonConvert.SerializeObject(
            siteData,
            Formatting.Indented,
            GetJsonSerializerSettings());

        using var streamWriter = new StreamWriter(fileName);

        streamWriter.WriteLine(json);
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            //TypeNameHandling = TypeNameHandling.Auto,
            //SerializationBinder = new KnownTypesBinder
            //{
            //    KnownTypes = new[]
            //    {
            //        typeof(FactEstablishedCondition),
            //        typeof(QuestStatusCondition),
            //        typeof(BattleWonCondition),
            //        typeof(AndGameCondition),
            //        typeof(OrGameCondition),
            //        typeof(FactEstablishedEventTrigger),
            //        typeof(KnowledgeGainedEventTrigger),
            //        typeof(QuestDiscoveredEventTrigger),
            //        typeof(QuestAcceptedEventTrigger),
            //        typeof(SiteUnlockedEventTrigger)
            //    }
            //}
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

