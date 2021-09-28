using System;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Implementation.Tcp.Subscriber
{
    class Handler
    {
        public static void Handle(byte[] payloadBytes)
        {
            var messageString = Encoding.UTF8.GetString(payloadBytes);
            var message = JsonSerializer.Deserialize<Message>(messageString);

            Console.WriteLine(message.Content);
        }
    }
}
