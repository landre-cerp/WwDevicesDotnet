// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    public static class CommonKeyExtensions
    {
        public static Key ToKey(this CommonKey commonKey, ICdu cdu)
        {
            if(commonKey < CommonKey.DeviceSpecific) {
                return (Key)commonKey;
            } else if(commonKey < CommonKey.EitherOr || cdu == null) {
                return (Key)(-1);
            } else {
                (Key Choice1, Key Choice2) choices;
                switch(commonKey) {
                    case CommonKey.InitOrInitRef:           choices = (Key.Init, Key.InitRef); break;
                    case CommonKey.McduMenuOrMenu:          choices = (Key.McduMenu, Key.Menu); break;
                    case CommonKey.FPlnOrRte:               choices = (Key.FPln, Key.Rte); break;
                    case CommonKey.AirportOrDepArr:         choices = (Key.Airport, Key.DepArr); break;
                    case CommonKey.RadNavOrNavRad:          choices = (Key.RadNav, Key.NavRad); break;
                    case CommonKey.RightArrowOrNextPage:    choices = (Key.RightArrow, Key.NextPage); break;
                    case CommonKey.LeftArrowOrPrevPage:     choices = (Key.LeftArrow, Key.PrevPage); break;
                    case CommonKey.SecFPlnOrAltn:           choices = (Key.SecFPln, Key.Altn); break;
                    case CommonKey.OvfyOrDel:               choices = (Key.Ovfy, Key.Del); break;
                    case CommonKey.AtcCommOrFmcComm:        choices = (Key.AtcComm, Key.FmcComm); break;
                    default:                                throw new NotImplementedException();
                }
                return cdu.IsKeySupported(choices.Choice1) ? choices.Choice1
                    : cdu.IsKeySupported(choices.Choice2) ? choices.Choice2
                    : (Key)(-1);
            }
        }

        public static string Describe(this CommonKey commonKey, ICdu cdu)
        {
            return ToKey(commonKey, cdu).Describe();
        }
    }
}
