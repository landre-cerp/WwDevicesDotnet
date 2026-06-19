// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pdc3nm
{
    /// <summary>
    /// An enumeration of all of the buttons and controls on the PDC3 Autopilot front panel
    /// </summary>
    public enum Control
    {

        // Horizontal buttons
        WXR,
        STA,
        WPT,
        ARPT,
        DATA,
        POS,
        TERR,

        Nav1Vor1,
        Nav1Off,
        Nav1Adf1,

        Nav2Vor2,
        Nav2Off,
        Nav2Adf2,

        MinsBaro,
        MinsRadio,
        MinsRst,
        MinsCenter,
        MinsIncSlow,
        MinsDecSlow,
        MinsIncFast,
        MinsDecFast,

        FPV,
        MTRS,

        BaroIn,
        BaroHpa,
        BaroStd,
        BaroIncSlow,
        BaroDecSlow,
        BaroCenter,
        BaroIncFast,
        BaroDecFast,

        ModeApp,
        ModeVor,
        ModeMap,
        ModePln,
        ModeCtr, 

        Range5,
        Range10,
        Range20,
        Range40,
        Range80,
        Range160,
        Range320,
        Range640,
        RangeTFC


    }
}
