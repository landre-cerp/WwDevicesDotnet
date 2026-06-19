// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Agp32
{
    /// <summary>
    /// Describes the capabilities of a Winctrl 32 AGP Metal panel.
    /// TODO: update once capabilities are known from the Wireshark trace.
    /// </summary>
    public class Agp32Capabilities : IFrontpanelCapabilities
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
