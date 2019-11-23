using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleAzureEventHub.IoT.RealTimeAnalyzer
{
    public class RealTimeConsumer : IEventProcessor
    {
        private readonly List<DeviceTelemetry> _cache = new List<DeviceTelemetry>();
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
                _cache.Add(deviceTelemetry);

                var totalNumDevices = _cache.Count;
                var numDevicesSwitchedOn = _cache.Count(dt => dt.IsOn);

                Console.WriteLine($"{numDevicesSwitchedOn} of {totalNumDevices} devices are currently switched on.");
            }
            if (_checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                _checkpointStopWatch.Restart();
            }
        }
    }
}
