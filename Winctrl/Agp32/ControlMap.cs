// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Agp32
{
    /// <summary>
    /// Maps Winctrl 32 AGP Metal controls to their input report byte offsets and bit flags.
    /// TODO: populate from Wireshark trace.
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

                case Control.BrkFanOn:  return (1 << 0, 1);
                case Control.BrkFanOff: return (1 << 1, 1);

                case Control.AutoBrkLo: return (1 << 2, 1);
                case Control.AutoBrkMed: return (1 << 3, 1);
                case Control.AutoBrkMax: return (1 << 4, 1);

                case Control.ASkidNwStrOn: return (1 << 5, 1);
                case Control.ASkidNwStrOff: return (1 << 6, 1);

                case Control.ChrRstLeft: return (1 << 7, 1);
                case Control.ChrRstPush: return (1 << 0, 2);
                case Control.ChrRstRight: return (1 << 1, 2);

                case Control.ChrLeft: return (1 << 2, 2);
                case Control.ChrPush: return (1 << 3, 2);
                case Control.ChrRight: return (1 << 4, 2);

                case Control.DateSetLeft: return (1 << 5, 2);
                case Control.DateSetPush: return (1 << 6, 2);
                case Control.DateSetRight: return (1 << 7, 2);

                case Control.Gps: return (1 << 0, 3);
                case Control.Int: return (1 << 1, 3);
                case Control.Set: return (1 << 2, 3);

                case Control.Run: return (1 << 3, 3);
                case Control.Stp: return (1 << 4, 3);
                case Control.Rst: return (1 << 5, 3);

                case Control.TerrOnNd: return (1 << 6, 3);

                case Control.GearUp: return (1 << 7, 3);
                case Control.GearDown: return (1 << 0, 4);



                default: return (0, 0);
            }
        }
    }
}
