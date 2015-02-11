using System.Collections.Generic;
using System.Linq;

namespace TelldusCoreWrapper.Service
{
    public abstract class Device
    {
        protected Device()
        {
            SupportedMethods = new List<Method>();
        }
        public int Id { get; set; }
        public bool IsMethodSupported(int methodCode)
        {
            return SupportedMethods != null && SupportedMethods.Any(m => m.Code == methodCode);
        }

        public IEnumerable<Method> SupportedMethods { get; set; }
    }
}