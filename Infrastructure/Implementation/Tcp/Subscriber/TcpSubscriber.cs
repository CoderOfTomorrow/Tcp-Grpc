using Infrastructure.Abstractions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Infrastructure.Implementation.Tcp.Subscriber
{
    public class TcpSubscriber : ISubscriber
    {
        private readonly Socket socket;
        public string Topic { get; set; }

        public TcpSubscriber()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCall, null);
            Console.WriteLine("Waiting ... ");
        }

        private void ConnectedCall(IAsyncResult asyncResult)
        {
            if (socket.Connected)
            {
                Console.WriteLine("Subscriber connected .");
                Subscribe();
                Receive();
            }
            else
            {
                Console.WriteLine("Error: Subscriber not connected.");
            }
        }

        private void Subscribe()
        {
            var data = Encoding.UTF8.GetBytes("subscribe#" + Topic);
            Send(data);
        }

        private void Receive()
        {
            TcpConnection connection = new()
            {
                Socket = socket
            };

            socket.BeginReceive(connection.Data, 0, connection.Data.Length, SocketFlags.None, ReceiveCall, connection);
        }

        private void ReceiveCall(IAsyncResult asyncResult)
        {
            TcpConnection connection = asyncResult.AsyncState as TcpConnection;

            try
            {
                int buffSize = socket.EndReceive(asyncResult, out SocketError response);

                if (response == SocketError.Success)
                {
                    byte[] payloadBytes = new byte[buffSize];
                    Array.Copy(connection.Data, payloadBytes, payloadBytes.Length);

                    Handler.Handle(payloadBytes);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't receive data from broker. {e.Message}");
            }
            finally
            {
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length, SocketFlags.None, ReceiveCall, connection);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    connection.Socket.Close();
                }
            }
        }

        private void Send(byte[] data)
        {
            try
            {
                socket.Send(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send data: {e.Message}");
            }
        }
    }
}
