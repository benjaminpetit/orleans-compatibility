using Grains;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tests.Common;

namespace Silo
{
    public class TestSiloHost : MarshalByRefObject, ITestSiloHost
    {
        private ISiloHost siloHost;

        public SiloAddress SiloAddress => this.siloHost.Services.GetRequiredService<ILocalSiloDetails>().SiloAddress;

        public SiloAddress GatewayAddress => this.siloHost.Services.GetRequiredService<ILocalSiloDetails>().GatewayAddress;

        public TestSiloHost(
            string connectionString,
            string serviceId,
            string clusterId)
        {
            var siloPort = GetFreePort();
            var gwPort = GetFreePort();

            this.siloHost = new SiloHostBuilder()
                .Configure<ClusterOptions>(options => { options.ClusterId = clusterId; options.ServiceId = serviceId; })
                .UseAzureStorageClustering(options => options.ConnectionString = connectionString)
                .UseAzureTableReminderService(options => options.ConnectionString = connectionString)
                .ConfigureEndpoints(IPAddress.Loopback, siloPort, gwPort)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PingGrain).Assembly).WithReferences())
                .Build();
        }

        /// <summary>Starts the silo</summary>
        public void Start()
        {
            this.siloHost.StartAsync().GetAwaiter().GetResult();
        }

        /// <summary>Gracefully shuts down the silo</summary>
        public void Shutdown()
        {
            if (this.siloHost != null)
            {
                this.siloHost.StopAsync().GetAwaiter().GetResult();
                this.siloHost = null;
            }
        }

        private int GetFreePort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port;
        }
    }
}
