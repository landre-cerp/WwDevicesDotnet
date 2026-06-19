// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Collections.Generic;
using HidSharp;

namespace WwDevicesDotNet.Winctrl.Mcdu
{
    /// <summary>
    /// The implementation of <see cref="IMcdu"/> for the Winctrl MCDU.
    /// </summary>
    class McduDevice : CommonWinctrlPanel
    {
        protected override byte CommandPrefix => 0x32;

        private static readonly Dictionary<Led, byte> _LedIndicatorCodeMap = new Dictionary<Led, byte>() {
            { Led.Fail, 0x08 },
            { Led.Fm, 0x09 },
            { Led.Mcdu, 0x0a },
            { Led.Menu, 0x0b },
            { Led.Fm1, 0x0c },
            { Led.Ind, 0x0d },
            { Led.Rdy, 0x0e },
            { Led.Line, 0x0f },
            { Led.Fm2, 0x10 },
        };
        protected override Dictionary<Led, byte> LedIndicatorCodeMap => _LedIndicatorCodeMap;

        protected override Func<Key, (int Flag, int Offset)> KeyToFlagOffsetCallback => KeyboardMap.InputReport01FlagAndOffset;

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="hidDevice"></param>
        /// <param name="deviceId"></param>
        public McduDevice(HidDevice hidDevice, DeviceIdentifier deviceId) : base(hidDevice, deviceId)
        {
        }

        /// <inheritdoc/>
        ~McduDevice() => Dispose(false);
    }
}
