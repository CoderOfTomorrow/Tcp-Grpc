using Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Tcp.Broker
{
    public class TcpBrocker : IBrocker
    {
        private const int CONNECTIONS_LIMIT = 8;
        private readonly Socket socket;
        private readonly MessageStorage messageStorage;
        private readonly MessageStorage lostStorage;
        private readonly ConnectionStorage connectionStorage;
        public TcpBrocker(MessageStorage messageStorage, ConnectionStorage connectionStorage)
        {
            this.messageStorage = messageStorage;
            this.connectionStorage = connectionStorage;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            lostStorage = new();
        }

        public void Start(string ip, int port)
        {
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            socket.Listen(CONNECTIONS_LIMIT);
            Accept();
        }

        private void Accept()
        {
            socket.BeginAccept(AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult asyncResult)
        {
            TcpConnection connection = new();

            try
            {
                connection.Socket = socket.EndAccept(asyncResult);
                connection.Address = connection.Socket.RemoteEndPoint.ToString();
                connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length, SocketFlags.None, ReceiveCallBack, connection);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't accept: {e.Message}");
            }
            finally
            {
                Accept();
            }
        }

        private void ReceiveCallBack(IAsyncResult asyncResult)
        {
            TcpConnection connection = asyncResult.AsyncState as TcpConnection;

            try
            {
                Socket senderSocket = connection.Socket;
                int buffSize = senderSocket.EndReceive(asyncResult, out SocketError response);

                if (response == SocketError.Success)
                {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connection.Data, payload, payload.Length);

                    Handler.Handle(payload, connection, messageStorage, connectionStorage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't receive data: {e.Message}");
            }
            finally
            {
                try
                {
                    connection.Socket.BeginReceive(connection.Data, 0, connection.Data.Length, SocketFlags.None, ReceiveCallBack, connection);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    var address = connection.Socket.RemoteEndPoint.ToString();

                    connectionStorage.Remove(address);
                    connection.Socket.Close();
                }
            }
        }
        public async Task Execute(CancellationToken cancellationToken)
        {
            while (!messageStorage.IsEmpty())
            {
                var payload = messageStorage.GetNext();

                if (payload != null)
                {
                    var connections = connectionStorage.GetConnectionInfosByTopic(payload.Topic);
                    var tcpConnections = connections.OfType<TcpConnection>();
                    bool messageStatus = false;
                    foreach (var connection in tcpConnections)
                    {
                        if (connection.Topic == payload.Topic)
                            messageStatus = true;
                        var payloadString = JsonSerializer.Serialize(payload);
                        byte[] data = Encoding.UTF8.GetBytes(payloadString);

                        connection.Socket.Send(data);
                    }
                    if (!messageStatus)
                        lostStorage.Add(payload);
                }
            }
            for(var i = 0; i<lostStorage.Count(); i++)
            {
                var payload = lostStorage.GetNext();
                messageStorage.Add(payload);
            }
            await Task.Delay(500, cancellationToken);
        }
    }
}
