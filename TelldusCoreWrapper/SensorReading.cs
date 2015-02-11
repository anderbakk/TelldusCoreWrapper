namespace TelldusCoreWrapper
{
    public struct SensorReading
    {
        public SensorReading(Sensor sensor, string sensorValue):this()
        {
            Sensor = sensor;
            SensorValue = sensorValue;
        }

        public Sensor Sensor{ get; private set; }
        public string SensorValue { get; private set; }
    }
}
