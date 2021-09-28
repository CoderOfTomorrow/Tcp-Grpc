using Infrastructure;
using Infrastructure.Abstractions;
using Infrastructure.Implementation.Grps.Subscriber;
using Infrastructure.Implementation.Tcp.Subscriber;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Subscriber
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Subscriber...");

            var subscriber = ResolveDependencies().GetService<ISubscriber>();

            Console.Write("Topic: ");
            subscriber.Topic = Console.ReadLine().ToLower();

            subscriber.Connect(Settings.Ip, Settings.Port);

            Console.ReadLine();
        }

        static ServiceProvider ResolveDependencies()
        {
            return new ServiceCollection()
            //.AddSingleton<ISubscriber, TcpSubscriber>()      
            .AddSingleton<ISubscriber, GrpcSubscriber>()
            .BuildServiceProvider();
        }
    }
}
