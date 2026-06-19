// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// General exceptions thrown by the WwDevicesDotNet library.
    /// </summary>
    public class WwDeviceException : Exception
    {
        public WwDeviceException()
        {
        }

        public WwDeviceException(string message) : base(message)
        {
        }

        public WwDeviceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
