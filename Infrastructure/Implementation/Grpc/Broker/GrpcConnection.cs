using Grpc.Net.Client;
using Infrastructure.Abstractions;

namespace Infrastructure.Implementation.Grpc.Broker
{
    public class GrpcConnection : IConnection
    {
        public GrpcConnection(string address, string topic)
        {
            Address = address;
            Topic = topic;
            Channel = GrpcChannel.ForAddress(address);
        }
        public string Address { get; set; }
        public string Topic { get; set; }
        public GrpcChannel Channel { get; }
    }
}
