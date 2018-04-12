using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Common;

namespace Tests
{
    public static class Versions
    {
        public const string Current = "Current"; 
        public const string Next = "Next";
    }

    public class TestCluster : IDisposable
    {
        private IDictionary<string, string> siloPathPerVersion = new Dictionary<string, string>
        {
            {Versions.Current,  @"..\..\..\..\Silo.vCurrent\bin\Debug\net462"   },
            {Versions.Next,     @"..\..\..\..\Silo.vNext\bin\Debug\net462"      },
        };

        private IDictionary<string, string> clientPathPerVersion = new Dictionary<string, string>
        {
            {Versions.Current,  @"..\..\..\..\Client.vCurrent\bin\Debug\net462" },
            {Versions.Next,     @"..\..\..\..\Client.vNext\bin\Debug\net462"    },
        };

        private string connectionString;
        private string serviceId;
        private string clusterId;
        private List<ITestSiloHost> startedSilos = new List<ITestSiloHost>();

        public TestCluster() : this(
                  "UseDevelopmentStorage=true;",
                  "Test_" + Guid.NewGuid(),
                  "Cluster_" + Guid.NewGuid())
        {
        }

        public TestCluster(string connectionString, string serviceId, string clusterId)
        {
            this.connectionString = connectionString;
            this.serviceId = serviceId;
            this.clusterId = clusterId;
        }

        public ITestSiloHost StartSilo(string version)
        {
            var silo = CreateSiloHost(siloPathPerVersion[version]);
            silo.Start();
            this.startedSilos.Add(silo);
            return silo;
        }

        public ITestClusterClient StartClient(string version)
        {
            var client = CreateClientHost(clientPathPerVersion[version]);
            client.Connect();
            return client;
        }

        public void Dispose()
        {
            foreach (var silo in this.startedSilos)
            {
                silo.Shutdown();
            }
        }

        private ITestClusterClient CreateClientHost(string applicationBase)
        {
            AppDomainSetup setup = GetAppDomainSetupInfo(applicationBase, "Client.dll");
            var appDomain = AppDomain.CreateDomain("Client_", null, setup);

            var siloHost = (ITestClusterClient) appDomain.CreateInstanceAndUnwrap(
                    "Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "Client.TestClusterClient",
                    false,
                    BindingFlags.Default,
                    null,
                    new object[] { this.connectionString, this.serviceId, this.clusterId },
                    CultureInfo.CurrentCulture,
                    new object[] { });

            return siloHost;
        }

        private ITestSiloHost CreateSiloHost(string applicationBase)
        {
            AppDomainSetup setup = GetAppDomainSetupInfo(applicationBase, "Silo.dll");
            var appDomain = AppDomain.CreateDomain("Silo_" + Guid.NewGuid(), null, setup);

            var siloHost = (ITestSiloHost) appDomain.CreateInstanceAndUnwrap(
                    "Silo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                    "Silo.TestSiloHost",
                    false,
                    BindingFlags.Default,
                    null,
                    new object[] { this.connectionString, this.serviceId, this.clusterId },
                    CultureInfo.CurrentCulture,
                    new object[] { });

            return siloHost;
        }

        private static AppDomainSetup GetAppDomainSetupInfo(string applicationBase, string dllName)
        {
            var currentAppDomain = AppDomain.CurrentDomain;

            return new AppDomainSetup
            {
                ApplicationBase = string.IsNullOrEmpty(applicationBase) ? Environment.CurrentDirectory : applicationBase,
                ConfigurationFile = Path.Combine(applicationBase, $"{dllName}.config"),
                ShadowCopyFiles = currentAppDomain.SetupInformation.ShadowCopyFiles,
                ShadowCopyDirectories = currentAppDomain.SetupInformation.ShadowCopyDirectories,
                CachePath = currentAppDomain.SetupInformation.CachePath
            };
        }
    }
}
