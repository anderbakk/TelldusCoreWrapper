using System.Collections.Generic;
using System.Linq;

namespace TelldusCoreWrapper.Service
{
    public class Device
    {
        public Device()
        {
            SupportedMethods = new List<Method>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }

        public bool IsMethodSupported(int methodCode)
        {
            return SupportedMethods != null && SupportedMethods.Any(m => m.Code == methodCode);
        }

        public IEnumerable<Method> SupportedMethods { get; set; }
    }
}