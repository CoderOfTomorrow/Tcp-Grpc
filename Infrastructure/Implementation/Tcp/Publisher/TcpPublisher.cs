using Infrastructure.Abstractions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Infrastructure.Implementation.Tcp.Publisher
{
    public class TcpPublisher : IPublisher
    {
        private readonly Socket socket;
        public bool IsConnected { get; set; }

        public TcpPublisher()
        {
            socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallBack, null);
            Thread.Sleep(2000);
        }

        public void Send(Message message)
        {
            try
            {
                var messageString = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(messageString);

                socket.Send(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send data: {e.Message}");
            }
        }

        private void ConnectedCallBack(IAsyncResult asyncResult)
        {
            if (socket.Connected)
            {
                Console.WriteLine("Sender connected to broker.");
            }
            else
            {
                Console.WriteLine("Error: Sender not connected to broker.");
            }

            IsConnected = socket.Connected;
        }
    }
}
