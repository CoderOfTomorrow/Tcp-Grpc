using Grpc.Net.Client;
using GrpcAgent;
using Infrastructure.Abstractions;
using System;

namespace Infrastructure.Implementation.Grpc.Publisher
{
    public class GrpcPublisher : IPublisher
    {
        public bool IsConnected { get; set; } = true;
        private grpcPublisher.grpcPublisherClient client;

        public void Connect(string ipAddress, int port)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress($"http://{ipAddress}:{port}");
            client = new grpcPublisher.grpcPublisherClient(channel);
        }

        public void Send(Message message)
        {
            var requst = new PublishRequest()
            {
                Topic = message.Topic,
                Content = message.Content
            };
            var status = client.PublishMessage(requst);

            if(status.IsSuccess)
                Console.WriteLine("Status - The message was sent with succes\n");
            else
                Console.WriteLine("Status -The message failed to be sent\n");
        }
    }
}
