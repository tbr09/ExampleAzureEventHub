using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.IoT.Sender
{
    class Program
    {
        private const string EventHubConnectionStringPublisher = "<EventHubConnectionString>";
        private const string EventHubConnectionStringConsumer = "<EventHubConnectionStringConsumer>";
        private const string EventHubName = "azureioteventhubsvc";

        private const string StorageContainerName = "iotevents";
        private const string StorageAccountName = "eventhubiotex";
        private const string StorageAccountKey = "<StorageAccountKey>";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static async Task Main(string[] args)
        {
            var publisher = new Publisher();

            publisher.Init(EventHubConnectionStringPublisher, EventHubName);

            var numEvents = 1000;
            var random = new Random(Environment.TickCount);

            var deviceTelemetry = new DeviceTelemetry
            {
                DeviceType = DeviceType.Phone,
                IpAddress = "127.0.0.1",
                IsOn = true,
                Time = DateTime.Now
            };

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionStringConsumer,
                StorageConnectionString,
                StorageContainerName);

            await eventProcessorHost.RegisterEventProcessorAsync<Consumer>();

            for (int i = 0; i < numEvents; i++)
            {
                var randomDeviceTelemetry = DeviceTelemetry.GenerateRandom(random);
                await publisher.PublishAsync(randomDeviceTelemetry);
            }

            Console.ReadKey();
        }
    }
}
