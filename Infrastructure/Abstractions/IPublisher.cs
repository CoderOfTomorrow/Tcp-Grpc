namespace Infrastructure.Abstractions
{
    public interface IPublisher
    {
        public bool IsConnected { get; set; }
        public void Connect(string ipAddress, int port);
        public void Send(Message message);
    }
}
