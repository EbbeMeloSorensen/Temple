using Craft.DataStructures.IO;
using Newtonsoft.Json;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.Dialogues.DialogueGraphConditions;
using Temple.Infrastructure.Dialogues.GameEventTriggers;

namespace Temple.Infrastructure.IO
{
    public static class DialogueIO
    {
        public static IEnumerable<DialogueGraph> ReadDialogueGraphListFromFile(
            string fileName)
        {
            using var streamReader = new StreamReader(fileName);
            var json = streamReader.ReadToEnd();

            var jsonResolver = new IgnoreVertexCountResolver();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = jsonResolver,
                Converters =
                {
                    new GraphJsonConverter()
                },
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new KnownTypesBinder
                {
                    KnownTypes = new[]
                    {
                        typeof(FactEstablishedCondition),
                        typeof(QuestStatusCondition),
                        typeof(FactEstablishedEventTrigger),
                        typeof(KnowledgeGainedEventTrigger),
                        typeof(QuestDiscoveredEventTrigger),
                        typeof(QuestAcceptedEventTrigger),
                        typeof(SiteUnlockedEventTrigger)
                    }
                }
            };

            return JsonConvert.DeserializeObject<List<DialogueGraph>>(json, settings);
        }

        public static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var jsonResolver = new IgnoreVertexCountResolver();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = jsonResolver,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new KnownTypesBinder
                {
                    KnownTypes = new[]
                    {
                        typeof(FactEstablishedCondition),
                        typeof(QuestStatusCondition),
                        typeof(FactEstablishedEventTrigger),
                        typeof(KnowledgeGainedEventTrigger),
                        typeof(QuestDiscoveredEventTrigger),
                        typeof(QuestAcceptedEventTrigger),
                        typeof(SiteUnlockedEventTrigger)
                    }
                }
            };

            return settings;
        }
    }
}
