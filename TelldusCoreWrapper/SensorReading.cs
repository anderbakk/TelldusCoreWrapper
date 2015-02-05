namespace TelldusCoreWrapper
{
    public struct SensorReading
    {
        public SensorReading(string protocolString, string model, string sensorValue, int dataType):this()
        {
            Protocol = protocolString;
            Model = model;
            SensorValue = sensorValue;
            DataType = dataType;
        }

        public int DataType { get; private set; }

        public string SensorValue { get; private set; }

        public string Model { get; private set; }

        public string Protocol { get; private set; }
    }
}
