namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.Configuration
{
    using System;
    using Microsoft.ServiceBus;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.Configuration.AdvanceExtensibility;
    using NServiceBus.Settings;
    using NUnit.Framework;

    [TestFixture]
    [Category("AzureServiceBus")]
    public class When_configuring_messaging_factories
    {
        [Test]
        public void Should_be_able_to_set_retrypolicy()
        {
            var settings = new SettingsHolder();
            var extensions = new TransportExtensions<AzureServiceBusTransport>(settings);

            var connectivitySettings = extensions.MessagingFactories().RetryPolicy(RetryPolicy.NoRetry);

            Assert.IsInstanceOf<NoRetry>(connectivitySettings.GetSettings().Get<RetryPolicy>(WellKnownConfigurationKeys.Connectivity.MessagingFactories.RetryPolicy));
        }

        [Test]
        public void Should_be_able_to_set_batchflushinterval()
        {
            var settings = new SettingsHolder();
            var extensions = new TransportExtensions<AzureServiceBusTransport>(settings);

            var connectivitySettings = extensions.MessagingFactories().BatchFlushInterval(TimeSpan.FromSeconds(0));

            Assert.AreEqual(TimeSpan.FromSeconds(0), connectivitySettings.GetSettings().Get<TimeSpan>(WellKnownConfigurationKeys.Connectivity.MessagingFactories.BatchFlushInterval));
        }

    }
}