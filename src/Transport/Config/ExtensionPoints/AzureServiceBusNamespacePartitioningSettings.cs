﻿namespace NServiceBus
{
    using NServiceBus.AzureServiceBus;
    using NServiceBus.AzureServiceBus.Addressing;
    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.Settings;

    public class AzureServiceBusNamespacePartitioningSettings : ExposeSettings
    {
        SettingsHolder settings;

        public AzureServiceBusNamespacePartitioningSettings(SettingsHolder settings)
            : base(settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Namespace partitioning strategy to use.
        /// <remarks> Default is <see cref="SingleNamespacePartitioning"/>. 
        /// Additional strategies are <see cref="RoundRobinNamespacePartitioning"/>,
        /// <see cref="FailOverNamespacePartitioning"/>,
        /// <see cref="ShardedNamespacePartitioning"/>,
        /// <see cref="ReplicatedNamespacePartitioning"/>.
        /// </remarks>
        /// </summary>
        public AzureServiceBusNamespacePartitioningSettings UseStrategy<T>() where T : INamespacePartitioningStrategy
        {
            settings.Set(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Strategy, typeof(T));

            return this;
        }

        public AzureServiceBusNamespacePartitioningSettings AddNamespace(string name, string connectionString)
        {
            NamespaceConfigurations namespaces;
            if (!settings.TryGet(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Namespaces, out namespaces))
            {
                namespaces = new NamespaceConfigurations();
                settings.Set(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Namespaces, namespaces);
            }
            
            namespaces.Add(name, connectionString);
            return this;
        }
    }
}