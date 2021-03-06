namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.Topology.Sending
{
    using System.Linq;
    using NServiceBus.Azure.WindowsAzureServiceBus.Tests.TestUtils;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.Routing;
    using NServiceBus.Settings;
    using NUnit.Framework;

    [TestFixture]
    [Category("AzureServiceBus")]
    public class When_sending_through_EndpointOrientedTopology
    {
        [Test]
        public void Determines_that_sends_go_to_a_single_queue()
        {
            // setting up the environment
            var container = new TransportPartsContainer();

            var topology = SetupEndpointOrientedTopology(container, "sales");

            var destination = topology.DetermineSendDestination("operations");

            Assert.IsTrue(destination.Entities.Single().Type == EntityType.Queue);
            Assert.IsTrue(destination.Entities.Single().Path == "operations");
        }

        [Test]
        public void Determines_that_sends_go_to_a_single_topic_owned_by_the_endpoint()
        {
            var container = new TransportPartsContainer();

            var topology = SetupEndpointOrientedTopology(container, "sales");

            var destination = topology.DeterminePublishDestination(typeof(SomeMessageType));

            Assert.IsTrue(destination.Entities.Single().Type == EntityType.Topic);
            Assert.IsTrue(destination.Entities.Single().Path == "sales.events");
        }

        ITopologySectionManager SetupEndpointOrientedTopology(TransportPartsContainer container, string enpointname)
        {
            var settings = new SettingsHolder();
            settings.Set<Conventions>(new Conventions());
            container.Register(typeof(SettingsHolder), () => settings);
            var extensions = new AzureServiceBusTopologySettings(settings);
            settings.SetDefault<EndpointName>(new EndpointName(enpointname));
            extensions.NamespacePartitioning().AddNamespace("namespace1", AzureServiceBusConnectionString.Value);

            var topology = new EndpointOrientedTopology(container);

            topology.Initialize(settings);

            return container.Resolve<ITopologySectionManager>();
        }

        class SomeMessageType
        {
        }
    }
}