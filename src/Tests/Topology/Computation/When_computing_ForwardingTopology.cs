namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.Topology.Computation
{
    using System.Linq;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.Routing;
    using NServiceBus.Settings;
    using NUnit.Framework;

    [TestFixture]
    [Category("AzureServiceBus")]
    public class When_computing_ForwardingTopology
    {
        private const string Connectionstring = "Endpoint=sb://namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=somesecretkey";
        private const string Name = "name";

        [Test]
        public void Determines_the_namespace_from_partitioning_strategy()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var topology = new ForwardingTopology(container);

            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();
            var definition = sectionManager.DetermineResourcesToCreate();

            var namespaceInfo = new RuntimeNamespaceInfo(Name, Connectionstring, NamespaceMode.Active);
            Assert.IsTrue(definition.Namespaces.Any(nsi => nsi == namespaceInfo));
        }

        [Test]
        public void Determines_there_should_be_a_queue_with_same_name_as_endpointname()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var topology = new ForwardingTopology(container);

            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();
            var definition = sectionManager.DetermineResourcesToCreate();

            Assert.AreEqual(1, definition.Entities.Count(ei => ei.Path == "sales" && ei.Type == EntityType.Queue && ei.Namespace.ConnectionString == Connectionstring));
        }

        [Test]
        public void Determines_there_should_be_a_topic_bundle_created()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var topology = new ForwardingTopology(container);

            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();
            var definition = sectionManager.DetermineResourcesToCreate();

            var result = definition.Entities.Where(ei => ei.Type == EntityType.Topic && ei.Namespace.ConnectionString == Connectionstring && ei.Path.StartsWith("bundle-"));

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Has.Exactly(1).Matches<EntityInfo>(x => x.Path == "bundle-1"));
            Assert.That(result, Has.Exactly(1).Matches<EntityInfo>(x => x.Path == "bundle-2"));
        }

        [Test]
        public void Creates_subscription_on_each_topic_in_bundle()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.UseTopology<ForwardingTopology>().NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var topology = new ForwardingTopology(container);

            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();
            sectionManager.DetermineResourcesToCreate();

            var section = sectionManager.DetermineResourcesToSubscribeTo(typeof(SomeEvent));

            Assert.That(section.Entities.Count(), Is.EqualTo(2));
            // TODO: need to verify that subscription is done on each topic
        }

        [Test]
        public void Creates_subscription_path_matching_the_subscribing_endpoint_name()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.UseTopology<ForwardingTopology>().NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var topology = new ForwardingTopology(container);

            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();
            sectionManager.DetermineResourcesToCreate();

            var section = sectionManager.DetermineResourcesToSubscribeTo(typeof(SomeEvent));

            Assert.IsTrue(section.Entities.All(e => e.Path == "sales"), "Subscription name should be matching subscribing endpoint name, but it wasn't.");
        }

        [Test]
        public void Should_creates_subscription_entities_marked_as_not_be_listened_to()
        {
            var container = new TransportPartsContainer();

            var settings = new SettingsHolder();
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);

            settings.SetDefault<EndpointName>(new EndpointName("sales"));
            extensions.UseTopology<ForwardingTopology>().NamespacePartitioning().AddNamespace(Name, Connectionstring);

            var topology = new ForwardingTopology(container);
            topology.Initialize(settings);

            var sectionManager = container.Resolve<ITopologySectionManager>();
            sectionManager.DetermineResourcesToCreate();

            var section = sectionManager.DetermineResourcesToSubscribeTo(typeof(SomeEvent));

            Assert.IsFalse(section.Entities.Any(x => x.ShouldBeListenedTo));
        }


        class SomeEvent
        {
        }
    }
}
