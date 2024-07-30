using Proxel.PluginAPI.Loader;
using Proxel.Protocol.Server;

namespace Proxel
{
    public class Program
    {
        static async Task Main(/*string[] args*/)
        {
            PluginLoader.CreatePath();
            // IPAddress.Parse(ipString);
            Server server = new();
            await server.Start();
        }
    }
}
