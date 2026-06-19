// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// The event args for the <see cref="ICdu.PaletteChanging"/> event.
    /// </summary>
    public class PaletteChangingEventArgs : EventArgs
    {
        /// <summary>
        /// The internal representation of the display's palette.
        /// </summary>
        public DisplayPalette DisplayPalette { get; }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="displayPalette"></param>
        public PaletteChangingEventArgs(DisplayPalette displayPalette)
        {
            DisplayPalette = displayPalette;
        }
    }
}
