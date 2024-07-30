using Proxel.Log4Console;
using Proxel.Protocol.Networking.Utils;
using Proxel.Protocol.Structs;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;

namespace Proxel.Protocol.Server
{
    public class ClientHandler
    {
        internal static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                await HandlePacketAsync(client.GetStream(), client);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occured while handling TcpClient: {ex.Message}", "ClientHandler");
            }
        }

        private static async Task HandlePacketAsync(NetworkStream stream, TcpClient client)
        {
            Packet packet;
            try
            {
                packet = await PacketReader.ReadPacketAsync(stream);
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occured while handling packet sent by client: {ex.Message}", "ClientPacketHandler");
                return;
            }

            switch (packet.PacketId)
            {
                case 0: // Handshake (0x00)
                    await HandshakeHandler.HandleHandshakeAsync(packet, stream, client);
                    break;
                default:
                    Log.Warn($"Unsupported packet ID sent by client: {packet.PacketId} (HEX: {packet.PacketId:X2})", "ClientPacketHandler");
                    break;
            }
        }
    }
}
