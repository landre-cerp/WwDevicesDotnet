// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pap3
{
    /// <summary>
    /// An enumeration of all of the buttons and controls on the PAP-3 Primary Autopilot Panel.
    /// </summary>
    public enum Control
    {
        
        // Speed

        N1,
        Speed,
        CO,
        SpdIntv,
        SpdDec,
        SpdInc,

        ATArmOn,
        ATArmOff,

        // Flight director
        PltFdOn,
        CplFdOn,
        PltFdOff,
        CplFdOff,

        // Course section
        PltCourseDec,
        PltCourseInc,
        CplCourseDec,
        CplCourseInc,

        // Heading section
        HdgDec,
        HdgInc,

        // Altitude section
        AltDec,
        AltInc,

        // Vertical Speed section
        VsDn,
        VsUp,

        // Autopilot buttons
        CmdA,
        CmdB,
        CwsA,
        CwsB,

        // Autopilot modes
        Lnav,
        Vnav,
        LvlChg,
        HdgSel,
        VorLoc,
        App,
        AltHold,
        Vs,
        AltIntv,

        // Other controls
        Disengage,
        Bank10,
        Bank15,
        Bank20,
        Bank25,
        Bank30,
        DisengageDown,
        DisengageUp
    }
}
