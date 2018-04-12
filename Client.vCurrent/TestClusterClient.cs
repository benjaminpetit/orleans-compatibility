using GrainInterfaces.vCurrent;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Common;
using Tests.Scenarios;
using Client.Scenarios;
using Xunit.Sdk;
using System.Linq;

namespace Client
{
    public class TestClusterClient : MarshalByRefObject, ITestClusterClient
    {
        private IClusterClient client;
        private IServiceProvider scenarioProvider;

        public TestClusterClient(
            string connectionString,
            string serviceId,
            string clusterId)
        {
            this.client = new ClientBuilder()
                .Configure<ClusterOptions>(options => { options.ClusterId = clusterId; options.ServiceId = serviceId; })
                .UseAzureStorageClustering(options => options.ConnectionString = connectionString)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IPingGrain).Assembly).WithReferences())
                .Build();
            this.scenarioProvider = LoadAllScenarios();
        }

        public void Connect()
        {
            this.client.Connect().GetAwaiter().GetResult();
        }

        public void Execute<T>(Func<T, Task> func) where T : ITestScenario
        {
            try
            {
                var scenario = this.scenarioProvider.GetService<T>();
                func.Invoke(scenario).GetAwaiter().GetResult();
            }
            catch (XunitException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private IServiceProvider LoadAllScenarios()
        {
            var sp = new ServiceCollection();
            sp.AddSingleton<IClusterClient>(client);

            // Add implementations of various ITestScenario by reflection
            var scenarioClasses = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ITestScenario).IsAssignableFrom(type) && !type.IsInterface);
            foreach (var s in scenarioClasses)
            {
                foreach (var i in s.GetInterfaces())
                {
                    if (typeof(ITestScenario).IsAssignableFrom(i) 
                        && typeof(ITestScenario) != i)
                        sp.AddSingleton(i, s);
                }
            }

            return sp.BuildServiceProvider();
        }
    }
}
