// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pap3
{
    /// <summary>
    /// Maps PAP-3 controls to their input report byte offsets and bit flags.
    /// </summary>
    static class ControlMap
    {
        /// <summary>
        /// Returns the flag (bit mask) and offset (byte position) in the input report for a given control.
        /// </summary>
        /// <param name="control">The control to look up.</param>
        /// <returns>A tuple containing the flag (bit mask) and offset (byte position).</returns>
        public static (int Flag, int Offset) InputReport01FlagAndOffset(Control control)
        {
            switch(control) {

                case Control.N1:            return (0x01, 1);
                case Control.Speed:         return (0x02, 1);
                case Control.Vnav:          return (0x04, 1);
                case Control.LvlChg:        return (0x08, 1);
                case Control.HdgSel:        return (0x10, 1);
                case Control.Lnav:          return (0x20, 1); 
                case Control.VorLoc:        return (0x40, 1);
                case Control.App:           return (0x80, 1);

                case Control.AltHold:       return (0x01, 2);
                case Control.Vs:            return (0x02, 2);
                case Control.CmdA:          return (0x04, 2);
                case Control.CwsA:          return (0x08, 2);
                case Control.CmdB:          return (0x10, 2);
                case Control.CwsB:          return (0x20, 2);
                case Control.CO:            return (0x40, 2);
                case Control.SpdIntv:       return (0x80, 2);

                case Control.AltIntv:       return (0x01, 3);
                case Control.PltCourseDec:  return (0x02, 3);
                case Control.PltCourseInc:  return (0x04, 3);
                case Control.SpdDec:        return (0x08, 3);
                case Control.SpdInc:        return (0x10, 3);
                case Control.HdgDec:        return (0x20, 3);
                case Control.HdgInc:        return (0x40, 3);
                case Control.AltDec:        return (0x80, 3);

                case Control.AltInc:        return (0x01, 4);
                case Control.CplCourseDec:  return (0x02, 4);
                case Control.CplCourseInc:  return (0x04, 4);
                case Control.PltFdOn:       return (0x08, 4);
                case Control.PltFdOff:      return (0x10, 4);
                case Control.CplFdOn:       return (0x20, 4);
                case Control.CplFdOff:      return (0x40, 4);
                case Control.DisengageUp:   return (0x80, 4);

                case Control.DisengageDown: return (0x01, 5);
                case Control.Bank10:        return (0x02, 5);
                case Control.Bank15:        return (0x04, 5);
                case Control.Bank20:        return (0x08, 5);
                case Control.Bank25:        return (0x10, 5);
                case Control.Bank30:        return (0x20, 5);

                case Control.VsDn:          return (0x40, 5);
                case Control.VsUp:          return (0x80, 5);

                case Control.ATArmOff:      return (0x02, 6);
                case Control.ATArmOn:       return (0x01, 6);


                default:                        return (0, 0);
            }
        }
    }
}
