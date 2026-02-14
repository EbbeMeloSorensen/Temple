using Craft.DataStructures.IO;
using Newtonsoft.Json;
using System;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.Dialogues.GameEventTriggers;
using Temple.Infrastructure.GameConditions;

namespace Temple.Infrastructure.IO
{
    public enum DialogueIOMode
    {
        Read,
        Write
    }

    public static class DialogueIO
    {
        public static void WriteToFile(
            this IEnumerable<DialogueGraph> graph,
            string fileName)
        {
            var json = JsonConvert.SerializeObject(
                graph,
                Formatting.Indented,
                GetJsonSerializerSettings(DialogueIOMode.Write));

            using var streamWriter = new StreamWriter(fileName);

            streamWriter.WriteLine(json);
        }

        public static IEnumerable<DialogueGraph> ReadDialogueGraphListFromFile(
            string fileName)
        {
            using var streamReader = new StreamReader(fileName);
            var json = streamReader.ReadToEnd();
            var settings = GetJsonSerializerSettings(DialogueIOMode.Read);

            return JsonConvert.DeserializeObject<List<DialogueGraph>>(json, settings);
        }

        private static JsonSerializerSettings GetJsonSerializerSettings(
            DialogueIOMode mode)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreVertexCountResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new KnownTypesBinder
                {
                    KnownTypes = new[]
                    {
                        typeof(FactEstablishedCondition),
                        typeof(QuestStatusCondition),
                        typeof(BattleWonCondition),
                        typeof(AndGameCondition),
                        typeof(OrGameCondition),
                        typeof(FactEstablishedEventTrigger),
                        typeof(KnowledgeGainedEventTrigger),
                        typeof(QuestDiscoveredEventTrigger),
                        typeof(QuestAcceptedEventTrigger),
                        typeof(SiteUnlockedEventTrigger)
                    }
                }
            };

            if (mode == DialogueIOMode.Read)
            {
                settings.Converters = new List<JsonConverter>
                {
                    new GraphJsonConverter()
                };
            }

            return settings;
        }
    }
}
