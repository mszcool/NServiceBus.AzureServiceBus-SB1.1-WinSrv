﻿namespace NServiceBus.AzureServiceBus
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using NServiceBus.Logging;
    using NServiceBus.Settings;

    class AzureServiceBusSubscriptionCreatorV6 : ICreateAzureServiceBusSubscriptions
    {
        AzureServiceBusSubscriptionCreator creator;
        ILog logger = LogManager.GetLogger<AzureServiceBusSubscriptionCreatorV6>();

        public AzureServiceBusSubscriptionCreatorV6(ReadOnlySettings settings)
        {
            creator = new AzureServiceBusSubscriptionCreator(settings);
        }

        public async Task<SubscriptionDescription> Create(string topicPath, string subscriptionName, SubscriptionMetadata metadata, string sqlFilter, INamespaceManager namespaceManager, string forwardTo = null)
        {
            var subscriptionDescription = await creator.Create(topicPath, subscriptionName, metadata, sqlFilter, namespaceManager, forwardTo).ConfigureAwait(false);

            if (await SubscriptionIsReusedAcrossDifferentNamespaces(subscriptionDescription, sqlFilter, namespaceManager).ConfigureAwait(false))
            {
                logger.Debug("Creating subscription using event type full name");
                subscriptionDescription = await creator.Create(topicPath, metadata.SubscriptionNameBasedOnEventWithNamespace, metadata, sqlFilter, namespaceManager, forwardTo).ConfigureAwait(false);
            }

            return subscriptionDescription;
        }

        async Task<bool> SubscriptionIsReusedAcrossDifferentNamespaces(SubscriptionDescription subscriptionDescription, string sqlFilter, INamespaceManager namespaceManager)
        {
            var rules = await namespaceManager.GetRules(subscriptionDescription).ConfigureAwait(false);
            var filter = rules.First().Filter as SqlFilter;
            if (filter != null && filter.SqlExpression != sqlFilter)
            {
                logger.Debug("Looks like this subscription name is already taken as the sql filter does not match the subscribed event name.");
                return true;
            }

            return false;
        }
    }
}