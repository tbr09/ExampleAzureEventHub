using ExampleAzureEventHub.IoT.Core;
using System;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.IoT.StreamAnalytics
{
    class Program
    {
        private const string EventHubConnectionStringPublisher = "<EventHubConnectionString>";
        private const string EventHubName = "azureioteventhubsvc";

        private const string StorageAccountName = "eventhubiotex";
        private const string StorageAccountKey = "<StorageAccountKey>";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        static async Task Main(string[] args)
        {
            var publisher = new Publisher();
            publisher.Init(EventHubConnectionStringPublisher, EventHubName);

            var numEvents = 2000;
            var random = new Random(Environment.TickCount);

            for (int i = 0; i < numEvents; i++)
            {
                var randomDeviceTelemetry = DeviceTelemetry.GenerateRandom(random);
                await publisher.PublishAsync(randomDeviceTelemetry);
            }

            Console.ReadKey();
        }
    }
}
