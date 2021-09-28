using Grpc.Core;
using GrpcAgent;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Grpc.Services
{
    public class NotifyService : grpcNotifier.grpcNotifierBase
    {
        public override Task<NotifyReply> Notify(NotifyRequest request, ServerCallContext context)
        {
            try
            {
                Console.WriteLine($"Received : {request.Content}");
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return Task.FromResult(new NotifyReply() { IsSuccess = true });
        }
    }
}
