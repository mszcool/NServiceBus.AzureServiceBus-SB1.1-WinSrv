namespace NServiceBus.AzureServiceBus.Topology.MetaModel
{
    using System;
    using System.Text.RegularExpressions;

    public class ConnectionString : IEquatable<ConnectionString>
    {
        public static readonly string Sample = "Endpoint=sb://[namespace name].servicebus.windows.net;SharedAccessKeyName=[shared access key name];SharedAccessKey=[shared access key]";
        private static readonly string Pattern = "^Endpoint=sb://(?<namespaceName>[A-Za-z][A-Za-z0-9-]{4,48}[A-Za-z0-9]).servicebus.windows.net/?;SharedAccessKeyName=(?<sharedAccessPolicyName>[\\w\\W]+);SharedAccessKey=(?<sharedAccessPolicyValue>[\\w\\W]+)$";

        // mszcool - added to support on-premises Service Bus 1.1 connection string values
        public static readonly string OnPremSample = "Endpoint=sb://[namespace name];StsEndpoint=https://[sts endpoint address]:[port];RuntimePort=[port];ManagementPort=[port];SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[shared access key]";
        private static readonly string OnPremPattern =
            "^Endpoint=sb\\://(?<serverName>[A-Za-z][A-Za-z0-9\\-\\.]+)/(?<namespaceName>[A-Za-z][A-Za-z0-9]{4,48}[A-Za-z0-9])/?;" +
            "StsEndPoint=(?<stsEndpoint>https\\://[A-Za-z][A-Za-z0-9\\-\\.]+\\:[0-9]{2,5}/[A-Za-z][A-Za-z0-9]+)/?;" +
            "RuntimePort=[0-9]{2,5};ManagementPort=[0-9]{2,5};" +
            "SharedAccessKeyName=(?<sharedAccessPolicyName>[\\w\\W]+);" +
            "SharedAccessKey=(?<sharedAccessPolicyValue>[\\w\\W]+)$";

        private readonly string _value;

        // mszcool - Added to identify when running against private cloud
        public bool IsPrivateCloud { get; }

        public string NamespaceName { get; }
        public string SharedAccessPolicyName { get; }
        public string SharedAccessPolicyValue { get; }

        public ConnectionString(string value)
        {
            // mszcool - added to support on-premises Service Bus 1.1 connection string values
            IsPrivateCloud = false;
            if (!Regex.IsMatch(value, Pattern, RegexOptions.IgnoreCase))
            {
                if (!Regex.IsMatch(value, OnPremPattern, RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException($"Provided value isn't a valid connection string. {Environment.NewLine}" +
                             $"The namespace name can contain only letters, numbers, and hyphens.The namespace must start with a letter, and it must end with a letter or number. {Environment.NewLine}" +
                             $"f.e. public cloud: {ConnectionString.Sample} {Environment.NewLine}" +
                             $"f.e. private cloud: {ConnectionString.OnPremSample}", nameof(value));

                }
                IsPrivateCloud = true;
            }
 
            _value = value;

            NamespaceName = Regex.Match(value, Pattern).Groups["namespaceName"].Value;
            SharedAccessPolicyName = Regex.Match(value, Pattern).Groups["sharedAccessPolicyName"].Value;
            SharedAccessPolicyValue = Regex.Match(value, Pattern).Groups["sharedAccessPolicyValue"].Value;
        }

        public bool Equals(ConnectionString other)
        {
            return other != null && (
                string.Equals(NamespaceName, other.NamespaceName, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(SharedAccessPolicyName, other.SharedAccessPolicyName, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(SharedAccessPolicyValue, other.SharedAccessPolicyValue));
        }

        public override bool Equals(object obj)
        {
            var target = obj as ConnectionString;
            return Equals(target);
        }

        public override int GetHashCode()
        {
            var namespaceName = NamespaceName.ToLower();
            var sharedAccessPolicyName = SharedAccessPolicyName.ToLower();

            return string.Concat(namespaceName, sharedAccessPolicyName, SharedAccessPolicyValue).GetHashCode();
        }

        public override string ToString()
        {
            return _value;
        }

        public static bool TryParse(string value, out ConnectionString connectionString)
        {
            try
            {
                connectionString = new ConnectionString(value);
                return true;
            }
            catch (ArgumentException)
            {
                connectionString = null;
                return false;
            }
        }

        public static implicit operator string(ConnectionString connectionString)
        {
            return connectionString.ToString();
        }

        public static bool operator ==(ConnectionString connectionString1, ConnectionString connectionString2)
        {
            if (ReferenceEquals(connectionString1, null) && ReferenceEquals(connectionString2, null)) return true;
            if (ReferenceEquals(connectionString1, null) || ReferenceEquals(connectionString2, null)) return false;

            return connectionString1.Equals(connectionString2);
        }

        public static bool operator !=(ConnectionString connectionString1, ConnectionString connectionString2)
        {
            return !(connectionString1 == connectionString2);
        }
    }
}
