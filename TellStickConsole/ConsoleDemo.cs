using System;
using System.Linq;
using System.Threading;
using TelldusCoreWrapper;
using TelldusCoreWrapper.Service;

namespace TellStickConsole
{
    public class ConsoleDemo
    {
        public static void Main(string[] args)
        {
            using (var service = new TelldusCoreService(new TelldusCoreLibraryWrapper()))
            {
                var devices = service.GetDevices();
                foreach (var device in devices)
                {
                    Console.WriteLine("{0} - {1}", device.Id, device.Name);
                    foreach (var method in device.SupportedMethods)
                    {
                        Console.WriteLine("\t{0}-{1}", method.Code, method.Description);
                    }
                    if (device.IsMethodSupported(2))
                        service.TurnOff(device.Id);
                    Thread.Sleep(1000);
                    if (device.IsMethodSupported(2))
                        service.TurnOn(device.Id);
                    Thread.Sleep(1000);

                    if (device.IsMethodSupported(16))
                        service.Dim(device.Id, 50);

                }

                var sensors = service.GetAllSensors().ToList();
                Console.WriteLine("Found {0} sensors", sensors.Count());
                var sensorValues = service.GetSensorReadings(sensors);
                foreach (var sensor in sensorValues)
                {
                    Console.WriteLine("Id : {0}, Model:{1}, Protocol:{2}, Value:{3}", 
                        sensor.Sensor.Id, sensor.Sensor.Model, 
                        sensor.Sensor.Protocol, sensor.SensorValue);
                }
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
