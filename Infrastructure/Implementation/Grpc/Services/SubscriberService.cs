using Grpc.Core;
using GrpcAgent;
using Infrastructure.Implementation.Grpc.Broker;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Grpc.Services
{
    public class SubscriberService : grpcSubscriber.grpcSubscriberBase
    {
        private readonly ConnectionStorage storage;
        public SubscriberService(ConnectionStorage connectionStorage)
        {
            storage = connectionStorage;
        }
        public override Task<SubscribeReply> Subscribe(SubscribeRequest request, ServerCallContext context)
        {
            var connection = new GrpcConnection(request.Address, request.Topic);

            storage.Add(connection);

            Console.WriteLine($"New client subscribed : {request.Address}");

            return Task.FromResult(new SubscribeReply()
            {
                IsSuccess = true
            });
        }
    }
}
