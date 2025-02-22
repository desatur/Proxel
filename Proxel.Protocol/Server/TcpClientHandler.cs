using Proxel.Log4Console;
using Proxel.Protocol.Networking.Utils;
using Proxel.Protocol.Server.Config;
using Proxel.Protocol.Structs;
using System.Net.Sockets;

namespace Proxel.Protocol.Server
{
    public class TcpClientHandler
    {
        public TcpClientConfig Config { get; private set; }

        public TcpClientHandler()
        {
            Config = new TcpClientConfig();
        }

        internal async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                client.ReceiveBufferSize = Config.ReceiveBufferSize;
                client.SendBufferSize = Config.SendBufferSize;
                client.ReceiveTimeout = Config.ReceiveTimeout;
                client.SendTimeout = Config.SendTimeout;
                await HandlePacketStreamAsync(client);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occured while handling TcpClient: {ex.Message}", "ClientHandler");
            }
        }

        private static async Task HandlePacketStreamAsync(TcpClient client)
        {
            Packet packet;
            try
            {
                packet = await PacketReader.ReadPacketAsync(client.GetStream());
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occured while handling packet sent by client: {ex.Message}", "HandlePacketStreamAsync");
                return;
            }

            switch (packet.PacketId)
            {
                case 0: // Handshake (0x00)
                    await HandshakeHandler.HandleHandshakeAsync(packet, client);
                    break;
                default:
                    Log.Warn($"Unsupported packet ID sent by client: {packet.PacketId} (HEX: {packet.PacketId:X2})", "HandlePacketStreamAsync");
                    break;
            }
        }
    }
}
