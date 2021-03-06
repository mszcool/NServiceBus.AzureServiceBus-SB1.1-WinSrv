namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.Addressing.NamespacePartitioning
{
    using System.Configuration;
    using System.Linq;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.AzureServiceBus.Addressing;
    using NServiceBus.Settings;
    using NUnit.Framework;

    [TestFixture]
    [Category("AzureServiceBus")]
    public class When_using_roundrobin_strategy_on_multiple_namespaces
    {
        private static readonly string PrimaryConnectionString = "Endpoint=sb://namespace1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=somesecretkey";
        private static readonly string SecondaryConnectionString = "Endpoint=sb://namespace2.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=somesecretkey";
        private static readonly string TertiaryConnectionString = "Endpoint=sb://namespace3.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=somesecretkey";

        private static readonly string PrimaryName = "namespace1";
        private static readonly string SecondaryName = "namespace2";
        private static readonly string TertiaryName = "namespace3";

        private RoundRobinNamespacePartitioning _strategy;

        [SetUp]
        public void SetUp()
        {
            var settings = new SettingsHolder();
            var extensions = new AzureServiceBusTopologySettings(settings);
            extensions.NamespacePartitioning().AddNamespace(PrimaryName, PrimaryConnectionString);
            extensions.NamespacePartitioning().AddNamespace(SecondaryName, SecondaryConnectionString);
            extensions.NamespacePartitioning().AddNamespace(TertiaryName, TertiaryConnectionString);

            _strategy = new RoundRobinNamespacePartitioning(settings);
        }

        [Test]
        public void Roundrobin_partitioning_strategy_will_return_a_single_connectionstring_for_purpose_of_sending()
        {
            var namespaces = _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending);

            Assert.AreEqual(1, namespaces.Count());
        }

        [Test]
        public void Roundrobin_partitioning_strategy_will_return_all_connectionstrings_for_purpose_of_creating()
        {
            var namespaces = _strategy.GetNamespaces("endpoint1", PartitioningIntent.Creating);

            Assert.AreEqual(3, namespaces.Count());
        }

        [Test]
        public void Roundrobin_partitioning_strategy_will_return_all_connectionstrings_for_purpose_of_receiving()
        {
            var namespaces = _strategy.GetNamespaces("endpoint1", PartitioningIntent.Receiving);

            Assert.AreEqual(3, namespaces.Count());
        }

        [Test]
        public void Roundrobin_partitioning_strategy_will_circle_active_namespace()
        {
            Assert.AreEqual(new RuntimeNamespaceInfo(PrimaryName, PrimaryConnectionString, NamespaceMode.Active), _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending).First());
            Assert.AreEqual(new RuntimeNamespaceInfo(SecondaryName, SecondaryConnectionString, NamespaceMode.Active), _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending).First());
            Assert.AreEqual(new RuntimeNamespaceInfo(TertiaryName, TertiaryConnectionString, NamespaceMode.Active), _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending).First());

            Assert.AreEqual(new RuntimeNamespaceInfo(PrimaryName, PrimaryConnectionString, NamespaceMode.Active), _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending).First());
            Assert.AreEqual(new RuntimeNamespaceInfo(SecondaryName, SecondaryConnectionString, NamespaceMode.Active), _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending).First());
            Assert.AreEqual(new RuntimeNamespaceInfo(TertiaryName, TertiaryConnectionString, NamespaceMode.Active), _strategy.GetNamespaces("endpoint1", PartitioningIntent.Sending).First());
        }
        
        [Test]
        public void Roundrobin_partitioning_strategy_will_throw_if_no_namespace_are_provided()
        {
            var settings = new SettingsHolder();

            Assert.Throws<ConfigurationErrorsException>(() => new RoundRobinNamespacePartitioning(settings));
        }

        [Test]
        public void Roundrobin_partitioning_strategy_will_throw_if_too_little_namespaces_are_provided()
        {
            var settings = new SettingsHolder();
            var extensions = new AzureServiceBusTopologySettings(settings);
            extensions.NamespacePartitioning().AddNamespace(PrimaryName, PrimaryConnectionString);
            
            Assert.Throws<ConfigurationErrorsException>(() => new RoundRobinNamespacePartitioning(settings));
        }

    }
}