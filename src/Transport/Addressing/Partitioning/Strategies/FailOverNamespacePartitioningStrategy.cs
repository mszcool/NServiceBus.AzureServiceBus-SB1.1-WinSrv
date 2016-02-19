﻿namespace NServiceBus.AzureServiceBus.Addressing
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using NServiceBus.Settings;

    public class FailOverNamespacePartitioningStrategy : INamespacePartitioningStrategy
    {
        private readonly NamespacesDefinition _namespaces;

        public FailOverNamespacePartitioningStrategy(ReadOnlySettings settings)
        {
            if (!settings.TryGet(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Namespaces, out _namespaces) || _namespaces.Count != 2)
            {
                throw new ConfigurationErrorsException("The 'FailOver' namespace partitioning strategy requires exactly two namespaces to be configured");
            }
        }

        public FailOverMode Mode { get; set; }

        public IEnumerable<NamespaceInfo> GetNamespaces(string endpointName, PartitioningIntent partitioningIntent)
        {
            var primary = _namespaces.First();
            var secondary = _namespaces.Last();

            if (partitioningIntent == PartitioningIntent.Sending)
            {
                yield return Mode == FailOverMode.Primary
                ? new NamespaceInfo(primary.ConnectionString, NamespaceMode.Active)
                : new NamespaceInfo(secondary.ConnectionString, NamespaceMode.Active);
            }

            if (partitioningIntent == PartitioningIntent.Creating || partitioningIntent == PartitioningIntent.Receiving)
            {
                yield return new NamespaceInfo(primary.ConnectionString, Mode == FailOverMode.Primary ? NamespaceMode.Active : NamespaceMode.Passive);
                yield return new NamespaceInfo(secondary.ConnectionString, Mode == FailOverMode.Secondary ? NamespaceMode.Active : NamespaceMode.Passive);
            }
        }
    }

    public enum FailOverMode
    {
        Primary,
        Secondary
    }
}