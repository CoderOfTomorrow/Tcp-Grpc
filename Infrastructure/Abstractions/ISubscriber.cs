namespace Infrastructure.Abstractions
{
    public interface ISubscriber
    {
        public string Topic { get; set; }
        public void Connect(string ipAddress, int port);
    }
}
