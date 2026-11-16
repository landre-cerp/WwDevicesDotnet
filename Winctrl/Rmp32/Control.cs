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
    /// Confirmed by capture: 1-7, 15-21. Names for 1-14 follow the SimAppPro
    /// panel drawing and still need checking against the physical keys.
    /// Rotary encoders emit a pulse on a "left" or "right" button for each detent.
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

        // Which of the two rotary rings is "outer" and which is "inner" is
        // not confirmed yet: encoder A = 15/16, encoder B = 17/18.
        EncoderALeft,        // 15
        EncoderARight,       // 16
        EncoderBLeft,        // 17
        EncoderBRight,       // 18

        KnobPush,            // 19
        PowerOff,            // 20  ON/OFF toggle, OFF position
        PowerOn,             // 21  ON/OFF toggle, ON position
    }
}
