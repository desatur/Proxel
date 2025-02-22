using Proxel.PluginAPI.Loader;
using Proxel.Protocol.Server;

namespace Proxel
{
    public class Program
    {
        static void Main(/*string[] args*/)
        {
            PluginLoader.CreatePath();
            // IPAddress.Parse(ipString);
            Server server = new();
            server.Start();
        }
    }
}
