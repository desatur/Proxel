using System.ComponentModel;
using System.Reflection;
using Proxel.Log4Console;

namespace Proxel.Config
{
    public abstract class ConfigBase : INotifyPropertyChanged
    {
        private static readonly string ConfigDirectory = Path.Combine(Directory.GetCurrentDirectory(), "config");
        private readonly ConfigSerializer _serializer;
        private readonly string _filePath;

        protected ConfigBase(string fileName)
        {
            _serializer = new ConfigSerializer();
            _filePath = Path.Combine(ConfigDirectory, fileName);

            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }
            Load();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Save();
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var yaml = await File.ReadAllTextAsync(_filePath);
                        _serializer.DeserializeInto(yaml, this);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to load configuration: {ex.Message}", "Config Serializer");
                    }
                }).Wait();
            }
        }

        private void Save()
        {
            Task.Run(async () =>
            {
                try
                {
                    var yaml = _serializer.Serialize(this);
                    await File.WriteAllTextAsync(_filePath, yaml);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to save configuration: {ex.Message}", "Config Serializer");
                }
            }).Wait();
        }

        protected void SetProperty<T>(string propertyName, T value)
        {
            var property = GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this, value);
                OnPropertyChanged(propertyName);
            }
        }

        protected T GetProperty<T>(string propertyName)
        {
            var property = GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanRead)
            {
                return (T)property.GetValue(this);
            }
            throw new ArgumentException($"Property '{propertyName}' does not exist or is not readable.");
        }
    }
}
