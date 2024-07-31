using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Proxel.Config
{
    public class ConfigSerializer
    {
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;

        public ConfigSerializer()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        public T Deserialize<T>(string yaml)
        {
            if (string.IsNullOrWhiteSpace(yaml)) throw new ArgumentException("YAML content cannot be null or empty.", nameof(yaml));
            try
            {
                return _deserializer.Deserialize<T>(yaml);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Deserialization failed.", ex);
            }
        }

        public void DeserializeInto<T>(string yaml, T obj)
        {
            if (string.IsNullOrWhiteSpace(yaml)) throw new ArgumentException("YAML content cannot be null or empty.", nameof(yaml));
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            T tempObj = Deserialize<T>(yaml);
            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var value = prop.GetValue(tempObj);
                    prop.SetValue(obj, value);
                }
            }
        }

        public string Serialize<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            try
            {
                return _serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Serialization failed.", ex);
            }
        }

        public async Task<T> LoadFromFileAsync<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found.", filePath);
            try
            {
                var yaml = await File.ReadAllTextAsync(filePath);
                return Deserialize<T>(yaml);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load the configuration from file.", ex);
            }
        }

        public async Task SaveToFileAsync<T>(T obj, string filePath)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            try
            {
                var yaml = Serialize(obj);
                await File.WriteAllTextAsync(filePath, yaml);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save the configuration to file.", ex);
            }
        }
    }
}
