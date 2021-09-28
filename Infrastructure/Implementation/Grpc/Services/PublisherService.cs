using Grpc.Core;
using GrpcAgent;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Grpc.Services
{
    public class PublisherService : grpcPublisher.grpcPublisherBase
    {
        private readonly MessageStorage storage;
        public PublisherService(MessageStorage storage)
        {
            this.storage = storage;
        }
        public override Task<PublishReply> PublishMessage(PublishRequest request, ServerCallContext context)
        {
            var message = new Message
            {
                Topic = request.Topic,
                Content = request.Content
            };

            storage.Add(message);

            Console.WriteLine($"A message was sent to broker({storage.Count()}) :\nTopic - {request.Topic}\nContent - {request.Content}\n");

            return Task.FromResult(new PublishReply()
            {
                IsSuccess = true
            });
        }
    }
}
