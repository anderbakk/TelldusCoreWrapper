using System;
using System.Collections.Generic;

namespace TelldusCoreWrapper
{
    public interface ITelldusCoreLibraryWrapper
    {
        /// <summary>
        ///     This function initiates the library. Call this function before any other call to a function in telldus-core.
        /// </summary>
        /// <returns></returns>
        int Init();

        /// <summary>
        ///     Close the library and clean up the cache it uses. This should be called when the library is not supposed to be used
        ///     anymore.
        /// </summary>
        /// <returns></returns>
        int Close();

        /// <summary>
        ///     Turns a device on. Make sure the device supports this by calling tdMethods() before any call to this function.
        /// </summary>
        /// <param name="deviceId">The device id to turn on</param>
        /// <returns>TELLSTICK_SUCCESS on success or appropriate error code on failure.</returns>
        int TurnOn(int deviceId);

        /// <summary>
        /// Turns a device off. Make sure the device supports this by calling tdMethods() before any call to this function.
        /// </summary>
        /// <param name="deviceId">The device id to turn off.</param>
        /// <returns>TELLSTICK_SUCCESS on success or appropriate error code on failure.</returns>
        int TurnOff(int deviceId);

        /// <summary>
        /// This function returns the unique id of a device with a specific index.
        /// </summary>
        /// <param name="deviceIndex">The device index to query. The index starts from 0.</param>
        /// <returns>The unique id for the device or -1 if the device is not found.</returns>
        int GetDeviceId(int deviceIndex);

        /// <summary>
        /// Query a device for which methods it supports. By supplying the methods you support the library could remap the methods a device support for better fit the application.
        /// </summary>
        /// <param name="deviceIndex">The device id to query</param>
        /// <param name="methodsSupported">The methods the client application supports, OR'ed into a single integer</param>
        /// <returns>The method-flags OR'ed into an integer</returns>
        int Methods(int deviceIndex, int methodsSupported);

        /// <summary>
        /// This function returns the number of devices configured.
        /// </summary>
        /// <returns>An integer of the total number of devices configured.</returns>
        int GetNumberOfDevices();

        /// <summary>
        /// Query a device for it's name.
        /// </summary>
        /// <param name="deviceId">The unique id of the device to query</param>
        /// <returns>The name of the device or an empty string if the device is not found. The returned string must be freed by calling tdReleaseString()</returns>
        string GetName(int deviceId);

        /// <summary>
        /// This function removes a controller from the list of controllers. The controller must not be available (disconnected) for this to work.
        /// </summary>
        /// <param name="theString">A string returned from a td* function</param>
        void ReleaseString(IntPtr theString);

        /// <summary>
        /// Dims a device. Make sure the device supports this by calling tdMethods() before any call to this function
        /// </summary>
        /// <param name="deviceId">The device id to dim</param>
        /// <param name="level">The level the device should dim to. This value should be 0 - 255</param>
        void Dim(int deviceId, int level);

        IEnumerable<SensorReading> Sensor();
    }
}