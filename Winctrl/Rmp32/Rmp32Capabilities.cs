// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// Capabilities of the Winctrl 32 RMP: two 7-segment frequency displays
    /// (ACTIVE and STBY/CRS) and no autopilot-style displays.
    /// </summary>
    public class Rmp32Capabilities : IFrontpanelCapabilities
    {
        public bool HasSpeedDisplay => false;
        public bool HasHeadingDisplay => false;
        public bool HasAltitudeDisplay => false;
        public bool HasVerticalSpeedDisplay => false;
        public bool CanDisplayBarometricPressure => false;
        public bool CanDisplayQnhQfe => false;
        public bool HasPilotCourseDisplay => false;
        public bool HasCopilotCourseDisplay => false;
        public bool SupportsAlphanumericDisplay => false;
        public bool HasFlightLevelMode => false;
        public bool HasMachSpeedMode => false;
        public bool HasClockDisplay => false;
        public bool HasChronometerDisplay => false;
        public bool HasElapsedTimeDisplay => false;
    }
}
