// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;
using System.Linq;
using HidSharp;
using WwDevicesDotNet.Winctrl.Mcdu;
using WwDevicesDotNet.Winctrl.Pfp3N;
using WwDevicesDotNet.Winctrl.Pfp7;
using WwDevicesDotNet.Winctrl.Pfp4;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Finds USB devices and creates instances of <see cref="ICdu"/> implementations
    /// for them.
    /// </summary>
    public static class CduFactory
    {
        //          ARE YOU HERE LOOKING FOR THE LIST OF ALL KNOWN DEVICE IDENTIFIERS?
        //                       They've moved to SupportedDevices.cs

        /// <summary>
        /// Returns a device identifier corresponding to the vendor and product IDs passed
        /// across, or null if the vendor and product ID do not correspond with a CDU that
        /// the library can interact with.
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static DeviceIdentifier GetDeviceIdentifierForUsbIdentifiers(
            int vendorId,
            int productId
        )
        {
            return SupportedDevices
                .AllSupportedDevices
                .FirstOrDefault(deviceIdentifier =>
                       deviceIdentifier.UsbVendorId == vendorId
                    && deviceIdentifier.UsbProductId == productId
                );
        }

        /// <summary>
        /// Returns a collection of all MCDU devices that can be found on the local machine.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<DeviceIdentifier> FindLocalDevices()
        {
            var result = new List<DeviceIdentifier>();

            var local = DeviceList.Local;
            foreach(var hidDevice in local.GetHidDevices()) {
                var deviceIdentifier = GetDeviceIdentifierForUsbIdentifiers(
                    hidDevice.VendorID,
                    hidDevice.ProductID
                );
                if(deviceIdentifier != null) {
                    result.Add(deviceIdentifier);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an initialised connection to a CDU device. If the device ID is not
        /// specified then the first CDU found on the system is used. If the requested CDU
        /// cannot be found (or there are no CDUs to default to) then null is returned.
        /// </summary>
        /// <param name="deviceId">
        /// The specific device to connect to, if null then the first device found is
        /// used. Defaults to null.
        /// </param>
        /// <param name="device">
        /// If not null then only devices for this product are considered if <paramref
        /// name="deviceId"/> is not supplied. Defaults to null.
        /// </param>
        /// <param name="deviceUser">
        /// If not null then only devices for this seat are considered if <paramref
        /// name="deviceId"/> is not supplied. Defaults to null.
        /// </param>
        /// <param name="deviceType">
        /// If not null then only devices for this category of CDU are considered if
        /// <paramref name="deviceId"/> is not supplied. Defaults to null.
        /// </param>
        /// <returns></returns>
        public static ICdu ConnectLocal(
            DeviceIdentifier deviceId = null,
            Device? device = null,
            DeviceUser? deviceUser = null,
            DeviceType? deviceType = null
        )
        {
            ICdu result = null;

            if(deviceId == null) {
                deviceId = FindLocalDevices()
                    .Where(candidate =>
                           (device == null || candidate.Device == device)
                        && (deviceUser == null || candidate.DeviceUser == deviceUser)
                        && (deviceType == null || candidate.DeviceType == deviceType)
                    )
                    // The order selected here is only to make it deterministic
                    .OrderBy(candidate => candidate.UsbVendorId)
                    .ThenBy(candidate => candidate.UsbProductId)
                    .FirstOrDefault();
            }

            if(deviceId != null) {
                var hidDevice = DeviceList
                    .Local
                    .GetHidDevices(
                        vendorID: deviceId.UsbVendorId,
                        productID: deviceId.UsbProductId
                    )
                    .FirstOrDefault();
                if(hidDevice != null) {
                    switch(deviceId.Device) {
                        case Device.WinctrlMcdu:
                            var mcdu = new McduDevice(hidDevice, deviceId);
                            mcdu.Initialise();
                            result = mcdu;
                            break;
                        case Device.WinctrlPfp3N:
                            var pfp3N = new Pfp3NDevice(hidDevice, deviceId);
                            pfp3N.Initialise();
                            result = pfp3N;
                            break;
                        case Device.WinctrlPfp7:
                            var pfp7 = new Pfp7Device(hidDevice, deviceId);
                            pfp7.Initialise();
                            result = pfp7;
                            break;
                        case Device.WinctrlPfp4:
                            var pfp4 = new Pfp4Device(hidDevice, deviceId);
                            pfp4.Initialise();
                            result = pfp4;
                            break;
                    }
                }
            }

            return result;
        }
    }
}
