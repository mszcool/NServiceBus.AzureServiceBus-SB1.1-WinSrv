namespace NServiceBus
{
    using NServiceBus.AzureServiceBus;
    using NServiceBus.AzureServiceBus.Addressing;
    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.Settings;

    public class AzureServiceBusSanitizationSettings : ExposeSettings
    {
         SettingsHolder settings;

         public AzureServiceBusSanitizationSettings(SettingsHolder settings)
            : base(settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Rules to apply for entity path/name sanitization.
        /// <remarks> Default is <see cref="AdjustmentSanitization"/>. For backwards compatibility, use <see cref="AdjustmentSanitizationV6"/>.</remarks>
        /// </summary>
        public AzureServiceBusSanitizationSettings UseStrategy<T>() where T : ISanitizationStrategy
         {
             settings.Set(WellKnownConfigurationKeys.Topology.Addressing.Sanitization.Strategy, typeof(T));

             return this;
         }
    }
}