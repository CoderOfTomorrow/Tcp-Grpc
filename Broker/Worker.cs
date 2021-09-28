using Infrastructure;
using Infrastructure.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Broker
{
    public class Worker : BackgroundService
    {
        private readonly IBrocker broker;
        public Worker(IBrocker broker)
        {
            this.broker = broker;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            broker.Start(Settings.Ip, Settings.Port);

            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await broker.Execute(stoppingToken);
            }
        }
    }
}
