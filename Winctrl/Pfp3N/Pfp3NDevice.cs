// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Collections.Generic;
using HidSharp;

namespace WwDevicesDotNet.Winctrl.Pfp3N
{
    /// <summary>
    /// Implements <see cref="ICdu"/> for a Winctrl PFP-3N.
    /// </summary>
    class Pfp3NDevice : CommonWinctrlPanel
    {
        protected override byte CommandPrefix => 0x31;

        private static readonly Dictionary<Led, byte> _LedIndicatorCodeMap = new Dictionary<Led, byte>() {
            { Led.Dspy, 0x03 },
            { Led.Fail, 0x04 },
            { Led.Msg, 0x05 },
            { Led.Ofst, 0x06 },
            { Led.Exec, 0x07 },
        };
        protected override Dictionary<Led, byte> LedIndicatorCodeMap => _LedIndicatorCodeMap;

        protected override Func<Key, (int Flag, int Offset)> KeyToFlagOffsetCallback => KeyboardMap.InputReport01FlagAndOffset;

        public Pfp3NDevice(HidDevice hidDevice, DeviceIdentifier deviceId) : base(hidDevice, deviceId)
        {
        }

        /// <inheritdoc/>
        ~Pfp3NDevice() => Dispose(false);
    }
}
