// Copyright © 2025 onwards, Laurent Andre
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
