using System.Collections.Generic;
using System.Linq;

namespace TelldusCoreWrapper.Entities
{
    public class Sensor : Device
    {
        public IEnumerable<SensorValueType> SupportedMethods { get; set; }
        public string Protocol { get; set; }
        public string Model { get; set; }
    }

    public abstract class Device
    {
        public int Id { get; set; }
    }

    public class ReceiverDevice : Device
    {
        public ReceiverDevice()
        {
            SupportedMethods = new List<Method>();
        }
        public string Name { get; set; }
        public int Index { get; set; }
        public IEnumerable<Method> SupportedMethods { get; set; }

        public bool IsMethodSupported(Method method)
        {
            return SupportedMethods.Contains(method);
        }
    }
}