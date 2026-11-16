// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// Display state for the RMP32: two 6-digit 7-segment displays.
    /// A '.' in the text lights the decimal point of the preceding digit,
    /// e.g. "124.850". Null or empty blanks the display.
    /// </summary>
    public class Rmp32State : IFrontpanelState
    {
        /// <summary>Text for the ACTIVE display.</summary>
        public string ActiveDisplay { get; set; }

        /// <summary>Text for the STBY/CRS display.</summary>
        public string StandbyDisplay { get; set; }

        // Not applicable to the RMP32, required by IFrontpanelState.
        public int? Speed { get; set; }
        public int? Heading { get; set; }
        public int? Altitude { get; set; }
        public int? VerticalSpeed { get; set; }
    }
}
