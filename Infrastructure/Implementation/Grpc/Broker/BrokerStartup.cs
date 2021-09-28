using Infrastructure.Implementation.Grpc.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Implementation.Grpc.Broker
{
    public class BrokerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MessageStorage>();
            services.AddSingleton<ConnectionStorage>();
            services.AddGrpc();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<PublisherService>();
                endpoints.MapGrpcService<SubscriberService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
