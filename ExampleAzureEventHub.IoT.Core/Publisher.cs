using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.IoT.Core
{
    public class Publisher
    {
        private EventHubClient eventHubClient;

        public void Init(string connectionString, string eventHubName)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = eventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
        }

        public async Task PublishAsync<T>(T myEvent)
        {
            var serializedEvent = JsonConvert.SerializeObject(myEvent);

            var eventBytes = Encoding.UTF8.GetBytes(serializedEvent);

            var eventData = new EventData(eventBytes);

            await eventHubClient.SendAsync(eventData);
        }
    }
}
