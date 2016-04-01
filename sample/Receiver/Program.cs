using System;
using NServiceBus;

class Program
{

    static void Main()
    {
        string connStr = System.Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");

        Console.Title = "Samples.MessageDurability.Receiver";
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.Transactions()
            .Disable();
        busConfiguration.EndpointName("Samples.MessageDurability.Receiver");
        busConfiguration.ScaleOut().UseSingleBrokerQueue();
        busConfiguration.UseTransport<AzureServiceBusTransport>()
            .ConnectionString(connStr);
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.EnableInstallers();
        busConfiguration.UsePersistence<InMemoryPersistence>();

        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
