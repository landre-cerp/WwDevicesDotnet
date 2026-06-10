// Copyright © 2025 onwards, Laurent Andre
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

namespace WwDevicesDotNet.Winctrl.Agp32
{
    /// <summary>
    /// Maps Winctrl 32 AGP Metal controls to their input report byte offsets and bit flags.
    /// TODO: populate from Wireshark trace.
    /// </summary>
    static class ControlMap
    {
        /// <summary>
        /// Returns the flag (bit mask) and offset (byte position) in the input report for a given control.
        /// </summary>
        /// <param name="control">The control to look up.</param>
        /// <returns>A tuple containing the flag (bit mask) and offset (byte position).</returns>
        public static (int Flag, int Offset) InputReport01FlagAndOffset(Control control)
        {
            switch(control) {

                case Control.BrkFanOn:  return (1 << 0, 1);
                case Control.BrkFanOff: return (1 << 1, 1);

                case Control.AutoBrkLo: return (1 << 2, 1);
                case Control.AutoBrkMed: return (1 << 3, 1);
                case Control.AutoBrkMax: return (1 << 4, 1);

                case Control.ASkidNwStrOn: return (1 << 5, 1);
                case Control.ASkidNwStrOff: return (1 << 6, 1);

                case Control.ChrRstLeft: return (1 << 7, 1);
                case Control.ChrRstPush: return (1 << 0, 2);
                case Control.ChrRstRight: return (1 << 1, 2);

                case Control.ChrLeft: return (1 << 2, 2);
                case Control.ChrPush: return (1 << 3, 2);
                case Control.ChrRight: return (1 << 4, 2);

                case Control.DateSetLeft: return (1 << 5, 2);
                case Control.DateSetPush: return (1 << 6, 2);
                case Control.DateSetRight: return (1 << 7, 2);

                case Control.Gps: return (1 << 0, 3);
                case Control.Int: return (1 << 1, 3);
                case Control.Set: return (1 << 2, 3);

                case Control.Run: return (1 << 3, 3);
                case Control.Stp: return (1 << 4, 3);
                case Control.Rst: return (1 << 5, 3);

                case Control.TerrOnNd: return (1 << 6, 3);

                case Control.GearUp: return (1 << 7, 3);
                case Control.GearDown: return (1 << 0, 4);



                default: return (0, 0);
            }
        }
    }
}
