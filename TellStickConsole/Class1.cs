using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelldusCoreWrapper;

namespace TellStickConsole
{
    public class Class1
    {
        public static void Main(string[] args)
        {
            using (var service = new TelldusCoreService(new TelldusCoreLibraryWrapper()))
            {
                var devices = service.GetDevices();
                foreach (var device in devices)
                {
                    Console.WriteLine("{0} - {1} - {2}", device.Id, device.Name, device.SupportedMethod);
                }
          
            }
            
            Console.ReadLine();
        }
    }
}
