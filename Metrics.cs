// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    public static class Metrics
    {
        /// <summary>
        /// The number of lines of display on the MCDU.
        /// </summary>
        public const int Lines = 14;

        /// <summary>
        /// The number of columns of display on the MCDU.
        /// </summary>
        public const int Columns = 24;

        /// <summary>
        /// The total number of cells on the MCDU display.
        /// </summary>
        public const int Cells = Lines * Columns;

        /// <summary>
        /// Display width in pixels.
        /// </summary>
        public const int DisplayWidthPixels = (16 * 2) + (23 * Columns);

        /// <summary>
        /// Display height in pixels.
        /// </summary>
        public const int DisplayHeightPixels = (17 * 2) + (29 * Lines);

        /// <summary>
        /// The x of the left edge of the visible area, measured on PFP-3N, PFP-4, PFP-7
        /// and MCDU. All four share it; only the vertical aperture differs.
        /// </summary>
        public const int DisplayLeftPixel = 0x24;

        /// <summary>
        /// Where the text of a grid of <paramref name="columns"/> columns starts if it is
        /// to sit centred in the visible area, given the step the device advances by
        /// between characters.
        /// </summary>
        /// <remarks>
        /// The default grid lands on 0x34: 24 columns of 23px leave 32px of the 584px
        /// aperture spare, half of it either side. Wider grids have to start further
        /// left, and 25 columns of 23px start 12px inside the old margin - drawn from
        /// 0x34 they would run past the right edge instead.
        /// </remarks>
        /// <param name="glyphPixelWidth"></param>
        /// <param name="columns"></param>
        public static int TextOriginX(int glyphPixelWidth, int columns)
        {
            var spare = DisplayWidthPixels - (glyphPixelWidth * columns);
            return DisplayLeftPixel + (spare > 0 ? spare / 2 : 0);
        }
    }
}
