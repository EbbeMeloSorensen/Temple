using Newtonsoft.Json;
using System.Globalization;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Rules;
using Temple.Infrastructure.Dialogues;

namespace Temple.Infrastructure.IO;

public static class QuestIO
{
    public static void WriteToFile(
        this IEnumerable<Quest> siteComponents,
        string fileName)
    {
        var json = JsonConvert.SerializeObject(
            siteComponents,
            Formatting.Indented,
            GetJsonSerializerSettings());

        using var streamWriter = new StreamWriter(fileName);

        streamWriter.WriteLine(json);
    }

    public static IEnumerable<Quest> ReadQuestListFromFile(
        string fileName)
    {
        using var streamReader = new StreamReader(fileName);
        var json = streamReader.ReadToEnd();
        var settings = GetJsonSerializerSettings();

        return JsonConvert.DeserializeObject<List<Quest>>(json, settings);
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            Culture = CultureInfo.InvariantCulture,
            ContractResolver = new QuestResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new KnownTypesBinder
            {
                KnownTypes = new[]
                {
                    typeof(AcceptQuestRule),
                    typeof(AdvanceOnCheatRule),
                    typeof(BecomeAvailableOnAreaEnterRule),
                    typeof(BecomeAvailableOnDialogueRule),
                    typeof(BecomeAvailableOnQuestDiscoveredRule),
                    typeof(SatisfyOnBattleWonRule),
                    typeof(SatisfyOnDialogueRule),
                    typeof(TurnInOnDialogueRule)
                }
            }
        };

        return settings;
    }
}