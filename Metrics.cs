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
    }
}
