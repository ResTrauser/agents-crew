using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CrewAI.DotNet.Core.Configuration
{
    public class YamlConfigurationLoader
    {
        private readonly IDeserializer _deserializer;

        public YamlConfigurationLoader()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) // Assuming standard YAML snake_case
                .Build();
        }

        public T Load<T>(string yamlContent)
        {
            return _deserializer.Deserialize<T>(yamlContent);
        }

        public T LoadFromFile<T>(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return Load<T>(content);
        }
    }
}
