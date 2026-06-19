// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes a copy of the palette held by the device. This is similar to <see
    /// cref="Palette"/> but it is at a lower level. It is not intended as a random write
    /// buffer, it is intended to be populated with an entire palette buffer in one go and
    /// to represent that buffer's contents reasonably efficiently.
    /// </summary>
    public class DisplayPalette
    {
        /// <summary>
        /// An array of colours, one for each colour index (as used by <see
        /// cref="DisplayBufferFontAndColour"/>).
        /// </summary>
        public DisplayColour[] Colours { get; }

        /// <summary>
        /// The number of colours in the palette.
        /// </summary>
        public int CountColours => Colours.Length;

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="countColours"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DisplayPalette(int countColours)
        {
            if(countColours < 1) {
                throw new ArgumentOutOfRangeException(nameof(countColours));
            }
            Colours = new DisplayColour[countColours];
        }

        public bool CopyFrom(PaletteColour[] colourArray)
        {
            if(colourArray == null) {
                throw new ArgumentNullException(nameof(colourArray));
            }
            if(colourArray.Length != CountColours) {
                throw new ArgumentOutOfRangeException(nameof(colourArray));
            }

            var result = false;
            for(var idx = 0;idx < CountColours;++idx) {
                result = Colours[idx].CopyFrom(colourArray[idx]) || result;
            }

            return result;
        }

        public void CopyFrom(DisplayPalette other)
        {
            if(other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            if(other.CountColours != CountColours) {
                throw new ArgumentOutOfRangeException(nameof(other));
            }
            for(var idx = 0;idx < CountColours;++idx) {
                Colours[idx].CopyFrom(other.Colours[idx]);
            }
        }

        public void CopyTo(DisplayPalette other) => other?.CopyTo(this);

        public DisplayPalette Clone()
        {
            var result = new DisplayPalette(CountColours);
            result.CopyFrom(this);
            return result;
        }
    }
}
