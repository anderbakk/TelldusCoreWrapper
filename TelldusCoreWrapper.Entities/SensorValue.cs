using System;

namespace TelldusCoreWrapper.Entities
{
    public class SensorValue
    {
        public SensorValue(SensorValueType sensorValueType, string value, DateTime timestamp)
        {
            SensorValueType = sensorValueType;
            Value = value;
            Timestamp = timestamp;
        }

        public SensorValueType SensorValueType { get; private set; }
        public string Value { get; private set; }
        public DateTime Timestamp { get;private  set; }
    }
}