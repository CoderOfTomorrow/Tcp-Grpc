using System.Collections.Concurrent;

namespace Infrastructure
{
    public class MessageStorage
    {
        private readonly ConcurrentQueue<Message> messageQueue;
        public MessageStorage()
        {
            messageQueue = new();
        }

        public void Add(Message message)
        {
            messageQueue.Enqueue(message);
        }

        public Message GetNext()
        {
            messageQueue.TryDequeue(out Message payload);
            return payload;
        }

        public bool IsEmpty()
        {
            return messageQueue.IsEmpty;
        }

        public int Count()
        {
            return messageQueue.Count;
        }
    }
}
