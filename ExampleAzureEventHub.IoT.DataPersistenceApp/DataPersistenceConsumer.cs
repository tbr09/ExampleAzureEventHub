using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ExampleAzureEventHub.IoT.DataPersistenceApp
{
    public class DataPersistenceConsumer : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            if (reason == CloseReason.Shutdown)
                await context.CheckpointAsync();
            Console.ReadKey();
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            _checkpointStopWatch = new Stopwatch();
            _checkpointStopWatch.Start();
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var deviceTelemetry = JsonConvert.DeserializeObject<DeviceTelemetry>(data);

                string sqlDatabaseConnectionString = "<sqlDatabaseConnectionString>";

                SaveTelemetryToSqlDatabase(deviceTelemetry, sqlDatabaseConnectionString);

                //await SaveTelemetryToTableStorage(deviceTelemetry);
            }
            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }

        private static void SaveTelemetryToSqlDatabase(DeviceTelemetry deviceTelemetry, string sqlDatabaseConnectionString)
        {
            using (var connection = new SqlConnection(sqlDatabaseConnectionString))
            {
                connection.Open();

                const string sqlCommandText =
                    "insert into DeviceTelemetry(IPAdresses,Time,DeviceType,IsOn) values (@IPAdresses,@Time,@DeviceType,@IsOn);";
                using (var command = new SqlCommand(sqlCommandText, connection))
                {
                    command.Parameters.AddWithValue("@IPAdresses", deviceTelemetry.IpAddress);
                    command.Parameters.AddWithValue("@Time", deviceTelemetry.Time);
                    command.Parameters.AddWithValue("@DeviceType", deviceTelemetry.DeviceType);
                    command.Parameters.AddWithValue("@IsOn", deviceTelemetry.IsOn);

                    Console.WriteLine($"Added Device IP {deviceTelemetry.IpAddress} to the database.");
                    command.ExecuteNonQuery();
                }
            }
        }

        private static async Task SaveTelemetryToTableStorage(DeviceTelemetry deviceTelemetry)
        {
            string storageConnectionString = "<storageConnectionString>";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudTableClient cloudTableClient = storageAccount.CreateCloudTableClient();

            var table = cloudTableClient.GetTableReference("deviceTelemetry");

            await table.CreateIfNotExistsAsync();

            var deviceTelemetryEntity = new DeviceTelemetryEntity(deviceTelemetry.IpAddress, deviceTelemetry.Time, deviceTelemetry.DeviceType, deviceTelemetry.IsOn);

            TableOperation insertOperation = TableOperation.Insert(deviceTelemetryEntity);

            await table.ExecuteAsync(insertOperation);
        }
    }
}
