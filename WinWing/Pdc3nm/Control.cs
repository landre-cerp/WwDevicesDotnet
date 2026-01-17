// Copyright © 2025 onwards, Laurent Andre
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

namespace WwDevicesDotNet.WinWing.Pdc3nm
{
    /// <summary>
    /// An enumeration of all of the buttons and controls on the PDC3 Autopilot front panel
    /// </summary>
    public enum Control
    {

        // Horizontal buttons
        WXR,
        STA,
        WPT,
        ARPT,
        DATA,
        POS,
        TERR,

        Nav1Vor1,
        Nav1Off,
        Nav1Adf1,

        Nav2Vor2,
        Nav2Off,
        Nav2Adf2,

        MinsBaro,
        MinsRadio,
        MinsRst,
        MinsCenter,
        MinsIncSlow,
        MinsDecSlow,
        MinsIncFast,
        MinsDecFast,

        FPV,
        MTRS,

        BaroIn,
        BaroHpa,
        BaroStd,
        BaroIncSlow,
        BaroDecSlow,
        BaroCenter,
        BaroIncFast,
        BaroDecFast,

        ModeApp,
        ModeVor,
        ModeMap,
        ModePln,
        ModeCtr, 

        Range5,
        Range10,
        Range20,
        Range40,
        Range80,
        Range160,
        Range320,
        Range640,
        RangeTFC


    }
}
