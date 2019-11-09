﻿using System;

namespace ExampleAzureEventHub.IoT.Core
{
    [Flags]
    public enum DeviceType
    {
        Unknown = 0,
        PersonalComputer = 1,
        Laptop = 2,
        Phone = 4,
        Tablet = 8
    }
}
