namespace NServiceBus
{
    using System;
    using Microsoft.ServiceBus;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.AzureServiceBus.Topology.MetaModel;
    using NServiceBus.Configuration.AdvanceExtensibility;

    public static class AzureServiceBusTransportExtensions 
    {
        public static AzureServiceBusTopologySettings UseTopology<T>(this TransportExtensions<AzureServiceBusTransport> transportExtensions) where T : ITopology, new()
        {
            var topology = Activator.CreateInstance<T>();
            return UseTopology(transportExtensions, topology);
        }

        public static AzureServiceBusTopologySettings UseTopology<T>(this TransportExtensions<AzureServiceBusTransport> transportExtensions, Func<T> factory) where T : ITopology
        {
            return UseTopology(transportExtensions, factory());
        }

        public static AzureServiceBusTopologySettings UseTopology<T>(this TransportExtensions<AzureServiceBusTransport> transportExtensions, T topology) where T : ITopology
        {
            var settings = transportExtensions.GetSettings();
            settings.Set<ITopology>(topology);
            return new AzureServiceBusTopologySettings(settings);
        }


        public static void BrokeredMessageBodyType(this TransportExtensions<AzureServiceBusTransport> transportExtensions, SupportedBrokeredMessageBodyTypes type)
        {
            var settings = transportExtensions.GetSettings();
            settings.Set(WellKnownConfigurationKeys.Serialization.BrokeredMessageBodyType, type);
        }


        public static void NumberOfClientsPerEntity(this TransportExtensions<AzureServiceBusTransport> transportExtensions, int number)
        {
            var settings = transportExtensions.GetSettings();
            settings.Set(WellKnownConfigurationKeys.Connectivity.NumberOfClientsPerEntity, number);
        }

        public static void SendViaReceiveQueue(this TransportExtensions<AzureServiceBusTransport> transportExtensions, bool sendViaReceiveQueue)
        {
            var settings = transportExtensions.GetSettings();
            settings.Set(WellKnownConfigurationKeys.Connectivity.SendViaReceiveQueue, sendViaReceiveQueue);
        }
        public static void ConnectivityMode(this TransportExtensions<AzureServiceBusTransport> transportExtensions, ConnectivityMode connectivityMode)
        {
            var settings = transportExtensions.GetSettings();
            settings.Set(WellKnownConfigurationKeys.Connectivity.ConnectivityMode, connectivityMode);
        }


        public static AzureServiceBusMessageReceiverSettings MessageReceivers(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusMessageReceiverSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusMessageSenderSettings MessageSenders(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusMessageSenderSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusMessagingFactoriesSettings MessagingFactories(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusMessagingFactoriesSettings(transportExtensions.GetSettings());
        }
        

        public static void UseNamespaceNamesInsteadOfConnectionStrings(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            var settings = transportExtensions.GetSettings();
            settings.Set(WellKnownConfigurationKeys.Topology.Addressing.UseNamespaceNamesInsteadOfConnectionStrings, typeof(PassThroughNamespaceNameToConnectionStringMapper));
        }
        
        public static AzureServiceBusQueueSettings Queues(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusQueueSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusTopicSettings Topics(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusTopicSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusSubscriptionSettings Subscriptions(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusSubscriptionSettings(transportExtensions.GetSettings());
        }


        public static AzureServiceBusNamespacePartitioningSettings NamespacePartitioning(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusNamespacePartitioningSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusCompositionSettings Composition(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusCompositionSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusValidationSettings Validation(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusValidationSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusSanitizationSettings Sanitization(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusSanitizationSettings(transportExtensions.GetSettings());
        }

        public static AzureServiceBusIndividualizationSettings Individualization(this TransportExtensions<AzureServiceBusTransport> transportExtensions)
        {
            return new AzureServiceBusIndividualizationSettings(transportExtensions.GetSettings());
        }
    }
}