using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.Receiver
{
    class Program
    {
        private const string EventHubConnectionString = "Endpoint=sb://azureeventhubnspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=sXJUmoGj6kb99ndhtg2VljUxlwbVaUktfPbx2tiSD0s=";
        private const string EventHubName = "eventhubaz203";
        private const string StorageContainerName = "messages";
        private const string StorageAccountName = "azureeventhubblob203";
        private const string StorageAccountKey = "6U2byBqKXmJdEZfD5GRmh1UqPwR1n00DWwfaal2dLM9HOLEP14aGf3SocibJum7qQt8y0I6iC02rTZf+cDqNiw==";

        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventProcessorHost = new EventProcessorHost(
                EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                EventHubConnectionString,
                StorageConnectionString,
                StorageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
