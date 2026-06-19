// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// An enumeration of all of the USB devices that the library can interact with.
    /// </summary>
    public enum Device
    {
        /// <summary>
        /// A Winctrl Airbus MCDU.
        /// </summary>
        WinctrlMcdu,

        /// <summary>
        /// A Winctrl Boeing 777 PFP-7.
        /// </summary>
        WinctrlPfp7,

        /// <summary>
        /// A Winctrl Boeing 737 PFP-3N. I do not have one of these so it might not work!
        /// </summary>
        WinctrlPfp3N,

        /// <summary>
        /// A Winctrl Airbus FCU panel (standalone, no EFIS attached).
        /// </summary>
        WinctrlFcu,

        /// <summary>
        /// A Winctrl Airbus FCU panel with left EFIS attached.
        /// </summary>
        WinctrlFcuLeftEfis,

        /// <summary>
        /// A Winctrl Airbus FCU panel with right EFIS attached.
        /// </summary>
        WinctrlFcuRightEfis,

        /// <summary>
        /// A Winctrl Airbus FCU panel with both left and right EFIS attached.
        /// </summary>
        WinctrlFcuBothEfis,

        /// <summary>
        /// A Winctrl PAP-3 Primary Autopilot Panel.
        /// </summary>
        WinctrlPap3,

        /// <summary>
        /// A Winctrl PFP-4.
        /// </summary>
        WinctrlPfp4,


        WinctrlPdc3n,

        /// <summary>
        /// A Winctrl 32 AGP Metal panel.
        /// </summary>
        WinctrlAgp32,
    }
}
