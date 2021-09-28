using Infrastructure.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Implementation
{
    public class ConnectionStorage
    {
        private readonly List<IConnection> connections;
        private readonly object locker;

        public ConnectionStorage()
        {
            connections = new();
            locker = new object();
        }

        public int Count()
        {
            return connections.Count;
        }
        public void Add(IConnection connection)
        {
            lock (locker)
            {
                connections.Add(connection);
            }
        }

        public void Remove(string address)
        {
            lock (locker)
            {
                connections.RemoveAll(x => x.Address == address);
            }
        }

        public List<IConnection> GetConnectionInfosByTopic(string topic)
        {
            List<IConnection> selectedConnections;

            lock (locker)
            {
                selectedConnections = connections.Where(x => x.Topic == topic).ToList();
            }

            return selectedConnections;
        }
    }
}
