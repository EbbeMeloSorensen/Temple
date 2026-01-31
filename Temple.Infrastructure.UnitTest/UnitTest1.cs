using Craft.DataStructures.Graph;
using Craft.IO.Utils;
using Newtonsoft.Json;
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
                new(""),
            };

            var graph = new GraphAdjacencyList<DialogueVertex, LabelledEdge>(vertices, true);

            var jsonResolver = new ContractResolver();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = jsonResolver
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

            // Act
            var a = JsonConvert.DeserializeObject<GraphAdjacencyList<DialogueVertex, LabelledEdge>>(json);

            // Assert
        }
    }
}