namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.Creation
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using NServiceBus.Azure.WindowsAzureServiceBus.Tests.TestUtils;
    using NServiceBus.AzureServiceBus;
    using NServiceBus.Settings;
    using NUnit.Framework;

    [TestFixture]
    [Category("AzureServiceBus")]
    public class When_creating_queues
    {
        [Test]
        public async Task Does_not_create_queues_when_createqueues_is_set_to_false()
        {
            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());

            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            //make sure there is no leftover from previous test
            await namespaceManager.DeleteQueue("myqueue");

            settings.Set("Transport.CreateQueues", false);
            
            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            Assert.IsFalse(await namespaceManager.QueueExists("myqueue"));
        }

        [Test]
        public async Task Uses_queue_description_when_provided_by_user()
        {
            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var descriptionToUse = new QueueDescription("myqueue");
            
            extensions.Queues().DescriptionFactory((name, s) => descriptionToUse);

            var creator = new AzureServiceBusQueueCreator(settings);

            var description = await creator.Create("myqueue", namespaceManager);

            Assert.IsTrue(await namespaceManager.QueueExists("myqueue"));
            Assert.AreEqual(descriptionToUse, description);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_ForwardDeadLetteredMessagesTo_on_the_created_entity()
        {
            // mszcool - not supported by Service Bus 1.1 for Windows Server
            await Task.Run(() => Assert.Inconclusive("Not supported by Service Bus 1.1 for Windows Server!"));

            //var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            //// forwarding queue needs to exist before you can use it as a forwarding target
            //// needs to be created with different settings as it cannot forward to itself obviously
            //var originalsettings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            //var originalcreator = new AzureServiceBusQueueCreator(originalsettings);
            //await originalcreator.Create("myotherqueue", namespaceManager);

            //// actual test
            //var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            //var extensions = new AzureServiceBusTopologySettings(settings);

            //extensions.Queues().ForwardDeadLetteredMessagesTo("myotherqueue");

            //var creator = new AzureServiceBusQueueCreator(settings);

            //await creator.Create("myqueue", namespaceManager);

            //var real = await namespaceManager.GetQueue("myqueue");

            //Assert.IsTrue(real.ForwardDeadLetteredMessagesTo.EndsWith("myotherqueue"));

            ////cleanup 
            //await namespaceManager.DeleteQueue("myqueue");
            //await namespaceManager.DeleteQueue("myotherqueue");
        }

        [Test]
        public async Task Properly_sets_ForwardDeadLetteredMessagesTo_on_the_created_entity_that_qualifies_condition()
        {
            // mszcool - not supported by Service Bus 1.1 for Windows Server
            await Task.Run(() => Assert.Inconclusive("Not supported by Service Bus 1.1 for Windows Server!"));

            //var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            //var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            //var extensions = new AzureServiceBusTopologySettings(settings);

            //extensions.Queues().ForwardDeadLetteredMessagesTo(name => name == "myqueue", "myotherqueue");

            //var creator = new AzureServiceBusQueueCreator(settings);

            //await creator.Create("myotherqueue", namespaceManager);
            //await creator.Create("myqueue", namespaceManager);

            //var real = await namespaceManager.GetQueue("myqueue");
            //var forwardReal = await namespaceManager.GetQueue("myotherqueue");

            //Assert.IsTrue(real.ForwardDeadLetteredMessagesTo.EndsWith("myotherqueue"));
            //Assert.IsTrue(string.IsNullOrEmpty(forwardReal.ForwardDeadLetteredMessagesTo));

            ////cleanup 
            //await namespaceManager.DeleteQueue("myqueue");
            //await namespaceManager.DeleteQueue("myotherqueue");
        }

        [Test]
        public async Task Properly_sets_EnableExpress_on_the_created_entity()
        {
            // mszcool - not supported by Service Bus 1.1 for Windows Server
            await Task.Run(() => Assert.Inconclusive("Not supported by Service Bus 1.1 for Windows Server!"));

            //var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            //var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            //var extensions = new AzureServiceBusTopologySettings(settings);

            //extensions.Queues().EnableExpress(true);

            //var creator = new AzureServiceBusQueueCreator(settings);

            //await creator.Create("myqueue", namespaceManager);

            //var real = await namespaceManager.GetQueue("myqueue");

            //Assert.IsTrue(real.EnableExpress);

            ////cleanup 
            //await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_EnableExpress_on_the_created_entity_that_qualifies_condition()
        {
            // mszcool - not supported by Service Bus 1.1 for Windows Server
            await Task.Run(() => Assert.Inconclusive("Not supported by Service Bus 1.1 for Windows Server!"));

            //var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            //var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            //var extensions = new AzureServiceBusTopologySettings(settings);

            //extensions.Queues().EnableExpress(name => name == "myqueue", true);

            //var creator = new AzureServiceBusQueueCreator(settings);

            //await creator.Create("myqueue", namespaceManager);

            //var real = await namespaceManager.GetQueue("myqueue");

            //Assert.IsTrue(real.EnableExpress, "Queue should be marked as express, but it wasn't.");

            ////cleanup 
            //await namespaceManager.DeleteQueue("myqueue");
        }


        [Test]
        public async Task Properly_sets_AutoDeleteOnIdle_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().AutoDeleteOnIdle(TimeSpan.FromDays(1));

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.AreEqual(TimeSpan.FromDays(1), real.AutoDeleteOnIdle);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_EnablePartitioning_on_the_created_entity()
        {
            // mszcool - not supported by Service Bus 1.1 for Windows Server
            await Task.Run(() => Assert.Inconclusive("Not supported by Service Bus 1.1 for Windows Server!"));

            //var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            ////make sure any previously created queues with this name are removed as the EnablePartitioning cannot be updated
            //await namespaceManager.DeleteQueue("myqueue");

            //var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            //var extensions = new AzureServiceBusTopologySettings(settings);

            //extensions.Queues().EnablePartitioning(true);

            //var creator = new AzureServiceBusQueueCreator(settings);

            //await creator.Create("myqueue", namespaceManager);

            //var real = await namespaceManager.GetQueue("myqueue");

            //Assert.IsTrue(real.EnablePartitioning);

            ////cleanup 
            //await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_EnableBatchedOperations_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().EnableBatchedOperations(false);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.IsFalse(real.EnableBatchedOperations);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_MaxDeliveryCount_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().MaxDeliveryCount(10);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.AreEqual(10, real.MaxDeliveryCount);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_DuplicateDetectionHistoryTimeWindow_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().DuplicateDetectionHistoryTimeWindow(TimeSpan.FromMinutes(20));

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.AreEqual(TimeSpan.FromMinutes(20), real.DuplicateDetectionHistoryTimeWindow);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_EnableDeadLetteringOnMessageExpiration_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().EnableDeadLetteringOnMessageExpiration(true);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.IsTrue(real.EnableDeadLetteringOnMessageExpiration);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_DefaultMessageTimeToLive_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().DefaultMessageTimeToLive(TimeSpan.FromDays(1));

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.AreEqual(TimeSpan.FromDays(1), real.DefaultMessageTimeToLive);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_RequiresSession_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            //make sure any existing queue is cleaned up as the RequiresSession property cannot be changed on existing queues
            await namespaceManager.DeleteQueue("myqueue");

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().RequiresSession(true);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.IsTrue(real.RequiresSession);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_RequiresDuplicateDetection_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().RequiresDuplicateDetection(true);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.IsTrue(real.RequiresDuplicateDetection);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_MaxSizeInMegabytes_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().MaxSizeInMegabytes(SizeInMegabytes.Size3072);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.AreEqual(3072, real.MaxSizeInMegabytes);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_LockDuration_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().LockDuration(TimeSpan.FromMinutes(5));

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.AreEqual(TimeSpan.FromMinutes(5), real.LockDuration);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Properly_sets_SupportOrdering_on_the_created_entity()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);

            extensions.Queues().SupportOrdering(true);

            var creator = new AzureServiceBusQueueCreator(settings);

            await creator.Create("myqueue", namespaceManager);

            var real = await namespaceManager.GetQueue("myqueue");

            Assert.IsTrue(real.SupportOrdering);

            //cleanup 
            await namespaceManager.DeleteQueue("myqueue");
        }

        [Test]
        public async Task Should_be_able_to_update_an_existing_queue_with_new_property_values()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));
            await namespaceManager.CreateQueue(new QueueDescription("existingqueue"));

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);
            extensions.Queues().DescriptionFactory((queuePath, readOnlySettings) => new QueueDescription(queuePath)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(100),
                // mszcool - not supported by Service Bus 1.1 for Windows Server
                // EnableExpress = true
            });

            var creator = new AzureServiceBusQueueCreator(settings);
            await creator.Create("existingqueue", namespaceManager);

            var queueDescription = await namespaceManager.GetQueue("existingqueue");
            Assert.AreEqual(TimeSpan.FromMinutes(100), queueDescription.AutoDeleteOnIdle);
            
            //cleanup 
            await namespaceManager.DeleteQueue("existingqueue");
        }

        [Test]
        public async Task Should_be_able_to_update_an_existing_queue_with_new_property_values_without_failing_on_readonly_properties()
        {
            var namespaceManager = new NamespaceManagerAdapter(NamespaceManager.CreateFromConnectionString(AzureServiceBusConnectionString.Value));
            await namespaceManager.CreateQueue(new QueueDescription("existingqueue")
            {
                LockDuration = TimeSpan.FromSeconds(50),
                RequiresDuplicateDetection = true,
                // mszcool - not supported by Service Bus 1.1 for Windows Server
                // EnablePartitioning = true,
                RequiresSession = true,
            });

            var settings = new DefaultConfigurationValues().Apply(new SettingsHolder());
            var extensions = new AzureServiceBusTopologySettings(settings);
            extensions.Queues().DescriptionFactory((queuePath, readOnlySettings) => new QueueDescription(queuePath)
            {
                LockDuration = TimeSpan.FromSeconds(50),
                RequiresDuplicateDetection = false,
                // mszcool - not supported by Service Bus 1.1 for Windows Server
                // EnablePartitioning = false,
                RequiresSession = false,
            });

            var creator = new AzureServiceBusQueueCreator(settings);
            Assert.Throws<ArgumentException>(async () =>  await creator.Create("existingqueue", namespaceManager));
            
            //cleanup 
            await namespaceManager.DeleteQueue("existingqueue");
        }
    }
}