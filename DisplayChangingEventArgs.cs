// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// The event args for the <see cref="ICdu.DisplayChanging"/> event.
    /// </summary>
    public class DisplayChangingEventArgs : EventArgs
    {
        /// <summary>
        /// The internal representation of the display content.
        /// </summary>
        public DisplayBuffer DisplayBuffer { get; }

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="displayBuffer"></param>
        public DisplayChangingEventArgs(DisplayBuffer displayBuffer)
        {
            DisplayBuffer = displayBuffer;
        }
    }
}
