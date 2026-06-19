// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    public static class LedExtensions
    {
        public static string Describe(this Led led)
        {
            switch(led) {
                case Led.Dspy:  return "DSPY";
                case Led.Exec:  return "EXEC";
                case Led.Fail:  return "FAIL";
                case Led.Fm:    return "FM";
                case Led.Fm1:   return "FM1";
                case Led.Fm2:   return "FM2";
                case Led.Ind:   return "IND";
                case Led.Line:  return "LINE";
                case Led.Mcdu:  return "MCDU";
                case Led.Menu:  return "MENU";
                case Led.Msg:   return "MSG";
                case Led.Ofst:  return "OFST";
                case Led.Rdy:   return "RDY";
                case (Led)(-1): return "N/A";
                default:        return "";
            }
        }

        public static CommonLed ToCommonLed(this Led led)
        {
            switch(led) {
                case Led.Fail:  return CommonLed.Fail;
                case Led.Line:
                case Led.Exec:  return CommonLed.LineOrExec;
                default:        return CommonLed.DeviceSpecific;
            }
        }
    }
}
