using System.Globalization;
using Newtonsoft.Json;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.GameConditions;

namespace Temple.Infrastructure.IO;

public static class SiteDataIO
{
    public static void WriteSiteComponentsToFile(
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
            Culture = CultureInfo.InvariantCulture,
            ContractResolver = new SiteComponentResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new KnownTypesBinder
            {
                KnownTypes = new[]
                {
                    typeof(KnowledgeGainedCondition),
                    typeof(FactEstablishedCondition),
                    typeof(QuestStatusCondition),
                    typeof(BattleWonCondition),
                    typeof(AndGameCondition),
                    typeof(OrGameCondition),
                    typeof(NotGameCondition),
                    typeof(Quad),
                    typeof(Cylinder),
                    typeof(Sphere),
                    typeof(NPC),
                    typeof(Temple.Domain.Entities.DD.Exploration.Barrier),
                    typeof(EventTrigger_LeaveSite),
                    typeof(EventTrigger_ScriptedBattle)
                }
            }
        };

        return settings;
    }
}

