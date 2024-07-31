using Proxel.Log4Console;

namespace Proxel.Config
{
    public abstract class Config<T> where T : class, new()
    {
        private static readonly ConfigSerializer _serializer = new();

        public static T Load(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return _serializer.LoadFromFile<T>(filePath);
                }
                else
                {
                    File.Create(filePath).Dispose();
                    Log.Warn($"Config file not found: {filePath}, a new config file was created.", "Config::Load");
                    return new T();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load config ({filePath}): {ex.Message}", "Config::Load");
                throw;
            }
        }

        public void Save(string filePath)
        {
            try
            {
                _serializer.SaveToFile(this as T, filePath);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save config ({filePath}): {ex.Message}", "Config::Load");
                throw;
            }
        }
    }
}
