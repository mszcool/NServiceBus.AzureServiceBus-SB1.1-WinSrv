namespace NServiceBus.AzureServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NServiceBus.AzureServiceBus.Addressing;
    using NServiceBus.AzureServiceBus.Topology.MetaModel;
    using NServiceBus.Settings;
    using NServiceBus.Transports;

    public class ForwardingTopology : ITopology
    {
        ITopologySectionManager topologySectionManager;
        ITransportPartsContainer container;

        public ForwardingTopology() : this(new TransportPartsContainer()) { }

        internal ForwardingTopology(ITransportPartsContainer container)
        {
            this.container = container;
        }

        public bool HasNativePubSubSupport => true;
        public bool HasSupportForCentralizedPubSub => true;
        public bool NeedsMappingConfigurationBetweenPublishersAndEventTypes => false;

        public void Initialize(SettingsHolder settings)
        {
            ApplyDefaults(settings);
            InitializeContainer(settings);
        }

        private void ApplyDefaults(SettingsHolder settings)
        {
            new DefaultConfigurationValues().Apply(settings);
            // ensures settings are present/correct
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Addressing.Composition.Strategy, typeof(FlatComposition));
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Addressing.Individualization.Strategy, typeof(DiscriminatorBasedIndividualization));
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Strategy, typeof(SingleNamespacePartitioning));
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Addressing.Sanitization.Strategy, typeof(AdjustmentSanitization));
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Addressing.Validation.Strategy, typeof(EntityNameValidationRules));
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Bundling.NumberOfEntitiesInBundle, 2);
            settings.SetDefault(WellKnownConfigurationKeys.Topology.Bundling.BundlePrefix, "bundle-");
            topologySectionManager = new ForwardingTopologySectionManager(settings, container);
        }

        private void InitializeContainer(SettingsHolder settings)
        {
            // runtime components
            container.Register<ReadOnlySettings>(() => settings);
            container.Register<ITopologySectionManager>(() => topologySectionManager);
            container.RegisterSingleton<NamespaceManagerCreator>();
            container.RegisterSingleton<NamespaceManagerLifeCycleManager>();
            container.RegisterSingleton<MessagingFactoryCreator>();
            container.RegisterSingleton<MessagingFactoryLifeCycleManager>();
            container.RegisterSingleton<MessageReceiverCreator>();
            container.RegisterSingleton<MessageReceiverLifeCycleManager>();
            container.RegisterSingleton<MessageSenderCreator>();
            container.RegisterSingleton<MessageSenderLifeCycleManager>();
            container.RegisterSingleton<AzureServiceBusQueueCreator>();
            container.RegisterSingleton<AzureServiceBusTopicCreator>();
            container.RegisterSingleton<AzureServiceBusForwardingSubscriptionCreator>();

            container.RegisterSingleton<DefaultConnectionStringToNamespaceNameMapper>();
            container.RegisterSingleton(settings.Get<Type>(WellKnownConfigurationKeys.Topology.Addressing.UseNamespaceNamesInsteadOfConnectionStrings));

            container.Register<DefaultBrokeredMessagesToIncomingMessagesConverter>();
            container.Register<DefaultBatchedOperationsToBrokeredMessagesConverter>();
            container.Register<TopologyCreator>();
            container.Register<Batcher>();

            var oversizedMessageHandler = (IHandleOversizedBrokeredMessages)settings.Get(WellKnownConfigurationKeys.Connectivity.MessageSenders.OversizedBrokeredMessageHandlerInstance);
            container.Register<IHandleOversizedBrokeredMessages>(() => oversizedMessageHandler);

            container.RegisterSingleton<DefaultOutgoingBatchRouter>();
            container.RegisterSingleton<TopologyOperator>();
            container.Register<MessageReceiverNotifier>();
            container.RegisterSingleton<SubscriptionManager>();
            container.RegisterSingleton<TransportResourcesCreator>();
            container.RegisterSingleton<Dispatcher>();
            container.Register<MessagePump>();

            var compositionStrategyType = (Type)settings.Get(WellKnownConfigurationKeys.Topology.Addressing.Composition.Strategy);
            container.Register(compositionStrategyType);

            var individualizationStrategyType = (Type)settings.Get(WellKnownConfigurationKeys.Topology.Addressing.Individualization.Strategy);
            container.Register(individualizationStrategyType);

            var partitioningStrategyType = (Type)settings.Get(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Strategy);
            container.Register(partitioningStrategyType);

            var sanitizationStrategyType = (Type)settings.Get(WellKnownConfigurationKeys.Topology.Addressing.Sanitization.Strategy);
            container.Register(sanitizationStrategyType);

            var validationStrategyType = (Type)settings.Get(WellKnownConfigurationKeys.Topology.Addressing.Validation.Strategy);
            container.Register(validationStrategyType);
        }

        public Func<ICreateQueues> GetQueueCreatorFactory()
        {
            return () => container.Resolve<ICreateQueues>();
        }
        
        public Func<IPushMessages> GetMessagePumpFactory()
        {
            return () =>
            {
                var pump = container.Resolve<MessagePump>();
                return pump;
            };
        }

        public Func<IDispatchMessages> GetDispatcherFactory()
        {
            return () => container.Resolve<IDispatchMessages>();
        }

        public Func<IManageSubscriptions> GetSubscriptionManagerFactory()
        {
            return () => container.Resolve<IManageSubscriptions>();
        }

        public async Task<StartupCheckResult> RunPreStartupChecks()
        {
            var settings = container.Resolve<ReadOnlySettings>();

            var manageRightsCheck = new ManageRightsCheck(container.Resolve<IManageNamespaceManagerLifeCycle>(), settings);
            var topicPartitioningCheck = new TopicPartitioningCheckForForwardingTopology(settings);

            var results = new List<StartupCheckResult>
            {
                await manageRightsCheck.Run().ConfigureAwait(false),
                await topicPartitioningCheck.Run().ConfigureAwait(false)
            };

            if (results.Any(x => x.Succeeded == false))
            {
                return StartupCheckResult.Failed(string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage)));
            }

            return StartupCheckResult.Success;
        }

        public OutboundRoutingPolicy GetOutboundRoutingPolicy()
        {
            return new OutboundRoutingPolicy(OutboundRoutingType.Unicast, OutboundRoutingType.Multicast, OutboundRoutingType.Unicast);
        }
    }
}