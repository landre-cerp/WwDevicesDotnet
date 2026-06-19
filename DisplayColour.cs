// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    public struct DisplayColour
    {
        /// <summary>
        /// The packed colour in RRGGBBAA format.
        /// </summary>
        public UInt32 PackedValue { get; set; }

        public byte Red
        {
            get => (byte)((PackedValue & 0xff000000) >> 24);
            set => PackedValue = ((uint)value << 24) | (PackedValue & 0x00ffffff);
        }

        public byte Green
        {
            get => (byte)((PackedValue & 0x00ff0000) >> 16);
            set => PackedValue = ((uint)value << 16) | (PackedValue & 0xff00ffff);
        }

        public byte Blue
        {
            get => (byte)((PackedValue & 0x0000ff00) >> 8);
            set => PackedValue = ((uint)value << 8) | (PackedValue & 0xffff00ff);
        }

        public byte Alpha
        {
            get => (byte)(PackedValue & 0x000000ff);
            set => PackedValue = (uint)value | (PackedValue & 0xffffff00);
        }

        public byte R
        {
            get => Red;
            set => Red = value;
        }

        public byte G
        {
            get => Green;
            set => Green = value;
        }

        public byte B
        {
            get => Blue;
            set => Blue = value;
        }

        public byte A
        {
            get => Alpha;
            set => Alpha = value;
        }

        public string WinctrlColourString => $"{B:X2}{G:X2}{R:X2}{A:X2}";

        public override string ToString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

        public bool CopyFrom(PaletteColour paletteColour)
        {
            if(paletteColour == null) {
                throw new ArgumentNullException(nameof(paletteColour));
            }
            var originalValue = PackedValue;

            PackedValue =
                  (uint)(paletteColour.R << 24)
                | (uint)(paletteColour.G << 16)
                | (uint)(paletteColour.B << 8)
                | (uint)paletteColour.A;

            return PackedValue != originalValue;
        }

        public void CopyTo(PaletteColour paletteColour)
        {
            if(paletteColour == null) {
                throw new ArgumentNullException(nameof(paletteColour));
            }
            paletteColour.R = R;
            paletteColour.G = G;
            paletteColour.B = B;
            paletteColour.A = A;
        }

        public void CopyFrom(DisplayColour other) => PackedValue = other.PackedValue;

        public void CopyTo(DisplayColour other) => other.PackedValue = PackedValue;
    }
}
