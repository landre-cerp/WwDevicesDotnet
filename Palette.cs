// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System.Text;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes the colours that make up the palette for an MCDU device.
    /// </summary>
    public class Palette
    {
        public const string DefaultBlackRgb =   "000000";
        public const string DefaultAmberRgb =   "FFA500";
        public const string DefaultWhiteRgb =   "FFFFFF";
        public const string DefaultCyanRgb =    "00FFFF";
        public const string DefaultGreenRgb =   "00FF3D";
        public const string DefaultMagentaRgb = "FF63FF";
        public const string DefaultRedRgb =     "FF0000";
        public const string DefaultYellowRgb =  "FFFF00";
        public const string DefaultBrownRgb =   "615C42";
        public const string DefaultGreyRgb =    "777777";
        public const string DefaultKhakiRgb =   "79735E";

        public PaletteColour Black { get; } = PaletteColour.Parse(DefaultBlackRgb);

        public PaletteColour Amber { get; } = PaletteColour.Parse(DefaultAmberRgb);

        public PaletteColour White { get; } = PaletteColour.Parse(DefaultWhiteRgb);

        public PaletteColour Cyan { get; } = PaletteColour.Parse(DefaultCyanRgb);

        public PaletteColour Green { get; } = PaletteColour.Parse(DefaultGreenRgb);

        public PaletteColour Magenta { get; } = PaletteColour.Parse(DefaultMagentaRgb);

        public PaletteColour Red { get; } = PaletteColour.Parse(DefaultRedRgb);

        public PaletteColour Yellow { get; } = PaletteColour.Parse(DefaultYellowRgb);

        public PaletteColour Brown { get; } = PaletteColour.Parse(DefaultBrownRgb);

        public PaletteColour Grey { get; } = PaletteColour.Parse(DefaultGreyRgb);

        public PaletteColour Khaki { get; } = PaletteColour.Parse(DefaultKhakiRgb);

        public PaletteColour[] ToWinctrlOrdinalColours()
        {
            return new PaletteColour[] {
                Black,
                Amber,
                White,
                Cyan,
                Green,
                Magenta,
                Red,
                Yellow,
                Brown,
                Grey,
                Khaki,
            };
        }

        public void CopyFrom(Palette other)
        {
            Black.CopyFrom(other?.Black);
            Amber.CopyFrom(other?.Amber);
            White.CopyFrom(other?.White);
            Cyan.CopyFrom(other?.Cyan);
            Green.CopyFrom(other?.Green);
            Magenta.CopyFrom(other?.Magenta);
            Red.CopyFrom(other?.Red);
            Yellow.CopyFrom(other?.Yellow);
            Brown.CopyFrom(other?.Brown);
            Grey.CopyFrom(other?.Grey);
            Khaki.CopyFrom(other?.Khaki);
        }

        public void CopyTo(Palette other) => other?.CopyFrom(this);
    }
}
