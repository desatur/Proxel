using System.Reflection;
using System.Text;
using YamlDotNet.Core.Events;
using YamlDotNet.Core;
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
            return _deserializer.Deserialize<T>(yaml);
        }

        public void DeserializeInto<T>(string yaml, T obj)
        {
            var tempObj = Deserialize<T>(yaml);
            foreach (var prop in typeof(T).GetProperties())
            {
                var value = prop.GetValue(tempObj);
                prop.SetValue(obj, value);
            }
        }

        public string Serialize<T>(T obj)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                Emitter emitter = new(writer);
                EmitWithComments(emitter, obj);
            }
            return stringBuilder.ToString();
        }

        private static void EmitWithComments(IEmitter emitter, object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                var commentAttribute = property.GetCustomAttribute<CommentAttribute>();
                if (commentAttribute != null)
                {
                    emitter.Emit(new Comment(commentAttribute.Text, false));
                }

                var value = property.GetValue(obj);
                emitter.Emit(new MappingStart());
                emitter.Emit(new Scalar(property.Name));
                emitter.Emit(new Scalar(value?.ToString() ?? "null"));
                emitter.Emit(new MappingEnd());
            }
        }

        public T LoadFromFile<T>(string filePath)
        {
            Log4Console.Log.Debug(filePath);
            var yaml = File.ReadAllText(filePath);
            return Deserialize<T>(yaml);
        }

        public void SaveToFile<T>(T obj, string filePath)
        {
            Log4Console.Log.Debug(filePath);
            var yaml = Serialize(obj);
            File.WriteAllText(filePath, yaml);
        }
    }
}
