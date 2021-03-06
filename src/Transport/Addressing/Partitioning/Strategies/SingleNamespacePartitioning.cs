namespace NServiceBus.AzureServiceBus.Addressing
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using NServiceBus.Settings;

    public class SingleNamespacePartitioning : INamespacePartitioningStrategy
    {
        private readonly NamespaceConfigurations namespaces;

        public SingleNamespacePartitioning(ReadOnlySettings settings)
        {
            if (!settings.TryGet(WellKnownConfigurationKeys.Topology.Addressing.Partitioning.Namespaces, out namespaces) || namespaces.Count != 1)
            {
                throw new ConfigurationErrorsException("The 'Single' namespace partitioning strategy requires exactly one namespace, please configure the connection string to your azure servicebus namespace");
            }
        }

        public IEnumerable<RuntimeNamespaceInfo> GetNamespaces(string endpointName, PartitioningIntent partitioningIntent)
        {
            var @namespace = namespaces.First();
            yield return new RuntimeNamespaceInfo(@namespace.Name, @namespace.ConnectionString, NamespaceMode.Active);
        }
    }
}