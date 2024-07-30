using System.Net;
using System.Net.Sockets;
using Proxel.Log4Console;
using Proxel.Protocol.Structs;

namespace Proxel.Protocol.Server
{
    public class Server : IAsyncDisposable
    {
        #region Public
        public IPAddress TargetIPAddress { get; private set; }
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
        #endregion

        public Server(IPAddress ipAddress = null, ushort port = 25565)
        {
            #if DEBUG
            ipAddress ??= TargetIPAddress = IPAddress.Loopback;
            #else
            ipAddress ??= ListeningIPAddress = IPAddress.Any;
            #endif
            _listener = new TcpListener(TargetIPAddress, port);
        }
        ~Server() { DisposeAsync().AsTask().Wait(); }

        public async Task Start()
        {
            _listener.Start();
            Listening = true;
            ListeningEndpoint = _listener.LocalEndpoint;
            Log.Info("Listening on endpoint " + ListeningEndpoint, "TcpListener");
            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _ = ClientHandler.HandleClientAsync(client);
            }
        }

        public async Task Stop()
        {
            if (!Listening) throw new Exception("Server is not running.");
            _listener.Stop();
            Listening = false;
            Log.Info($"No longer listening on endpoint {ListeningEndpoint}", "TcpListener");
        }

        public async ValueTask DisposeAsync()
        {
            await Stop();
            GC.SuppressFinalize(this);
        }
    }
}