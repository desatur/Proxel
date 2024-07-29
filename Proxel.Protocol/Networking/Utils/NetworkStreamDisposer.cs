using System.Net.Sockets;

namespace Proxel.Protocol.Networking.Utils
{
    public static class NetworkStreamDisposer
    {
        public static void Dispose(NetworkStream stream)
        {
            stream.Close();
            stream.Dispose();
        }
    }
}
