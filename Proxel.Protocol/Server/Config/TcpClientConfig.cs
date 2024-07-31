using Proxel.Config;
using System.ComponentModel;

namespace Proxel.Protocol.Server.Config
{
    internal class TcpClientConfig : ConfigBase
    {
        public TcpClientConfig() : base("TcpClient.yaml") { }

        public int ReceiveBufferSize { get; set; } = 8192;
        public int SendBufferSize { get; set; } = 8192;
        public int ReceiveTimeout { get; set; } = 2500;
        public int SendTimeout { get; set; } = 2500;
    }
}
