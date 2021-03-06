﻿namespace NServiceBus.AcceptanceTests.EndpointTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NServiceBus.AcceptanceTesting.Customization;
    using NServiceBus.AcceptanceTesting.Support;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Features;

    public class ServerWithNoDefaultPersistenceDefinitions : IEndpointSetupTemplate
    {
        public ServerWithNoDefaultPersistenceDefinitions()
        {
            typesToInclude = new List<Type>();
        }

        public ServerWithNoDefaultPersistenceDefinitions(List<Type> typesToInclude)
        {
            this.typesToInclude = typesToInclude;
        }

        public async Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointConfiguration, IConfigurationSource configSource, Action<EndpointConfiguration> configurationBuilderCustomization)
        {
            var settings = runDescriptor.Settings;

            var types = endpointConfiguration.GetTypesScopedByTestClass();

            typesToInclude.AddRange(types);

            var builder = new EndpointConfiguration(endpointConfiguration.EndpointName);
            builder.TypesToIncludeInScan(typesToInclude);
            builder.CustomConfigurationSource(configSource);
            builder.EnableInstallers();

            builder.DisableFeature<TimeoutManager>();
            builder.DisableFeature<SecondLevelRetries>();
            builder.DisableFeature<FirstLevelRetries>();

            await builder.DefineTransport(settings, endpointConfiguration.EndpointName).ConfigureAwait(false);

            builder.RegisterComponentsAndInheritanceHierarchy(runDescriptor);

            configurationBuilderCustomization(builder);

            return builder;
        }

        List<Type> typesToInclude;
    }
}