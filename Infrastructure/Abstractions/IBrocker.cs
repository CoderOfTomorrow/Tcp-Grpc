using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Abstractions
{
    public interface IBrocker
    {
        public void Start(string ip, int port);
        public Task Execute(CancellationToken cancellationToken);
    }
}
