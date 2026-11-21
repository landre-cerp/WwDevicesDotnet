// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// An enumeration of the buttons and controls on the Winctrl 32 RMP
    /// (Radio Management Panel). The numbers are the SimAppPro button numbers
    /// (1-based); button N is bit (N-1) of the three button bytes that follow
    /// the report ID in the input report.
    /// </summary>
    /// <remarks>
    /// All 21 buttons confirmed on hardware (RMP L).
    /// Rotary rings emit a pulse on a "left" or "right" button for each detent.
    /// SEL is an indicator, not a button.
    /// </remarks>
    public enum Control
    {
        Swap,                // 1  active / standby transfer

        Vhf1,                // 2
        Vhf2,                // 3
        Vhf3,                // 4
        Load,                // 5
        Hf1,                 // 6
        Hf2,                 // 7
        Am,                  // 8

        Nav,                 // 9
        Vor,                 // 10
        Ils,                 // 11
        Gls,                 // 12
        Mls,                 // 13
        Adf,                 // 14

        // The big knob has two rings (confirmed on hardware).
        LowerRingLeft,       // 15  lower ring, counter-clockwise
        LowerRingRight,      // 16  lower ring, clockwise
        UpperRingLeft,       // 17  upper ring, counter-clockwise
        UpperRingRight,      // 18  upper ring, clockwise

        KnobPush,            // 19
        PowerUp,             // 20  ON/OFF toggle, up position
        PowerDown,           // 21  ON/OFF toggle, down position (held at power-up)
    }
}
