// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// The event args for the <see cref="ICdu.FontChanging"/> event.
    /// </summary>
    public class FontChangingEventArgs : EventArgs
    {
        /// <summary>
        /// The internal representation of the font.
        /// </summary>
        public DisplayFont DisplayFont { get; }

        /// <summary>
        /// The XOffset that the font is using.
        /// </summary>
        public int XOffset { get; }

        /// <summary>
        /// The YOffset that the font is using.
        /// </summary>
        public int YOffset { get; }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="displayFont"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FontChangingEventArgs(DisplayFont displayFont, int x, int y)
        {
            DisplayFont = displayFont;
            XOffset = x;
            YOffset = y;
        }
    }
}
