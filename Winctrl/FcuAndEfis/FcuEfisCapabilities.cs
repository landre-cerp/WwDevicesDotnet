// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.FcuAndEfis
{
    /// <summary>
    /// Describes the capabilities of a Winctrl FCU (with optional EFIS) device.
    /// </summary>
    public class FcuEfisCapabilities : IFrontpanelCapabilities
    {
        private readonly bool _hasLeftEfis;
        private readonly bool _hasRightEfis;

        public FcuEfisCapabilities(bool hasLeftEfis, bool hasRightEfis)
        {
            _hasLeftEfis = hasLeftEfis;
            _hasRightEfis = hasRightEfis;
        }

        /// <inheritdoc/>
        public bool HasSpeedDisplay => true;

        /// <inheritdoc/>
        public bool HasHeadingDisplay => true;

        /// <inheritdoc/>
        public bool HasAltitudeDisplay => true;

        /// <inheritdoc/>
        public bool HasVerticalSpeedDisplay => true;

        /// <inheritdoc/>
        public bool CanDisplayBarometricPressure => _hasLeftEfis || _hasRightEfis;

        /// <inheritdoc/>
        public bool CanDisplayQnhQfe => _hasLeftEfis || _hasRightEfis;

        /// <inheritdoc/>
        public bool HasPilotCourseDisplay => false;

        /// <inheritdoc/>
        public bool HasCopilotCourseDisplay => false;

        /// <inheritdoc/>
        public bool SupportsAlphanumericDisplay => true;

        /// <inheritdoc/>
        public bool HasFlightLevelMode => true;

        /// <inheritdoc/>
        public bool HasMachSpeedMode => true;
    }
}
