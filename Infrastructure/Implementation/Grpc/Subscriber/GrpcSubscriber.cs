using Grpc.Net.Client;
using GrpcAgent;
using Infrastructure.Abstractions;
using Infrastructure.Implementation.Grpc.Subscriber;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Infrastructure.Implementation.Grps.Subscriber
{
    public class GrpcSubscriber : ISubscriber
    {
        public string Topic { get; set; }

        public void Connect(string ipAddress, int port)
        {
            var host = WebHost.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                })
                .ConfigureKestrel(webHost => webHost.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2))
                .UseStartup<SubscriberStartup>()
                .UseUrls($"http://{ipAddress}:0")
                .Build();

            host.Start();

            Subscribe(host, ipAddress, port);
        }

        private void Subscribe(IWebHost host, string ip, int port)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress($"http://{ip}:{port}");
            var client = new grpcSubscriber.grpcSubscriberClient(channel);
            var address = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();

            var request = new SubscribeRequest()
            {
                Topic = Topic,
                Address = address
            };

            var status = client.Subscribe(request);

            if (status.IsSuccess)
                Console.WriteLine("Waiting for messages :");
        }
    }
}
