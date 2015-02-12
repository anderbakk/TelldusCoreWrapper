using System;
using System.Collections.Generic;
using TelldusCoreWrapper.Entities;

namespace TelldusCoreWrapper.Service
{
    public class TelldusCoreService : IDisposable
    {
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

        public IEnumerable<Sensor> GetSensors()
        {
            return _telldusCoreLibraryWrapper.GetSensors();
        }

        public IEnumerable<ReceiverDevice> GetDevices()
        {
            var numberOfDevices = GetNumberOfDevices();
            for (var index = 0; index < numberOfDevices; index++)
            {
                var deviceId = GetDeviceIdFrom(index);
                var name = GetName(deviceId);
                var supportedMethods = _telldusCoreLibraryWrapper.Methods(deviceId);

                yield return new ReceiverDevice
                {
                    Id = deviceId,
                    Name = name,
                    Index = index,
                    SupportedMethods = supportedMethods
                };
            }
        }



        public IEnumerable<SensorValue> GetValuesFromSensor(Sensor sensor)
        {
            return _telldusCoreLibraryWrapper.SensorValues(sensor);
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