using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Proxel.Log4Console;
using Proxel.Protocol.Structs;

namespace Proxel.Protocol.Server
{
    public class Server : IDisposable
    {
        #region Public
        public IPAddress ListenerIPAddress { get; private set; }
        public EndPoint ListeningEndpoint { get; private set; }
        public bool Listening { get; private set; }
        public List<Player> Players
        {
            get
            {
                return LoginHandler.Players;
            }
        }
        #endregion

        #region Private/Internal
        private readonly TcpListener _listener;
        private TcpClientHandler _tcpclienthandler;
        #endregion

        public Server(IPAddress ipAddress = null, ushort port = 25565)
        {
            #if DEBUG
            ipAddress ??= ListenerIPAddress = IPAddress.Loopback;
            #else
            ipAddress ??= ListenerIPAddress = IPAddress.Any;
            #endif
            _listener = new TcpListener(ListenerIPAddress, port);
            _tcpclienthandler = new TcpClientHandler();
        }

        ~Server() { Dispose(); }

        public void Start()
        {
            _listener.Start();
            Listening = true;
            ListeningEndpoint = _listener.LocalEndpoint;
            Log.Info("Listening on endpoint " + ListeningEndpoint, "TcpListener");

            while (Listening)
            {
                TcpClient client = _listener.AcceptTcpClient();
                _ = Task.Run(() => _tcpclienthandler.HandleClientAsync(client));
            }
        }

        public void Stop()
        {
            if (!Listening) throw new Exception("Server is not running.");
            Listening = false;
            _listener.Stop();
            Log.Info($"No longer listening on endpoint {ListeningEndpoint}", "TcpListener");
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
