using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;
using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.EventHubs;

namespace ExampleAzureEventHub.IoT.Receiver
{
    class Program
    {
        private const string EventHubConnectionStringConsumer = "<EventHubConnectionStringConsumer>";
        private const string EventHubName = "azureioteventhubsvc";

        private const string StorageContainerName = "iotevents";
        private const string StorageAccountName = "eventhubiotex";
        private const string StorageAccountKey = "<StorageAccountKey>";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static async Task Main(string[] args)
        {
            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionStringConsumer,
                StorageConnectionString,
                StorageContainerName);

            await eventProcessorHost.RegisterEventProcessorAsync<Consumer>();

            Console.ReadKey();
        }
    }
}
