// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// The 14 indicator LEDs of the RMP32 (IND1..IND14), as the type codes
    /// used by the 03 49 illumination command.
    /// </summary>
    public enum Rmp32Led : byte
    {
        Ind1 = 0x03, Ind2, Ind3, Ind4, Ind5, Ind6, Ind7,
        Ind8, Ind9, Ind10, Ind11, Ind12, Ind13, Ind14,
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
