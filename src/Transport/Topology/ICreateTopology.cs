namespace NServiceBus.AzureServiceBus
{
    using System.Threading.Tasks;

    // responsible for creating (part of) the topology
    // part of = only the part that matters to this endpoint

    // note there is some creation logic elsewhere already, those calls should be removed and centralized here

    public interface ICreateTopology
    {
        Task Create(TopologySection topology);
    }
}