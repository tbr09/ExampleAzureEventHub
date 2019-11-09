using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.IoT.Example
{
    class Program
    {
        private const string EventHubConnectionString = "<event-hub-connection-string>";
        private const string EventHubName = "azureioteventhubsvc";

        private const string StorageContainerName = "iotevents";
        private const string StorageAccountName = "eventhubiotex";
        private const string StorageAccountKey = "<storage-account-key>";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static async Task Main(string[] args)
        {
            var publisher = new Publisher();

            publisher.Init(EventHubConnectionString, EventHubName);

            var strings = new string[] { "Hello", "world" };


            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            await eventProcessorHost.RegisterEventProcessorAsync<Consumer>();

            for (int i = 0; i < 100; i++)
            {
                await publisher.PublishAsync(strings);
            }

            Console.ReadKey();
        }
    }
}
