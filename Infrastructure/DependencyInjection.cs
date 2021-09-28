using Infrastructure.Abstractions;
using Infrastructure.Implementation;
using Infrastructure.Implementation.Grpc.Broker;
using Infrastructure.Implementation.Tcp;
using Infrastructure.Implementation.Tcp.Broker;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTcpInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IBrocker, TcpBrocker>();
            services.AddSingleton<MessageStorage>();
            services.AddSingleton<ConnectionStorage>();
            //services.AddSingleton<IConnection, TcpConnection>();
            return services;
        }

        public static IServiceCollection AddGrpcInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IBrocker, GrpcBrocker>();
            services.AddSingleton<ConnectionStorage>();

            return services;
        }
    }
}
