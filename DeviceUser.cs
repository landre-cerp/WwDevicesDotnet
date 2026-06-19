// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// An enumeration of the different locations of a device in aircraft that contain
    /// more than one instance.
    /// </summary>
    public enum DeviceUser
    {
        /// <summary>
        /// The manufacturer does not support setting per-location USB identifiers.
        /// </summary>
        NotApplicable,

        /// <summary>
        /// The device has been flagged as in use by the left-hand seat.
        /// </summary>
        Captain,

        /// <summary>
        /// The device has been flagged as in use by the right-hand seat.
        /// </summary>
        FirstOfficer,

        /// <summary>
        /// The device has been flagged as in use by the observer / engineer / jump seat.
        /// </summary>
        Observer,
    }
}
