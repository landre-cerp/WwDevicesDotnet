// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pdc3nm
{
    /// <summary>
    /// Describes the capabilities of a Winctrl PDC-3 Navigation Display device.
    /// PDC-3 is primarily a control panel without autopilot displays.
    /// </summary>
    public class Pdc3Capabilities : IFrontpanelCapabilities
    {
        /// <inheritdoc/>
        public bool HasSpeedDisplay => false;

        /// <inheritdoc/>
        public bool HasHeadingDisplay => false;

        /// <inheritdoc/>
        public bool HasAltitudeDisplay => false;

        /// <inheritdoc/>
        public bool HasVerticalSpeedDisplay => false;

        /// <inheritdoc/>
        public bool CanDisplayBarometricPressure => false;

        /// <inheritdoc/>
        public bool CanDisplayQnhQfe => false;

        /// <inheritdoc/>
        public bool HasPilotCourseDisplay => false;

        /// <inheritdoc/>
        public bool HasCopilotCourseDisplay => false;

        /// <inheritdoc/>
        public bool SupportsAlphanumericDisplay => false;

        /// <inheritdoc/>
        public bool HasFlightLevelMode => false;

        /// <inheritdoc/>
        public bool HasMachSpeedMode => false;
    }
}
