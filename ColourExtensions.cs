// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Extension methods for the <see cref="Colour"/> enum.
    /// </summary>
    public static class ColourExtensions
    {
        [Obsolete("Legacy name, use ToWinctrlUsbColourAndFontCode instead")]
        public static (byte,byte) ToUsbColourAndFontCode(this Colour colour, bool isSmallFont)
        {
            return ToWinctrlUsbColourAndFontCode(colour, isSmallFont);
        }

        /// <summary>
        /// Converts the colour and font classification into the font and colour code used
        /// by all(?) Winctrl panels.
        /// </summary>
        /// <param name="colour"></param>
        /// <param name="isSmallFont"></param>
        /// <returns></returns>
        public static (byte,byte) ToWinctrlUsbColourAndFontCode(this Colour colour, bool isSmallFont)
        {
            var code = ToWinctrlColourOrdinal(colour) * 0x21;
            if(isSmallFont) {
                code += 0x16B;
            }

            return ((byte)(code & 0xff), (byte)((code & 0xff00) >> 8));
        }

        /// <summary>
        /// Returns the order in which the colour appears in Winctrl's 32bb...1901...0002
        /// and 32bb...1901...0003 packets. This feeds into the value to send for the colour
        /// when setting foreground (and background?).
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        public static int ToWinctrlColourOrdinal(this Colour colour)
        {
            switch(colour) {
                case Colour.Black:      return 0;
                case Colour.Amber:      return 1;
                case Colour.White:      return 2;
                case Colour.Cyan:       return 3;
                case Colour.Green:      return 4;
                case Colour.Magenta:    return 5;
                case Colour.Red:        return 6;
                case Colour.Yellow:     return 7;
                case Colour.Brown:      return 8;
                case Colour.Grey:       return 9;
                case Colour.Khaki:      return 10;
                default:                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the index number that represents this colour in the internal display buffer.
        /// </summary>
        /// <param name="colour"></param>
        /// <returns></returns>
        public static int ToDisplayBufferColourIndex(this Colour colour) => ToWinctrlColourOrdinal(colour);

        /// <summary>
        /// Returns the colour associated with the colour index passed across.
        /// </summary>
        /// <param name="colourIndex"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Colour FromDisplayBufferColourIndex(int colourIndex)
        {
            switch(colourIndex) {
                case 0:     return Colour.Black;
                case 1:     return Colour.Amber;
                case 2:     return Colour.White;
                case 3:     return Colour.Cyan;
                case 4:     return Colour.Green;
                case 5:     return Colour.Magenta;
                case 6:     return Colour.Red;
                case 7:     return Colour.Yellow;
                case 8:     return Colour.Brown;
                case 9:     return Colour.Grey;
                case 10:    return Colour.Khaki;
                default:    throw new NotImplementedException();
            }
        }

        public static char ToDuplicateCheckCode(this Colour colour, bool isSmallFont)
        {
            switch(colour) {
                case Colour.Black:      return isSmallFont ? 'l' : 'L';
                case Colour.Amber:      return isSmallFont ? 'a' : 'A';
                case Colour.Brown:      return isSmallFont ? 'b' : 'B';
                case Colour.Cyan:       return isSmallFont ? 'c' : 'C';
                case Colour.Green:      return isSmallFont ? 'g' : 'G';
                case Colour.Grey:       return isSmallFont ? 'e' : 'E';
                case Colour.Khaki:      return isSmallFont ? 'k' : 'K';
                case Colour.Magenta:    return isSmallFont ? 'm' : 'M';
                case Colour.Red:        return isSmallFont ? 'r' : 'R';
                case Colour.White:      return isSmallFont ? 'w' : 'W';
                case Colour.Yellow:     return isSmallFont ? 'y' : 'Y';
                default:                throw new NotImplementedException();
            }
        }
    }
}
