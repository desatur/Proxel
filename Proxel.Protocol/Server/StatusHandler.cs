using Proxel.Log4Console;
using Proxel.Protocol.Helpers;
using Proxel.Protocol.Networking.Utils;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Proxel.Protocol.Server
{
    public class StatusHandler
    {
        internal static async Task HandleStatusRequestAsync(NetworkStream networkStream)
        {
            // Status Request from client
            ushort length = (ushort)await VarInt.ReadVarIntAsync(networkStream); // Should be 1
            ushort packetId = (ushort)await VarInt.ReadVarIntAsync(networkStream); // Should be 0
            if (length == 1 && packetId == 0)
            {
                Log.Debug("Status Request detected!", "HandleStatusRequestAsync");
            }
            else return;

            using (var packetBuilder = new PacketBuilder(networkStream))
            {
                packetBuilder.SetPacketID(0x00);
                using (var statusBuilder = new StatusBuilder())
                {
                    Log.Debug($"Status JSON: {statusBuilder.GetFinalJson()}", $"HandleStatusRequestAsync");
                    packetBuilder.WriteByteArray(statusBuilder.GetFinalJsonAsByteArray());
                    await packetBuilder.Send();
                }
            }
        }
    }
}
