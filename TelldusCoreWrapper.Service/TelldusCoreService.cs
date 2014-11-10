using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TelldusCoreWrapper
{
    public enum TelldusCoreResultCodes
    {
        Success = 0,
        ErrorConnectingService = -6
    }

    public enum Methods
    {
        TurnOn = 1,
        TurnOff = 2,
        Dim = 16
    }

    public class TelldusCoreService : IDisposable
    {
        private readonly ITelldusCoreLibraryWrapper _telldusCoreLibraryWrapper;

        public TelldusCoreService(ITelldusCoreLibraryWrapper wrapper)
        {
            _telldusCoreLibraryWrapper = wrapper;
            _telldusCoreLibraryWrapper.Init();
        }

        public int GetDeviceIdFrom(int deviceIndex)
        {
            return _telldusCoreLibraryWrapper.GetDeviceId(deviceIndex);
        }

        public void TurnOn(int deviceId)
        {
            var resultCode = _telldusCoreLibraryWrapper.TurnOn(deviceId);
            VerifyResultCode(resultCode);
        }

        public void TurnOff(int deviceId)
        {
            var resultCode = _telldusCoreLibraryWrapper.TurnOff(deviceId);
            VerifyResultCode(resultCode);
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

                yield return new Device{Id = deviceId, Name = name, Index = index, SupportedMethod = supportedMethods};
            }
        }

        private int GetSupportedMethods(int deviceId)
        {
            var result = _telldusCoreLibraryWrapper.Methods(deviceId, GetMethods());
            return result;
        }

        private int GetMethods()
        {
            return (int) Methods.Dim;
        }

        private static void VerifyResultCode(int resultCode)
        {
            if(resultCode != (int) TelldusCoreResultCodes.Success)
                throw new Exception("Operation failed :" + resultCode);
        }

        public string GetName(int deviceId)
        {
            var name = _telldusCoreLibraryWrapper.GetName(deviceId);
            return name;
        }

        public void Dispose()
        {
            _telldusCoreLibraryWrapper.Close();
        }
    }
}