namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.Topology.Computation
{
    using System.Linq;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.Routing;
    using NServiceBus.Settings;
    using NUnit.Framework;

    [TestFixture]
    [Category("AzureServiceBus")]
    public class When_computing_EndpointOrientedTopology
    {
        private static readonly string Connectionstring = "Endpoint=sb://namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=somesecretkey";
        private static readonly string Name = "name";

        [Test]
        public void Determines_the_namespace_from_partitioning_strategy()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            settings.Set<Conventions>(new Conventions());
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);
            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            
            extensions.NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var definition = DetermineResourcesToCreate(settings, container);

            var namespaceInfo = new RuntimeNamespaceInfo(Name, Connectionstring, NamespaceMode.Active);
            Assert.IsTrue(definition.Namespaces.Any(nsi => nsi == namespaceInfo));
        }

        [Test]
        public void Determines_there_should_be_a_queue_with_same_name_as_endpointname()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            settings.Set<Conventions>(new Conventions());
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var definition = DetermineResourcesToCreate(settings, container);

            Assert.AreEqual(1, definition.Entities.Count(ei => ei.Path == "sales" && ei.Type == EntityType.Queue && ei.Namespace.ConnectionString == Connectionstring));
        }

        [Test]
        public void Determines_there_should_be_a_topic_with_same_name_as_endpointname_followed_by_dot_events()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            settings.Set<Conventions>(new Conventions());
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var definition = DetermineResourcesToCreate(settings, container);

            Assert.AreEqual(1, definition.Entities.Count(ei => ei.Path == "sales.events" && ei.Type == EntityType.Topic && ei.Namespace.ConnectionString == Connectionstring));
        }

        private static TopologySection DetermineResourcesToCreate(SettingsHolder settings, TransportPartsContainer container)
        {
            var topology = new EndpointOrientedTopology(container);

            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();

            var definition = sectionManager.DetermineResourcesToCreate();
            return definition;
        }
    }

}
