// Copyright © 2025 onwards, Andrew Whewell
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.Collections.Generic;

namespace WwDevicesDotNet
{
    /// <summary>
    /// An enumeration of all supported devices.
    /// </summary>
    public static class SupportedDevices
    {
        /// <summary>
        /// The identifier for a Winctrl MCDU device set to the left-hand seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlMcduCaptainDevice = new DeviceIdentifier(
            "Winctrl MCDU (Captain)", 0x4098, 0xBB36, Device.WinctrlMcdu, DeviceUser.Captain, DeviceType.AirbusA320Mcdu
        );

        /// <summary>
        /// The identifier for a Winctrl MCDU device set to the right-hand seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlMcduFirstOfficerDevice = new DeviceIdentifier(
            "Winctrl MCDU (F/O)", 0x4098, 0xBB3E, Device.WinctrlMcdu, DeviceUser.FirstOfficer, DeviceType.AirbusA320Mcdu
        );

        /// <summary>
        /// The identifier for a Winctrl MCDU device set to the observer seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlMcduObserverDevice = new DeviceIdentifier(
            "Winctrl MCDU (Observer)", 0x4098, 0xBB3A, Device.WinctrlMcdu, DeviceUser.Observer, DeviceType.AirbusA320Mcdu
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-3N device set to the left-hand seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp3NCaptainDevice = new DeviceIdentifier(
            "Winctrl PFP-3N (Captain)", 0x4098, 0xBB35, Device.WinctrlPfp3N, DeviceUser.Captain, DeviceType.Boeing737NGPfp
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-3N device set to the right-hand seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp3NFirstOfficerDevice = new DeviceIdentifier(
            "Winctrl PFP-3N (F/O)", 0x4098, 0xBB3D, Device.WinctrlPfp3N, DeviceUser.FirstOfficer, DeviceType.Boeing737NGPfp
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-3N device set to the observer seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp3NObserverDevice = new DeviceIdentifier(
            "Winctrl PFP-3N (Observer)", 0x4098, 0xBB39, Device.WinctrlPfp3N, DeviceUser.Observer, DeviceType.Boeing737NGPfp
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-7 device set to the left-hand seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp7CaptainDevice = new DeviceIdentifier(
            "Winctrl PFP-7 (Captain)", 0x4098, 0xBB37, Device.WinctrlPfp7, DeviceUser.Captain, DeviceType.Boeing777Pfp
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-7 device set to the right-hand seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp7FirstOfficerDevice = new DeviceIdentifier(
            "Winctrl PFP-7 (F/O)", 0x4098, 0xBB3F, Device.WinctrlPfp7, DeviceUser.FirstOfficer, DeviceType.Boeing777Pfp
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-7 device set to the observer seat position.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp7ObserverDevice = new DeviceIdentifier(
            "Winctrl PFP-7 (Observer)", 0x4098, 0xBB3B, Device.WinctrlPfp7, DeviceUser.Observer, DeviceType.Boeing777Pfp
        );

        /// <summary>
        /// The identifier for a Winctrl FCU device (standalone, no EFIS).
        /// </summary>
        public static readonly DeviceIdentifier WinctrlFcuDevice = new DeviceIdentifier(
            "Winctrl FCU", 0x4098, 0xBB10, Device.WinctrlFcu, DeviceUser.NotApplicable, DeviceType.AirbusA320Fcu
        );

        /// <summary>
        /// The identifier for a Winctrl FCU device with left EFIS attached.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlFcuLeftEfisDevice = new DeviceIdentifier(
            "Winctrl FCU + Left EFIS", 0x4098, 0xBC1D, Device.WinctrlFcuLeftEfis, DeviceUser.NotApplicable, DeviceType.AirbusA320Fcu
        );

        /// <summary>
        /// The identifier for a Winctrl FCU device with right EFIS attached.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlFcuRightEfisDevice = new DeviceIdentifier(
            "Winctrl FCU + Right EFIS", 0x4098, 0xBC1E, Device.WinctrlFcuRightEfis, DeviceUser.NotApplicable, DeviceType.AirbusA320Fcu
        );

        /// <summary>
        /// The identifier for a Winctrl FCU device with both left and right EFIS attached.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlFcuBothEfisDevice = new DeviceIdentifier(
            "Winctrl FCU + Both EFIS", 0x4098, 0xBA01, Device.WinctrlFcuBothEfis, DeviceUser.NotApplicable, DeviceType.AirbusA320Fcu
        );

        /// <summary>
        /// The identifier for a Winctrl PAP-3 Primary Autopilot Panel.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPap3Device = new DeviceIdentifier(
            "Winctrl PAP-3", 0x4098, 0xBF0F, Device.WinctrlPap3, DeviceUser.NotApplicable, DeviceType.Boeing737FrontPanel
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-4 device set as Captain/Left seat.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp4CaptainDevice = new DeviceIdentifier(
            "Winctrl PFP-4 (Captain)", 0x4098, 0xBB38, Device.WinctrlPfp4, DeviceUser.NotApplicable, DeviceType.Boeing747Cdu
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-4 device set as Observer seat.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp4ObserverDevice = new DeviceIdentifier(
            "Winctrl PFP-4 (Observer)", 0x4098, 0xBB3C, Device.WinctrlPfp4, DeviceUser.NotApplicable, DeviceType.Boeing747Cdu
        );

        /// <summary>
        /// The identifier for a Winctrl PFP-4 device set as First Officer/Right seat.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPfp4FirstOfficerDevice = new DeviceIdentifier(
            "Winctrl PFP-4 (First Officer)", 0x4098, 0xBB40, Device.WinctrlPfp4, DeviceUser.NotApplicable, DeviceType.Boeing747Cdu
        );

        /// <summary>
        /// The identifier for a Winctrl PDC-3 device Left PDC unit.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPdc3NDeviceLeft = new DeviceIdentifier(
            "Winctrl PDC-3NL", 0x4098, 0xBB61, Device.WinctrlPdc3n, DeviceUser.NotApplicable, DeviceType.PDC3N
        );

        /// <summary>
        /// The identifier for a Winctrl PDC-3 device configured as Right unit.
        /// </summary>
        public static readonly DeviceIdentifier WinctrlPdc3NDeviceRight = new DeviceIdentifier(
            "Winctrl PDC-3NR", 0x4098, 0xBB62, Device.WinctrlPdc3n, DeviceUser.NotApplicable, DeviceType.PDC3N
        );


        private static readonly DeviceIdentifier[] _AllSupportedDevices = new DeviceIdentifier[] {
            WinctrlMcduCaptainDevice,
            WinctrlMcduFirstOfficerDevice,
            WinctrlMcduObserverDevice,

            WinctrlPfp3NCaptainDevice,
            WinctrlPfp3NFirstOfficerDevice,
            WinctrlPfp3NObserverDevice,

            WinctrlPfp7CaptainDevice,
            WinctrlPfp7FirstOfficerDevice,
            WinctrlPfp7ObserverDevice,

            WinctrlPfp4CaptainDevice,
            WinctrlPfp4FirstOfficerDevice,
            WinctrlPfp4ObserverDevice
        };

        private static readonly DeviceIdentifier[] _AllSupportedFrontpanels = new DeviceIdentifier[] {
            WinctrlFcuDevice,
            WinctrlFcuLeftEfisDevice,
            WinctrlFcuRightEfisDevice,
            WinctrlFcuBothEfisDevice,
            WinctrlPap3Device,
            WinctrlPdc3NDeviceLeft,
            WinctrlPdc3NDeviceRight,
        };

        /// <summary>
        /// A collection of device identifiers for all supported CDU devices.
        /// </summary>
        public static IReadOnlyList<DeviceIdentifier> AllSupportedDevices => _AllSupportedDevices;

        /// <summary>
        /// A collection of device identifiers for all supported frontpanel devices (FCU, EFIS, PAP-3, etc.).
        /// </summary>
        public static IReadOnlyList<DeviceIdentifier> AllSupportedFrontpanels => _AllSupportedFrontpanels;
    }
}
