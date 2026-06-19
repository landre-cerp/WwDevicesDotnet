// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Text;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Identifies a USB Winctrl device supported
    /// </summary>
    public class DeviceIdentifier
    {
        /// <summary>
        /// Gets the USB Vendor ID returned by the device.
        /// </summary>
        public int UsbVendorId { get; }

        /// <summary>
        /// Gets the USB Product ID returned by the device.
        /// </summary>
        public int UsbProductId { get; }

        /// <summary>
        /// Gets the MCDU.NET device that corresponds to this vendor and product ID.
        /// </summary>
        public Device Device { get; }

        /// <summary>
        /// Gets the MCDU.NET device position (pilot, co-pilot etc.) that corresponds to
        /// this vendor and product ID.
        /// </summary>
        public DeviceUser DeviceUser { get; }

        /// <summary>
        /// Gets the broad category of device (Airbus A320 MCDU, Boeing 777 PFP) that this
        /// device replicates.
        /// </summary>
        public DeviceType DeviceType { get; }

        /// <summary>
        /// Gets a terse description of the device represented by this identifier.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="vendorId"></param>
        /// <param name="productId"></param>
        /// <param name="device"></param>
        /// <param name="deviceUser"></param>
        /// <param name="deviceType"></param>
        public DeviceIdentifier(
            string description,
            int vendorId,
            int productId,
            Device device,
            DeviceUser deviceUser,
            DeviceType deviceType
        )
        {
            Description = description;
            UsbVendorId = vendorId;
            UsbProductId = productId;
            Device = device;
            DeviceUser = deviceUser;
            DeviceType = deviceType;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = new StringBuilder(Device.ToString());
            if(DeviceUser != DeviceUser.NotApplicable) {
                result.Append(' ');
                result.Append(DeviceUser.ToString());
            }
            result.Append($" Vendor 0x{UsbVendorId:X4}");
            result.Append($" Product 0x{UsbProductId:X4}");

            return result.ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var result = Object.ReferenceEquals(this, obj);
            if(!result && obj is DeviceIdentifier other) {
                result = UsbVendorId == other.UsbVendorId
                      && UsbProductId == other.UsbProductId
                      && Device == other.Device
                      && DeviceUser == other.DeviceUser
                      && DeviceType == other.DeviceType;
            }

            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => UsbProductId.GetHashCode();

    }
}
