// Copyright © 2025 onwards, Laurent Andre
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
