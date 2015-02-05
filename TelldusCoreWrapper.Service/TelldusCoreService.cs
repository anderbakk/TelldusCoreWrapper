using System;
using System.Collections.Generic;
using System.Linq;

namespace TelldusCoreWrapper.Service
{
    public class TelldusCoreService : IDisposable
    {
        private readonly Dictionary<int, string> _availableMethods = new Dictionary<int, string>
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

        public IEnumerable<Device> GetDevices()
        {
            var numberOfDevices = GetNumberOfDevices();
            for (var index = 0; index < numberOfDevices; index++)
            {
                var deviceId = GetDeviceIdFrom(index);
                var name = GetName(deviceId);

                var supportedMethods = GetSupportedMethods(deviceId);

                yield return new Device
                {
                    Id = deviceId,
                    Name = name,
                    Index = index,
                    SupportedMethods = supportedMethods
                };
            }
        }

        private IEnumerable<Method> GetSupportedMethods(int deviceId)
        {
            var telldusResult = _telldusCoreLibraryWrapper.Methods(deviceId, GetAllMethodsAsSingleInt());

            return (from availableMethod in _availableMethods
                let orResult = availableMethod.Key | telldusResult
                where orResult == telldusResult
                select new Method {Code = availableMethod.Key, Description = availableMethod.Value}).ToList();
        }

        public int GetAllMethodsAsSingleInt()
        {
            return _availableMethods.Aggregate(0, (current, availableMethod) => current | availableMethod.Key);
        }

        public List<string> GetSensorValues()
        {
            var sensorReadings = _telldusCoreLibraryWrapper.Sensor();
            //Just for demo purpose
            return sensorReadings.Select(s => s.Model + " " + s.SensorValue).ToList();
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