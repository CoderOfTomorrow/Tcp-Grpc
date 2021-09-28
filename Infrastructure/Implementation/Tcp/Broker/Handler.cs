using System.Linq;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Implementation.Tcp.Broker
{
    public class Handler
    {
        public static void Handle(byte[] payloadBytes, TcpConnection connectionInfo, MessageStorage messageStorage, ConnectionStorage connectionStorage)
        {
            var messageString = Encoding.UTF8.GetString(payloadBytes);

            if (messageString.StartsWith("subscribe#"))
            {
                connectionInfo.Topic = messageString.Split("subscribe#").LastOrDefault();
                connectionStorage.Add(connectionInfo);
            }
            else
            {
                Message message = JsonSerializer.Deserialize<Message>(messageString);
                messageStorage.Add(message);
            }

        }
    }
}
