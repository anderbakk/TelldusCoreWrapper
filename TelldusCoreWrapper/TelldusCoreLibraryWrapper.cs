using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TelldusCoreWrapper.Entities;

namespace TelldusCoreWrapper
{
    public class TelldusCoreLibraryWrapper : ITelldusCoreLibraryWrapper
    {
        [DllImport("TelldusCore.dll")]
        private static extern int tdInit();

        /// <summary>
        ///     This function initiates the library. Call this function before any other call to a function in telldus-core.
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            return tdInit();
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdClose();

        /// <summary>
        ///     Close the library and clean up the cache it uses. This should be called when the library is not supposed to be used
        ///     anymore.
        /// </summary>
        /// <returns></returns>
        public int Close()
        {
            return tdClose();
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdTurnOn(int deviceId);

        /// <summary>
        ///     Turns a device on. Make sure the device supports this by calling tdMethods() before any call to this function.
        /// </summary>
        /// <param name="deviceId">The device id to turn on</param>
        /// <returns>TELLSTICK_SUCCESS on success or appropriate error code on failure.</returns>
        public int TurnOn(int deviceId)
        {
            return tdTurnOn(deviceId);
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdTurnOff(int deviceId);

        /// <summary>
        /// Turns a device off. Make sure the device supports this by calling tdMethods() before any call to this function.
        /// </summary>
        /// <param name="deviceId">The device id to turn off.</param>
        /// <returns>TELLSTICK_SUCCESS on success or appropriate error code on failure.</returns>
        public int TurnOff(int deviceId)
        {
            return tdTurnOff(deviceId);
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdGetDeviceId(int deviceIndex);

        /// <summary>
        /// This function returns the unique id of a device with a specific index.
        /// </summary>
        /// <param name="deviceIndex">The device index to query. The index starts from 0.</param>
        /// <returns>The unique id for the device or -1 if the device is not found.</returns>
        public int GetDeviceId(int deviceIndex)
        {
            return tdGetDeviceId(deviceIndex);
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdMethods(int deviceIndex, int methodsSupported);

        /// <summary>
        /// Query a device for which methods it supports. By supplying the methods you support the library could remap the methods a device support for better fit the application.
        /// </summary>
        /// <param name="deviceIndex">The device id to query</param>
        /// <param name="methodsSupported">The methods the client application supports, OR'ed into a single integer</param>
        /// <returns>The method-flags OR'ed into an integer</returns>
        public IEnumerable<Method> Methods(int deviceIndex)
        {
            var allMethods = GetAllMethodsAsSingleInt();
            var methodsAsSingleInt = tdMethods(deviceIndex, allMethods);
            return
                    Enum.GetValues(typeof(Method))
                        .Cast<Method>()
                        .Where(method => (methodsAsSingleInt & (int)method) != 0)
                        .ToList();
        }

        private static int GetAllMethodsAsSingleInt()
        {
            var availableMethods = (from Method method in Enum.GetValues(typeof(Method)) select (int)method).ToList();

            return availableMethods.Aggregate(0, (current, availableMethod) => current | availableMethod);
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdGetNumberOfDevices();

        /// <summary>
        /// This function returns the number of devices configured.
        /// </summary>
        /// <returns>An integer of the total number of devices configured.</returns>
        public int GetNumberOfDevices()
        {
            return tdGetNumberOfDevices();
        }

        [DllImport("TelldusCore.dll")]
        private static extern IntPtr tdGetName(int deviceId);
        /// <summary>
        /// Query a device for it's name.
        /// </summary>
        /// <param name="deviceId">The unique id of the device to query</param>
        /// <returns>The name of the device or an empty string if the device is not found. The returned string must be freed by calling tdReleaseString()</returns>
        public string GetName(int deviceId)
        {
            var intPtrName = tdGetName(deviceId);
            var marshalledName = Marshal.PtrToStringAnsi(intPtrName);
            tdReleaseString(intPtrName);

            return marshalledName;
        }

        [DllImport("TelldusCore.dll")]
        private static extern void tdReleaseString(IntPtr thestring);

        [DllImport("TelldusCore.dll")]
        private static extern void tdDim(int deviceId, IntPtr level);
        /// <summary>
        /// Dims a device. Make sure the device supports this by calling tdMethods() before any call to this function
        /// </summary>
        /// <param name="deviceId">The device id to dim</param>
        /// <param name="level">The level the device should dim to. This value should be 0 - 255</param>
        public void Dim(int deviceId, int level)
        {
            var intPointer = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(intPointer, level);

            tdDim(deviceId, intPointer);

            Marshal.FreeHGlobal(intPointer);            
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdSensor(IntPtr protocol, int protocolLength, IntPtr model, int modelLength, IntPtr id, IntPtr dataTypes);

        /// <summary>
        /// Returns a list of sensors detected. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Sensor> GetSensors()
        {
            const int protocolstringsize = 20;
            const int modelstringsize = 30;
			
            var protocol = Marshal.AllocHGlobal(Marshal.SystemDefaultCharSize * protocolstringsize);
            var model = Marshal.AllocHGlobal(Marshal.SystemDefaultCharSize * modelstringsize);
            var id = Marshal.AllocHGlobal(sizeof(int));
            var dataType = Marshal.AllocHGlobal(sizeof(int));
            
            var resultCode = tdSensor(protocol, protocolstringsize, model, modelstringsize, id, dataType);
            while (resultCode == ResultCodes.TellstickSuccess)
            {
                var dataTypeInt = Marshal.ReadIntPtr(dataType).ToInt32();
                var supportedMethods =
                    Enum.GetValues(typeof (SensorValueType))
                        .Cast<SensorValueType>()
                        .Where(sensorValueType => (dataTypeInt & (int) sensorValueType) != 0)
                        .ToList();

                yield return new Sensor
                {
                    Id = Marshal.ReadIntPtr(id).ToInt32(),
                    Protocol = Marshal.PtrToStringAnsi(protocol),
                    Model = Marshal.PtrToStringAnsi(model),
                    SupportedMethods = supportedMethods,
                };
                
                resultCode = tdSensor(protocol, protocolstringsize, model, modelstringsize, id, dataType);
            }
            
            Marshal.FreeHGlobal(protocol);
            Marshal.FreeHGlobal(model);
            Marshal.FreeHGlobal(id);
            Marshal.FreeHGlobal(dataType);
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdSensorValue(IntPtr protocol, IntPtr model, int id, int dataType, IntPtr value, int valueLength, IntPtr timestamp);

        public IEnumerable<SensorValue> SensorValues(Sensor sensor)
        {
            const int valuestringsize = 20;
            var protocol = new IntPtr();
            var model = new IntPtr();
            var value = Marshal.AllocHGlobal(Marshal.SystemDefaultCharSize * valuestringsize);
            var timestampPtr = Marshal.AllocHGlobal(sizeof(int));

            var sensorValues = new List<SensorValue>();
            foreach (var supportedMethod in sensor.SupportedMethods)
            {
                protocol = Marshal.StringToHGlobalAnsi(sensor.Protocol);
                model = Marshal.StringToHGlobalAnsi(sensor.Model);
                tdSensorValue(protocol, model, sensor.Id,
                    (int)supportedMethod, value, valuestringsize, timestampPtr);

                var timestamp = IntToDateTime(Marshal.ReadIntPtr(timestampPtr).ToInt32());
                var valueString = Marshal.PtrToStringAnsi(value);
                sensorValues.Add(new SensorValue(supportedMethod, valueString, timestamp));
            }
            
            Marshal.FreeHGlobal(value);
            Marshal.FreeHGlobal(timestampPtr);
            Marshal.FreeHGlobal(protocol);
            Marshal.FreeHGlobal(model);

            return sensorValues;
        }

        private static DateTime IntToDateTime(int timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime;
        }
		
    }

    
}