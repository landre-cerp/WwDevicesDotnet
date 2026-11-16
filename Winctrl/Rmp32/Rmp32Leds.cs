// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// The 14 indicator LEDs of the RMP32 (IND1..IND14 in SimAppPro), as the type
    /// codes used by the 03 49 illumination command. Each is the SEL lamp or the
    /// triangle above the key of the same name (confirmed on hardware).
    /// </summary>
    public enum Rmp32Led : byte
    {
        Sel = 0x03,   // IND1
        Vhf1,         // IND2
        Vhf2,         // IND3
        Vhf3,         // IND4
        Load,         // IND5
        Hf1,          // IND6
        Hf2,          // IND7
        Am,           // IND8
        Nav,          // IND9
        Vor,          // IND10
        Ils,          // IND11
        Gls,          // IND12
        Mls,          // IND13
        Adf,          // IND14
    }

    /// <summary>
    /// LED state model for the RMP32 panel.
    /// </summary>
    public class Rmp32Leds : IFrontpanelLeds
    {
        readonly Dictionary<Rmp32Led, bool> _States = new Dictionary<Rmp32Led, bool>();

        /// <summary>The LEDs to set and their desired on/off state.</summary>
        public IEnumerable<KeyValuePair<Rmp32Led, bool>> States => _States;

        public void Set(Rmp32Led led, bool on) => _States[led] = on;
    }
}
