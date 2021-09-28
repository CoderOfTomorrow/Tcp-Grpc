using Infrastructure;
using Infrastructure.Abstractions;
using Infrastructure.Implementation.Grpc.Publisher;
using Infrastructure.Implementation.Tcp.Publisher;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Publisher
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Publisher...");

            var publisher = ResolveDependencies().GetService<IPublisher>();
            publisher.Connect(Settings.Ip, Settings.Port);

            if (publisher.IsConnected)
            {
                while (true)
                {
                    var message = new Message();

                    Console.Write("Topic: ");
                    message.Topic = Console.ReadLine().ToLower();

                    Console.Write("Message: ");
                    message.Content = Console.ReadLine();

                    publisher.Send(message);
                }
            }
        }

        static ServiceProvider ResolveDependencies()
        {
            return new ServiceCollection()
            //.AddSingleton<IPublisher, TcpPublisher>()        
            .AddSingleton<IPublisher, GrpcPublisher>() 
            .BuildServiceProvider();
        }
    }
}
