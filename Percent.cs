// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    public static class Percent
    {
        /// <summary>
        /// Converts a percentage from 0 to 100 into a byte value from 0 to FF.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static byte ToByte(int percent)
        {
            return (byte)(255.0 * (Math.Max(0, Math.Min(100, percent)) / 100.0));
        }

        /// <summary>
        /// Normalises the value to lie within the range 0 to 100.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Clamp(int value) => Math.Max(0, Math.Min(100, value));
    }
}
