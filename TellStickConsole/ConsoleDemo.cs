using System;
using System.Linq;
using System.Threading;
using TelldusCoreWrapper;
using TelldusCoreWrapper.Entities;
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
                        Console.WriteLine("\t{0}", method);
                    }
                    if (device.IsMethodSupported(Method.TurnOff))
                        service.TurnOff(device.Id);
                    Thread.Sleep(1000);
                    if (device.IsMethodSupported(Method.TurnOn))
                        service.TurnOn(device.Id);
                    Thread.Sleep(1000);

                    if (device.IsMethodSupported(Method.Dim))
                        service.Dim(device.Id, 50);

                }

                var sensors = service.GetSensors().ToList();
                Console.WriteLine("Found {0} sensors", sensors.Count());

                foreach (var sensor in sensors)
                {
                    Console.WriteLine("Id {0}, Model {1}, Protocol {2}", sensor.Id, sensor.Model, sensor.Protocol);
                    
                    var values = service.GetValuesFromSensor(sensor);
                    foreach (var sensorValue in values)
                    {
                        Console.WriteLine("Value type {0}, Value {1}, Timestamp {2}", sensorValue.SensorValueType,
                            sensorValue.Value, sensorValue.Timestamp);
                    }
                }
                
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
