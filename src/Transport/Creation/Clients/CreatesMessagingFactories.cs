namespace NServiceBus.Azure.Transports.WindowsAzureServiceBus
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.ServiceBus.Messaging;

    class CreatesMessagingFactories : ICreateMessagingFactories
    {
        #region mszcool 2016-04-01

        // mszcool - Added Connection String parsing to detect whether a public or private cloud Service Bus is addressed!
        public static readonly string Sample = "Endpoint=sb://[namespace name].servicebus.windows.net;SharedAccessKeyName=[shared access key name];SharedAccessKey=[shared access key]";
        private static readonly string Pattern =
            "^Endpoint=sb://(?<namespaceName>[A-Za-z][A-Za-z0-9-]{4,48}[A-Za-z0-9]).servicebus.windows.net/?;SharedAccessKeyName=(?<sharedAccessPolicyName>[\\w\\W]+);SharedAccessKey=(?<sharedAccessPolicyValue>[\\w\\W]+)$";

        public static readonly string OnPremSample = "Endpoint=[namespace name];StsEndpoint=[sts endpoint address];RuntimePort=[port];ManagementPort=[port];SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[shared access key]";
        private static readonly string OnPremPattern =
            "^Endpoint=sb\\://(?<serverName>[A-Za-z][A-Za-z0-9\\-\\.]+)/(?<namespaceName>[A-Za-z][A-Za-z0-9]{4,48}[A-Za-z0-9])/?;" +
            "StsEndPoint=(?<stsEndpoint>https\\://[A-Za-z][A-Za-z0-9\\-\\.]+\\:[0-9]{2,5}/[A-Za-z][A-Za-z0-9]+)/?;" +
            "RuntimePort=[0-9]{2,5};ManagementPort=[0-9]{2,5};" +
            "SharedAccessKeyName=(?<sharedAccessPolicyName>[\\w\\W]+);" +
            "SharedAccessKey=(?<sharedAccessPolicyValue>[\\w\\W]+)$";

        private bool DetectPrivateCloudConnectionString(string connectionString)
        {
            if (Regex.IsMatch(connectionString, OnPremPattern, RegexOptions.IgnoreCase))
                return true;
            else if (Regex.IsMatch(connectionString, Pattern, RegexOptions.IgnoreCase))
                return false;
            else {
                throw new ArgumentException($"Invalid Azure Service Bus connection string configured. " +
                                            $"Valid examples: {Environment.NewLine}" +
                                            $"public cloud: {Pattern} {Environment.NewLine}", 
                                            $"private cloud (SB 1.1): {OnPremPattern}");
            }
        }

        #endregion

        ICreateNamespaceManagers createNamespaceManagers;

        public CreatesMessagingFactories(ICreateNamespaceManagers createNamespaceManagers)
        {
            this.createNamespaceManagers = createNamespaceManagers;
        }

        public MessagingFactory Create(Address address)
        {
            var potentialConnectionString = address.Machine;
            var namespaceManager = createNamespaceManagers.Create(potentialConnectionString);

            // mszcool - Updated to detect if Service Bus 1.1 for Windows Server is used
            if (DetectPrivateCloudConnectionString(potentialConnectionString))
            {
                // mszcool - Need to use this approach because different ports for control and transport endpoints are used
                return MessagingFactory.CreateFromConnectionString(potentialConnectionString);
            }
            else {
                var settings = new MessagingFactorySettings
                {
                    TokenProvider = namespaceManager.Settings.TokenProvider,
                    NetMessagingTransportSettings =
                {
                    BatchFlushInterval = TimeSpan.FromSeconds(0.1)
                }
                };
                return MessagingFactory.Create(namespaceManager.Address, settings);
            }
        }
    }
}