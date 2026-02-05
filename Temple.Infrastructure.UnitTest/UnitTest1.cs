using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Craft.DataStructures.Graph;
using Temple.Application.Core;
using Temple.Infrastructure.Dialogues;

namespace Temple.Infrastructure.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void SerializeAGraphToJsonFile()
        {
            // Arrange
            var vertices = new List<DialogueVertex>
            {
                new("Nice weather today, huh?"),
                new()
                {
                    Text = "Wanna kill rats?",
                    GameEventTrigger = new QuestDiscoveredEventTrigger("rat_infestation")
                },
                new("Then good luck"),
                new("Then fuck off, rat lover"),
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);
            graph.AddEdge(new LabelledEdge(0, 1, "Yep super nice"));
            graph.AddEdge(new LabelledEdge(1, 2, "Oh yes, I hate rats"));
            graph.AddEdge(new LabelledEdge(1, 3, "No, rats are cute"));
            graph.AddEdge(new LabelledEdge(2, 4, "Thanks, see you later"));
            graph.AddEdge(new LabelledEdge(3, 4, "Ok"));

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
                        typeof(QuestDiscoveredEventTrigger),
                        //typeof(QuestAcceptedEventTrigger)
                    }
                }
            };

            var json = JsonConvert.SerializeObject(
                graph,
                Formatting.Indented,
                settings);

            using var streamWriter = new StreamWriter(@"C:\Temp\serializedGraph.json");

            streamWriter.WriteLine(json);

            // Act
            // Assert
        }

        [Fact]
        public void DeserializeAGraphFromJsonFile()
        {
            // Arrange
            using var streamReader = new StreamReader(@"C:\Temp\serializedGraph.json");
            var json = streamReader.ReadToEnd();

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
                        typeof(QuestDiscoveredEventTrigger),
                        //typeof(QuestAcceptedEventTrigger)
                    }
                }
            };

            // Act
            var a = JsonConvert.DeserializeObject<GraphAdjacencyList<DialogueVertex, LabelledEdge>>(json, settings);

            // Assert
            a = null;
        }

        [Fact]
        public void SerializeADialogueGraphToJsonFile()
        {
            // Arrange
            var vertices = new List<DialogueVertex>
            {
                new("Nice weather today, huh?"),
                new()
                {
                    Text = "Wanna kill rats?",
                    GameEventTrigger = new QuestDiscoveredEventTrigger("rat_infestation")
                },
                new("Then good luck"),
                new("Then fuck off, rat lover"),
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);
            graph.AddEdge(new LabelledEdge(0, 1, "Yep super nice"));
            graph.AddEdge(new LabelledEdge(1, 2, "Oh yes, I hate rats"));
            graph.AddEdge(new LabelledEdge(1, 3, "No, rats are cute"));
            graph.AddEdge(new LabelledEdge(2, 4, "Thanks, see you later"));
            graph.AddEdge(new LabelledEdge(3, 4, "Ok"));

            var dialogueGraph = new DialogueGraph
            {
                Priority = 90.0,
                Graph = graph
            };

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
                        typeof(QuestDiscoveredEventTrigger),
                        //typeof(QuestAcceptedEventTrigger)
                    }
                }
            };

            var json = JsonConvert.SerializeObject(
                dialogueGraph,
                Formatting.Indented,
                settings);

            using var streamWriter = new StreamWriter(@"C:\Temp\serializedDialogueGraph.json");

            streamWriter.WriteLine(json);

            // Act
            // Assert
        }

        [Fact]
        public void DeserializeADialogueGraphFromJsonFile()
        {
            // Arrange
            using var streamReader = new StreamReader(@"C:\Temp\serializedDialogueGraph.json");
            var json = streamReader.ReadToEnd();

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
                        typeof(QuestDiscoveredEventTrigger),
                        //typeof(QuestAcceptedEventTrigger)
                    }
                }
            };

            // Act
            var a = JsonConvert.DeserializeObject<DialogueGraph>(json, settings);

            // Assert
            a = null;
        }

    }
}