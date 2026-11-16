// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// Maps Winctrl 32 RMP controls to input report byte offsets and bit flags.
    /// The report is 24 bytes: ID 0x01, three button bytes (21 buttons, bytes
    /// 1-3), then two little-endian 16-bit running encoder counters at bytes
    /// 10-11 (encoder A) and 12-13 (encoder B).
    /// </summary>
    static class ControlMap
    {
        public static (int Flag, int Offset) InputReport01FlagAndOffset(Control control)
        {
            // Control enum order == SimAppPro button number - 1.
            var bit = (int)control;
            return (1 << (bit % 8), 1 + bit / 8);
        }
    }
}
