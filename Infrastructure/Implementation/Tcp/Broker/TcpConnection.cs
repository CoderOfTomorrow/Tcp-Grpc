using Infrastructure.Abstractions;
using System.Net.Sockets;

namespace Infrastructure.Implementation.Tcp
{
    public class TcpConnection : IConnection
    {

        public const int BUFF_SIZE = 1024;
        public byte[] Data { get; set; }
        public Socket Socket { get; set; }
        public string Address { get; set; }
        public string Topic { get; set; }
        public TcpConnection()
        {
            Data = new byte[BUFF_SIZE];
        }
    }
}
