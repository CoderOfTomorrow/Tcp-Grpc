namespace Infrastructure.Abstractions
{
    public interface IConnection
    {
        public string Address { get; set; }
        public string Topic { get; set; }
    }
}
