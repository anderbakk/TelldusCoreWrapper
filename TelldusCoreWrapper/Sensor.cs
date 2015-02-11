namespace TelldusCoreWrapper
{
    public struct Sensor
    {
        public Sensor(string protocol, string model, int dataType, int id) : this()
        {
            Protocol = protocol;
            Model = model;
            DataType = dataType;
            Id = id;
        }
        public string Protocol { get; private set; }
        public string Model { get; private set; }
        public int DataType { get; private set; }
        public int Id { get; private set; }
    }
}