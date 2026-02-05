using Craft.DataStructures.IO;
using Newtonsoft.Json;
using Temple.Infrastructure.Dialogues;

namespace Temple.Infrastructure.IO
{
    public static class DialogueIO
    {
        public static IEnumerable<DialogueGraph> ReadDialogueGraphListFromFile(string fileName)
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
                        typeof(QuestDiscoveredEventTrigger),
                        typeof(QuestAcceptedEventTrigger)
                    }
                }
            };

            return JsonConvert.DeserializeObject<List<DialogueGraph>>(json, settings);
        }
    }
}
