// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;
using System.Linq;
using HidSharp;
using WwDevicesDotNet.Winctrl.FcuAndEfis;
using WwDevicesDotNet.Winctrl.Pap3;
using WwDevicesDotNet.Winctrl.Pdc3nm;
using WwDevicesDotNet.Winctrl.Agp32;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Finds USB devices and creates instances of <see cref="IFrontpanel"/> implementations
    /// for them. Supports FCU, EFIS, and other Winctrl control panels.
    /// </summary>
    public static class FrontpanelFactory
    {
        //          ARE YOU HERE LOOKING FOR THE LIST OF ALL KNOWN DEVICE IDENTIFIERS?
        //                       They've moved to SupportedDevices.cs

        /// <summary>
        /// Returns a device identifier corresponding to the vendor and product IDs passed
        /// across, or null if the vendor and product ID do not correspond with a frontpanel
        /// that the library can interact with.
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
                .AllSupportedFrontpanels
                .FirstOrDefault(deviceIdentifier =>
                       deviceIdentifier.UsbVendorId == vendorId
                    && deviceIdentifier.UsbProductId == productId
                );
        }

        /// <summary>
        /// Returns a collection of all frontpanel devices that can be found on the local machine.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<DeviceIdentifier> FindLocalDevices()
        {
            var local = DeviceList.Local;
            return local
                .GetHidDevices()
                .Select(hidDevice => GetDeviceIdentifierForUsbIdentifiers(
                    hidDevice.VendorID,
                    hidDevice.ProductID
                ))
                .Where(deviceIdentifier => deviceIdentifier != null)
                .ToList();
        }

        /// <summary>
        /// Creates an initialised connection to a frontpanel device. If the device ID is not
        /// specified then the first frontpanel found on the system is used. If the requested
        /// frontpanel cannot be found (or there are no frontpanels to default to) then null
        /// is returned.
        /// </summary>
        /// <param name="deviceId">
        /// The specific device to connect to, if null then the first device found is
        /// used. Defaults to null.
        /// </param>
        /// <param name="device">
        /// If not null then only devices for this product are considered if <paramref
        /// name="deviceId"/> is not supplied. Defaults to null.
        /// </param>
        /// <param name="deviceType">
        /// If not null then only devices for this category of frontpanel are considered if
        /// <paramref name="deviceId"/> is not supplied. Defaults to null.
        /// </param>
        /// <returns></returns>
        public static IFrontpanel ConnectLocal(
            DeviceIdentifier deviceId = null,
            Device? device = null,
            DeviceType? deviceType = null
        )
        {
            IFrontpanel result = null;

            if(deviceId == null) {
                deviceId = FindLocalDevices()
                    .Where(candidate =>
                           (device == null || candidate.Device == device)
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
                        case Device.WinctrlFcu:
                        case Device.WinctrlFcuLeftEfis:
                        case Device.WinctrlFcuRightEfis:
                        case Device.WinctrlFcuBothEfis:
                            // All FCU configurations use the same device class
                            // The device ID determines which configuration is present
                            var fcu = new FcuEfisDevice(hidDevice, deviceId);
                            fcu.Initialise();
                            result = fcu;
                            break;
                        case Device.WinctrlPap3:
                            var pap3 = new Pap3Device(hidDevice, deviceId);
                            pap3.Initialise();
                            result = pap3;
                            break;
                        case Device.WinctrlPdc3n:
                            var pdc3 = new Pdc3Device(hidDevice, deviceId);
                            pdc3.Initialise();
                            result = pdc3;
                            break;
                        case Device.WinctrlAgp32:
                            var agp32 = new Agp32Device(hidDevice, deviceId);
                            agp32.Initialise();
                            result = agp32;
                            break;
                    }
                }
            }

            return result;
        }
    }
}
