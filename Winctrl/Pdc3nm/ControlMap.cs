// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pdc3nm
{
    /// <summary>
    /// Maps PDC-3 controls to their input report byte offsets and bit flags.
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

                case Control.FPV:           return (0x01, 1);
                case Control.MTRS:          return (0x02, 1);
                case Control.WXR:           return (0x04, 1);
                case Control.STA:           return (0x08, 1);
                case Control.WPT:           return (0x10, 1);
                case Control.ARPT:          return (0x20, 1); 
                case Control.DATA:          return (0x40, 1);
                case Control.POS:           return (0x80, 1);

                case Control.TERR:          return (0x01, 2);
                case Control.Nav1Vor1:      return (0x02, 2);
                case Control.Nav1Off:       return (0x04, 2);
                case Control.Nav1Adf1:      return (0x08, 2);
                case Control.Nav2Vor2:      return (0x10, 2);
                case Control.Nav2Off:       return (0x20, 2);
                case Control.Nav2Adf2:      return (0x40, 2);
                case Control.MinsRst:       return (0x80, 2);

                case Control.ModeCtr:       return (0x01, 3);
                case Control.RangeTFC:      return (0x02, 3);
                case Control.BaroStd:       return (0x04, 3);
                case Control.MinsDecFast:   return (0x08, 3);
                case Control.MinsIncFast:   return (0x10, 3);
                case Control.BaroDecFast:   return (0x20, 3);
                case Control.BaroIncFast:   return (0x40, 3);
                case Control.MinsRadio:     return (0x80, 3);

                case Control.MinsBaro:      return (0x01, 4);
                case Control.BaroIn:        return (0x02, 4);
                case Control.BaroHpa:       return (0x04, 4);
                case Control.ModeApp:       return (0x08, 4);
                case Control.ModeVor:       return (0x10, 4);
                case Control.ModeMap:       return (0x20, 4);
                case Control.ModePln:       return (0x40, 4);
                case Control.Range5:        return (0x80, 4);

                case Control.Range10:       return (0x01, 5);
                case Control.Range20:       return (0x02, 5);
                case Control.Range40:       return (0x04, 5);
                case Control.Range80:       return (0x08, 5);
                case Control.Range160:      return (0x10, 5);
                case Control.Range320:      return (0x20, 5);
                case Control.Range640:      return (0x40, 5);

                case Control.MinsDecSlow:   return (0x80, 5);
               
                case Control.MinsCenter:   return (0x01, 6);
                case Control.MinsIncSlow:   return (0x02, 6);
                case Control.BaroDecSlow:    return (0x04, 6);
                case Control.BaroCenter:   return (0x08, 6);
                case Control.BaroIncSlow:   return (0x10, 6);


                default:                    return (0, 0);
            }
        }
    }
}
