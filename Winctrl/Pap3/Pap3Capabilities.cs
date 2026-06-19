// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pap3
{
    /// <summary>
    /// Describes the capabilities of a Winctrl PAP-3 device.
    /// </summary>
    public class Pap3Capabilities : IFrontpanelCapabilities
    {
        /// <inheritdoc/>
        public bool HasSpeedDisplay => true;

        /// <inheritdoc/>
        public bool HasHeadingDisplay => true;

        /// <inheritdoc/>
        public bool HasAltitudeDisplay => true;

        /// <inheritdoc/>
        public bool HasVerticalSpeedDisplay => true;

        /// <inheritdoc/>
        public bool CanDisplayBarometricPressure => false;

        /// <inheritdoc/>
        public bool CanDisplayQnhQfe => false;

        /// <inheritdoc/>
        public bool HasPilotCourseDisplay => true;

        /// <inheritdoc/>
        public bool HasCopilotCourseDisplay => true;

        /// <inheritdoc/>
        public bool SupportsAlphanumericDisplay => true;

        /// <inheritdoc/>
        public bool HasFlightLevelMode => true;

        /// <inheritdoc/>
        public bool HasMachSpeedMode => false;
    }
}
