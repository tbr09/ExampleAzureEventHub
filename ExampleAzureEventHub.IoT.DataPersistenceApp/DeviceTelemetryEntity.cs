using ExampleAzureEventHub.IoT.Core;
using Microsoft.Azure.CosmosDB.Table;
using System;

namespace ExampleAzureEventHub.IoT.DataPersistenceApp
{
    public class DeviceTelemetryEntity : TableEntity
    {
        public DeviceTelemetryEntity(string ipAddress, DateTime time, DeviceType deviceType, bool isOn)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = deviceType.ToString();
            IpAddress = ipAddress;
            Time = time;
            DeviceType = (int)deviceType;
            IsOn = isOn;
        }

        public string IpAddress { get; set; }

        public DateTime Time { get; set; }

        public int DeviceType { get; set; }

        public bool IsOn { get; set; }


    }
}
