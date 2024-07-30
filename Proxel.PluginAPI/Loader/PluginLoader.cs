using System.Reflection;

namespace Proxel.PluginAPI.Loader
{
    public class PluginLoader
    {
        public static List<Assembly> Plugins { get; private set; }

        private static string[] pluginAssemblies;
        private const string pluginPath = "plugins";

        public static void CreatePath()
        {
            if (!Directory.Exists(pluginPath))
            {
                Directory.CreateDirectory(pluginPath);
            }
        }

        internal static void FindAssemblies()
        {
            pluginAssemblies = Directory.GetFiles(pluginPath, "*.dll");
        }

        internal static void LoadPlugins()
        {
            foreach (var assemblyPath in pluginAssemblies)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    Plugins.Add(assembly);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
