using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.IoT.DataPersistenceApp
{
    class Program
    {
        private const string EventHubConnectionString = "<EventHubConnectionString>";
        private const string EventHubConnectionStringConsumer = "<EventHubConnectionStringConsumer>";
        private const string EventHubName = "azureioteventhubsvc";

        private const string StorageContainerName = "iotevents";
        private const string StorageAccountName = "eventhubiotex";
        private const string StorageAccountKey = "<StorageAccountKey>";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static async Task Main(string[] args)
        {
            var publisher = new Publisher();

            publisher.Init(EventHubConnectionString, EventHubName);

            var numEvents = 100;
            var random = new Random(Environment.TickCount);

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                "dataPersistence",
                EventHubConnectionStringConsumer,
                StorageConnectionString,
                StorageContainerName);

            await eventProcessorHost.RegisterEventProcessorAsync<DataPersistenceConsumer>();

            for (int i = 0; i < numEvents; i++)
            {
                var randomDeviceTelemetry = DeviceTelemetry.GenerateRandom(random);
                await publisher.PublishAsync(randomDeviceTelemetry);
            }

            Console.ReadKey();
        }
    }
}
