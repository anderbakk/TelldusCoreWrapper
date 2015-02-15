using System;

namespace TelldusCoreWrapper.Entities
{
    public class SensorValue
    {
        public SensorValueType SensorValueType { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}