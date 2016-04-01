namespace NServiceBus.AzureServiceBus
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using NServiceBus.Settings;

    class MessagingFactoryCreator : ICreateMessagingFactories
    {
        readonly IManageNamespaceManagerLifeCycle _namespaceManagers;
        Func<string, MessagingFactorySettings> _settingsFactory;
        readonly ReadOnlySettings _settings;

        public MessagingFactoryCreator(IManageNamespaceManagerLifeCycle namespaceManagers, ReadOnlySettings settings)
        {
            this._namespaceManagers = namespaceManagers;
            this._settings = settings;

            if (settings.HasExplicitValue(WellKnownConfigurationKeys.Connectivity.MessagingFactories.MessagingFactorySettingsFactory))
            {
                _settingsFactory = settings.Get<Func<string, MessagingFactorySettings>>(WellKnownConfigurationKeys.Connectivity.MessagingFactories.MessagingFactorySettingsFactory);
            }
            else
            {
                _settingsFactory = namespaceName =>
                {
                    var namespaceManager = _namespaceManagers.Get(namespaceName);

                    var s = new MessagingFactorySettings
                    {
                        TokenProvider = namespaceManager.Settings.TokenProvider,
                        NetMessagingTransportSettings =
                        {
                            BatchFlushInterval = settings.Get<TimeSpan>(WellKnownConfigurationKeys.Connectivity.MessagingFactories.BatchFlushInterval)
                        }
                    };

                    return s;
                };
            }
        }

        public IMessagingFactory Create(string namespaceName)
        {
            var namespaceManager = _namespaceManagers.Get(namespaceName);
            var factorySettings = _settingsFactory(namespaceName);

            // mszcool - This type of instantiation does not work with Service Bus for Windows Server 1.1
            //           Reason: it does not work with different ports for namespace manager and transport-operations which is a common setup
            var namespacesDefinition = _settings.Get<NamespaceConfigurations>(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Namespaces);
            var inner = default(MessagingFactory);
            if (namespacesDefinition.GetIsPrivateCloud(namespaceName))
            {
                var connectionString = namespacesDefinition.GetConnectionString(namespaceName);
                inner = MessagingFactory.CreateFromConnectionString(connectionString);
            }
            else
            {
                inner = MessagingFactory.Create(namespaceManager.Address, factorySettings);
            }

            if (_settings.HasExplicitValue(WellKnownConfigurationKeys.Connectivity.MessagingFactories.RetryPolicy))
            {
                inner.RetryPolicy = _settings.Get<RetryPolicy>(WellKnownConfigurationKeys.Connectivity.MessagingFactories.RetryPolicy);
            }
            return new MessagingFactoryAdapter(inner);
        }
        
    }
}