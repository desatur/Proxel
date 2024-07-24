using System.Net;
using System.Threading.Tasks;
using Proxel.Protocol.Server;

namespace Proxel
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server(IPAddress.Any);
            await server.StartAsync();
        }
    }
}
