// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    public static class CommonLedExtensions
    {
        public static Led ToLed(this CommonLed commonLed, ICdu cdu)
        {
            if(commonLed < CommonLed.DeviceSpecific) {
                return (Led)commonLed;
            } else if(commonLed == CommonLed.DeviceSpecific || cdu == null) {
                return (Led)(-1);
            } else {
                (Led Choice1, Led Choice2) choices;
                switch(commonLed) {
                    case CommonLed.LineOrExec: choices = (Led.Line, Led.Exec); break;
                    default:                   throw new NotImplementedException();
                }
                return cdu.IsLedSupported(choices.Choice1) ? choices.Choice1
                    : cdu.IsLedSupported(choices.Choice2) ? choices.Choice2
                    : (Led)(-1);
            }
        }

        public static string Describe(this CommonLed commonLed, ICdu cdu)
        {
            return ToLed(commonLed, cdu).Describe();
        }
    }
}
