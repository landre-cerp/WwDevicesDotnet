// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes the high-level capabilities of a frontpanel device.
    /// Used by clients to determine what features are supported by a specific device.
    /// </summary>
    public interface IFrontpanelCapabilities
    {
        /// <summary>
        /// Gets whether the device has a speed display.
        /// </summary>
        bool HasSpeedDisplay { get; }

        /// <summary>
        /// Gets whether the device has a heading display.
        /// </summary>
        bool HasHeadingDisplay { get; }

        /// <summary>
        /// Gets whether the device has an altitude display.
        /// </summary>
        bool HasAltitudeDisplay { get; }

        /// <summary>
        /// Gets whether the device has a vertical speed display.
        /// </summary>
        bool HasVerticalSpeedDisplay { get; }

        /// <summary>
        /// Gets whether the device can display barometric pressure.
        /// </summary>
        bool CanDisplayBarometricPressure { get; }

        /// <summary>
        /// Gets whether the device can display QNH/QFE indicators.
        /// </summary>
        bool CanDisplayQnhQfe { get; }

        /// <summary>
        /// Gets whether the device has pilot course display.
        /// </summary>
        bool HasPilotCourseDisplay { get; }

        /// <summary>
        /// Gets whether the device has copilot course display.
        /// </summary>
        bool HasCopilotCourseDisplay { get; }

        /// <summary>
        /// Gets whether the device supports alphanumeric display (not just numeric).
        /// </summary>
        bool SupportsAlphanumericDisplay { get; }

        /// <summary>
        /// Gets whether the device has flight level display mode.
        /// </summary>
        bool HasFlightLevelMode { get; }

        /// <summary>
        /// Gets whether the device has Mach speed display mode.
        /// </summary>
        bool HasMachSpeedMode { get; }
    }
}
