using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public int Methods(int deviceIndex, int methodsSupported)
        {
            return tdMethods(deviceIndex, methodsSupported);
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
        /// <summary>
        /// This function removes a controller from the list of controllers. The controller must not be available (disconnected) for this to work.
        /// </summary>
        /// <param name="theString">A string returned from a td* function</param>
        public void ReleaseString(IntPtr theString)
        {
            tdReleaseString(theString);
        }

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
        public IEnumerable<Sensor> Sensor()
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
                yield return new Sensor(Marshal.PtrToStringAnsi(protocol),
                    Marshal.PtrToStringAnsi(model),
                    Marshal.ReadIntPtr(dataType).ToInt32(),
                    Marshal.ReadIntPtr(id).ToInt32());
                
                resultCode = tdSensor(protocol, protocolstringsize, model, modelstringsize, id, dataType);
            }
            
            Marshal.FreeHGlobal(protocol);
            Marshal.FreeHGlobal(model);
            Marshal.FreeHGlobal(id);
            Marshal.FreeHGlobal(dataType);
        }

        [DllImport("TelldusCore.dll")]
        private static extern int tdSensorValue(IntPtr protocol, IntPtr model, int id, int dataType, IntPtr value, int valueLength, IntPtr timestamp);

        public IEnumerable<SensorReading> SensorValues(IEnumerable<Sensor> sensors)
        {
            const int valuestringsize = 20;
            var protocol = new IntPtr();
            var model = new IntPtr();
            var value = Marshal.AllocHGlobal(Marshal.SystemDefaultCharSize * valuestringsize);
            var timestamp = Marshal.AllocHGlobal(sizeof(int));

            foreach (var sensor in sensors)
            {   
                if ((sensor.DataType & 1) != 0)
                {
                    protocol = Marshal.StringToHGlobalAnsi(sensor.Protocol);
                    model = Marshal.StringToHGlobalAnsi(sensor.Model);
                    tdSensorValue(protocol, model, sensor.Id,
                        1, value, valuestringsize, timestamp);
                    var valueString = Marshal.PtrToStringAnsi(value);

                    yield return new SensorReading(sensor, valueString);
                }
                if ((sensor.DataType & 2) != 0)
                {
                    protocol = Marshal.StringToHGlobalAnsi(sensor.Protocol);
                    model = Marshal.StringToHGlobalAnsi(sensor.Model);
                    tdSensorValue(protocol, model, sensor.Id,
                        2, value, valuestringsize, timestamp);
                    var valueString = Marshal.PtrToStringAnsi(value);

                    yield return new SensorReading(sensor, valueString);
                }
            }

            Marshal.FreeHGlobal(value);
            Marshal.FreeHGlobal(timestamp);
            Marshal.FreeHGlobal(protocol);
            Marshal.FreeHGlobal(model);
            
        }
		
    }

    
}