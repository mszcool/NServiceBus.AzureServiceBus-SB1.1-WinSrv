using System;
using NServiceBus;

class Program
{

    static void Main()
    {
        string connStr = System.Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");

        Console.Title = "Samples.MessageDurability.Sender";
        #region non-transactional
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.Transactions()
            .Disable();
        #endregion
        busConfiguration.EndpointName("Samples.MessageDurability.Sender");
        busConfiguration.ScaleOut().UseSingleBrokerQueue();
        busConfiguration.UseTransport<AzureServiceBusTransport>()
            .ConnectionString(connStr);
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.EnableInstallers();
        busConfiguration.UsePersistence<InMemoryPersistence>();

        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            bus.Send("Samples.MessageDurability.Receiver", new MyMessage());
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
