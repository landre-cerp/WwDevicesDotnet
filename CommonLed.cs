// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// The LEDs that appear on all CDU devices. This is a short list.
    /// </summary>
    public enum CommonLed
    {
        Fail = Led.Fail,

        DeviceSpecific = 100,

        EitherOr = 200,
        LineOrExec = EitherOr + 1
    }
}
