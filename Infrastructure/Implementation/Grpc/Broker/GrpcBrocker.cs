using GrpcAgent;
using Infrastructure.Abstractions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Grpc.Broker
{
    public class GrpcBrocker : IBrocker
    {
        private MessageStorage messageStorage;
        private ConnectionStorage connectionStorage;
        private IWebHost host;
        public void Start(string ip, int port)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            host = WebHost.CreateDefaultBuilder()
                .UseStartup<BrokerStartup>()
                .UseUrls($"http://{ip}:{port}")
                .Build();

            host.Start();
        }
        public async Task Execute(CancellationToken cancellationToken)
        {
            messageStorage = host.Services.GetService<MessageStorage>();
            while (!messageStorage.IsEmpty())
            {
                var message = messageStorage.GetNext();
                if (message != null)
                {
                    connectionStorage = host.Services.GetService<ConnectionStorage>();
                    var connections = connectionStorage.GetConnectionInfosByTopic(message.Topic);
                    var grpcConnections = connections.OfType<GrpcConnection>();

                    foreach (var connection in grpcConnections)
                    {
                        var client = new grpcNotifier.grpcNotifierClient(connection.Channel);
                        var request = new NotifyRequest() { Content = message.Content };
                        client.Notify(request, cancellationToken: cancellationToken);
                    }
                }
            }
            await Task.Delay(2000, cancellationToken);
        }
    }
}
