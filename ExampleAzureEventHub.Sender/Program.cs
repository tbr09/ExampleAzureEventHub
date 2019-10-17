﻿using Microsoft.Azure.EventHubs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.Sender
{
    class Program
    {
        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "Endpoint=sb://azureeventhubnspace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=sXJUmoGj6kb99ndhtg2VljUxlwbVaUktfPbx2tiSD0s=";
        private const string EventHubName = "eventhubaz203";
        private static bool SetRandomPartitionKey = false;

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from a the connection string, and sets the EntityPath.
            // Typically the connection string should have the Entity Path in it, but for the sake of this simple scenario
            // we are using the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
            {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        // Creates an Event Hub client and sends 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            var rnd = new Random();

            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {
                    var message = $"Message {i}";

                    // Set random partition key?
                    if (SetRandomPartitionKey)
                    {
                        var pKey = Guid.NewGuid().ToString();
                        await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)), pKey);
                        Console.WriteLine($"Sent message: '{message}' Partition Key: '{pKey}'");
                    }
                    else
                    {
                        await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                        Console.WriteLine($"Sent message: '{message}'");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }
    }
}
