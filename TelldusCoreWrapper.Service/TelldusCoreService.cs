using System;
using System.Collections.Generic;
using System.Linq;

namespace TelldusCoreWrapper.Service
{
    public class TelldusCoreService : IDisposable
    {
        private readonly Dictionary<int, string> _availableReceiverMethods = new Dictionary<int, string>
        {
            {1, "Turn on"},
            {2, "Turn off"},
            {4, "Bell"},
            {8, "Toggle"},
            {16, "Dim"},
            {32, "Self learn"},
            {64, "Execute"},
            {128, "Up"},
            {256, "Down"},
            {512, "Stop"}
        };

        private readonly Dictionary<int, string> _availableSensorMethods = new Dictionary<int, string>
        {
            {1, "Temperature"},
            {2, "Humidity"},
        }; 

        private readonly ITelldusCoreLibraryWrapper _telldusCoreLibraryWrapper;

        public TelldusCoreService(ITelldusCoreLibraryWrapper wrapper)
        {
            _telldusCoreLibraryWrapper = wrapper;
            _telldusCoreLibraryWrapper.Init();
        }

        public void Dispose()
        {
            _telldusCoreLibraryWrapper.Close();
        }

        public int GetDeviceIdFrom(int deviceIndex)
        {
            return _telldusCoreLibraryWrapper.GetDeviceId(deviceIndex);
        }

        public void TurnOn(int deviceId)
        {
            _telldusCoreLibraryWrapper.TurnOn(deviceId);
            VerifyResultCode();
        }

        public void TurnOff(int deviceId)
        {
            _telldusCoreLibraryWrapper.TurnOff(deviceId);
            VerifyResultCode();
        }

        public void Dim(int deviceId, int level)
        {
            if (level < 0 || level > 255)
                throw new ArgumentOutOfRangeException("level");
            _telldusCoreLibraryWrapper.Dim(deviceId, level);
        }

        public int GetNumberOfDevices()
        {
            return _telldusCoreLibraryWrapper.GetNumberOfDevices();
        }

        public IEnumerable<SensorDevice> GetSensors()
        {
            var sensors = _telldusCoreLibraryWrapper.Sensor();
            return from sensor in sensors let availableMethods = (from availableSensorMethod in _availableSensorMethods
                where (sensor.DataType & availableSensorMethod.Key) != 0
                select new Method
                {
                    Code = availableSensorMethod.Key, Description = availableSensorMethod.Value
                }).ToList() select new SensorDevice
                {
                    Id = sensor.Id,
                    Model = sensor.Model,
                    Protocol = sensor.Protocol,
                    SupportedMethods = availableMethods
                   
                };
        }

        public IEnumerable<ReceiverDevice> GetDevices()
        {
            var numberOfDevices = GetNumberOfDevices();
            for (var index = 0; index < numberOfDevices; index++)
            {
                var deviceId = GetDeviceIdFrom(index);
                var name = GetName(deviceId);
                var supportedMethods = GetSupportedMethodsForReceiver(deviceId);

                yield return new ReceiverDevice
                {
                    Id = deviceId,
                    Name = name,
                    Index = index,
                    SupportedMethods = supportedMethods
                };
            }
        }

        private IEnumerable<Method> GetSupportedMethodsForReceiver(int deviceId)
        {
            var telldusResult = _telldusCoreLibraryWrapper.Methods(deviceId, GetAllMethodsAsSingleInt());

            return (from availableMethod in _availableReceiverMethods
                let orResult = availableMethod.Key | telldusResult
                where orResult == telldusResult
                select new Method {Code = availableMethod.Key, Description = availableMethod.Value}).ToList();
        }

        public int GetAllMethodsAsSingleInt()
        {
            return _availableReceiverMethods.Aggregate(0, (current, availableMethod) => current | availableMethod.Key);
        }

        public IEnumerable<Sensor> GetAllSensors()
        {
            return _telldusCoreLibraryWrapper.Sensor();
        }

        public IEnumerable<SensorReading> GetSensorReadings(IEnumerable<Sensor> sensors)
        {
            return _telldusCoreLibraryWrapper.SensorValues(sensors);
        }
        private static void VerifyResultCode()
        {
            //TODO Check if result code is SUCCESS or error
        }

        public string GetName(int deviceId)
        {
            var name = _telldusCoreLibraryWrapper.GetName(deviceId);
            return name;
        }
    }
}