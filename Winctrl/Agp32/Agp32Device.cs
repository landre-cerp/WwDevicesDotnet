// Copyright © 2025 onwards, Laurent Andre
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using HidSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static WwDevicesDotNet.Winctrl.Agp32.Agp32State;

namespace WwDevicesDotNet.Winctrl.Agp32
{
    /// <summary>
    /// Represents a Winctrl 32 AGP Metal panel device (A320-style clock / chrono).
    /// Handles communication with the physical hardware via HID protocol.
    ///
    /// Protocol (decoded from the Wireshark trace, filtered on 0xF0 URB OUT):
    ///
    ///   The 0xF0 report is a chunked transport carrying a byte stream:
    ///     F0 [SQ hi] [SQ lo] [chunkLen] [chunk bytes ...]
    ///     F0 02 [all zeros]                              = stream reset / init
    ///
    ///   Messages inside the stream:
    ///     [unitId u32 LE] [cmd u8] 01 00 00 [timestampMs u32 LE] [00] [dataLen u32 LE] [data]
    ///       cmd 0x02 : framebuffer write (AGP32: dataLen = 0x24 = 36 = 9 x u32 LE)
    ///       cmd 0x03 : commit / end-of-frame (dataLen = 0)
    ///
    ///   Unlike the PAP3 (176-byte framebuffer split over 4 reports), the AGP32
    ///   framebuffer fits in a single chunk, so one display update is exactly
    ///   two reports: a 0x35-length cmd-02 report and a 0x11-length cmd-03 report.
    ///
    ///   Framebuffer layout (9 little-endian u32 words at packet offset 0x15):
    ///     word 0      : always 0
    ///     words 1..7  : segment planes a,b,c,d,e,f,g — bit n = digit column n
    ///     word 8      : punctuation plane (colon dots) — bit n = column n
    ///   Digit columns: 0-3 = CHR MM SS (presumed, blank in trace),
    ///                  4-9 = clock HH MM SS, 10-13 = ET HH MM.
    /// </summary>
    public class Agp32Device : BaseFrontpanelDevice<Control>
    {
        /// <summary>
        /// Display unit ID in PAP3 convention (packet[4] = prefix >> 8, packet[5] = prefix &amp; 0xFF).
        /// The trace shows wire bytes 80 BB — the low half of a u32 LE 0x0000BB80 (48000) —
        /// so the prefix is 0x80BB.
        /// </summary>
        const ushort prefix = 0x80BB;

        const byte CmdWriteFramebuffer = 0x02;
        const byte CmdCommit = 0x03;

        // First digit column of each display field.
        const int ChrColumn = 0;     // presumed — not exercised in the trace
        const int ClockColumn = 4;
        const int EtColumn = 10;

        // Punctuation plane (word 8) bits. Trace value while clock + ET shown: 0x31E0
        // = bits 5,6,7,8 (the two clock colons) + bits 12,13 (the ET colon).
        // Hardware testing indicates CHR has one colon as well (88:88), mapped to bits 2,3.
        const uint PunctChrColon = (1u << 2) | (1u << 3);
        const uint PunctClockColons = (1u << 5) | (1u << 6) | (1u << 7) | (1u << 8);
        const uint PunctEtColon = (1u << 12) | (1u << 13);

        // Standard 7-segment glyphs for '0'..'9', bit 0 = segment a .. bit 6 = segment g.
        static readonly byte[] _Digit7Seg = { 0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7F, 0x6F };

        const int LeftAxisLightSensorOffset = 17;
        const int RightAxisLightSensorOffset = 19;
        const uint AxisLightSensorMax = 4095;

        ushort _SequenceNumber;
        readonly Stopwatch _Uptime = Stopwatch.StartNew();

        /// <inheritdoc/>
        public override IFrontpanelCapabilities Capabilities { get; } = new Agp32Capabilities();

        /// <summary>
        /// Gets the left raw axis-light sensor value (0..4095).
        /// </summary>
        public ushort LeftAxisLightRaw { get; private set; }

        /// <summary>
        /// Gets the right raw axis-light sensor value (0..4095).
        /// </summary>
        public ushort RightAxisLightRaw { get; private set; }

        /// <summary>
        /// Gets the fused axis-light value scaled to 0..65535.
        /// </summary>
        public ushort AxisLightValue { get; private set; }

        /// <summary>
        /// Raised when axis-light values change.
        /// </summary>
        public event EventHandler<Agp32AxisLightChangedEventArgs> AxisLightChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Agp32Device"/> class.
        /// </summary>
        /// <param name="hidDevice">The HID device to communicate with.</param>
        /// <param name="deviceId">The device identifier.</param>
        public Agp32Device(HidDevice hidDevice, DeviceIdentifier deviceId)
            : base(hidDevice, deviceId)
        {
        }

        /// <inheritdoc/>
        protected override void SendInitPacket()
        {
            // Stream reset: F0 02 followed by zeros. SimAppPro sends this before
            // the first display data (repeatedly at ~5s intervals while idle in
            // the trace; once is sufficient). Sequence numbering restarts after it.
            var packet = new byte[64];
            packet[0] = 0xF0;
            packet[1] = 0x02;
            SendCommand(packet);                 // TODO: use the same write path as Pap3Device
            _SequenceNumber = 0;
        }

        /// <inheritdoc/>
        protected override Control? GetControl(int offset, byte flag)
        {
            foreach (Control control in Enum.GetValues(typeof(Control)))
            {
                var (mapFlag, mapOffset) = ControlMap.InputReport01FlagAndOffset(control);
                if (mapOffset == offset && mapFlag == flag)
                {
                    return control;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        protected override void ProcessReport(byte[] data, int length)
        {
            if (length >= RightAxisLightSensorOffset + 2)
            {
                var leftRaw = (ushort)Math.Min(BitConverter.ToUInt16(data, LeftAxisLightSensorOffset), AxisLightSensorMax);
                var rightRaw = (ushort)Math.Min(BitConverter.ToUInt16(data, RightAxisLightSensorOffset), AxisLightSensorMax);
                var average12Bit = ((uint)leftRaw + (uint)rightRaw) / 2;
                var fusedValue = (ushort)((average12Bit * ushort.MaxValue) / AxisLightSensorMax);

                if (leftRaw != LeftAxisLightRaw || rightRaw != RightAxisLightRaw || fusedValue != AxisLightValue)
                {
                    LeftAxisLightRaw = leftRaw;
                    RightAxisLightRaw = rightRaw;
                    AxisLightValue = fusedValue;

                    AxisLightChanged?.Invoke(this, new Agp32AxisLightChangedEventArgs(AxisLightValue, LeftAxisLightRaw, RightAxisLightRaw));
                }
            }

            base.ProcessReport(data, length);
        }

        /// <inheritdoc/>
        public override void SetBrightness(byte panelBacklight, byte lcdBacklight, byte ledBacklight)
        {
            if (!IsConnected)
            {
                return;
            }

            SendCommand(BuildIlluminationCommand(0x00, panelBacklight));
            SendCommand(BuildIlluminationCommand(0x01, lcdBacklight));
            SendCommand(BuildIlluminationCommand(0x02, ledBacklight));
        }

        /// <inheritdoc/>
        public override void UpdateDisplay(IFrontpanelState state)
        {
            if (!IsConnected)
            {
                return;
            }

            if (state is Agp32State agp32State)
            {
                foreach (var packet in BuildAgp32DisplayCommands(agp32State))
                {
                    SendCommand(packet);             // TODO: use the same write path as Pap3Device
                }
            }
        }

        /// <inheritdoc/>
        public override void UpdateLeds(IFrontpanelLeds leds)
        {
            if (!IsConnected)
            {
                return;
            }

            if (leds is Agp32Leds agp32Leds)
            {
                foreach (var kvp in agp32Leds.States)
                {
                    SendCommand(BuildIlluminationCommand((byte)kvp.Key, kvp.Value ? (byte)0x01 : (byte)0x00));
                }
            }
        }

        /// <summary>
        /// Turns a single LED on or off.
        /// </summary>
        public void SetLed(Agp32Led led, bool on)
        {
            SendCommand(BuildIlluminationCommand((byte)led, on ? (byte)0x01 : (byte)0x00));
        }

        static byte[] BuildIlluminationCommand(byte type, byte value)
        {
            return new byte[14] {
                0x02,
                (byte)(prefix >> 8),     // 0x80
                (byte)(prefix & 0xFF),   // 0xBB
                0x00, 0x00,
                0x03, 0x49,
                type, value,
                0x00, 0x00, 0x00, 0x00, 0x00,
            };
        }

        /// <summary>
        /// Builds the two-packet display update sequence (framebuffer write + commit).
        /// </summary>
        List<byte[]> BuildAgp32DisplayCommands(Agp32State state)
        {
            var framebuffer = EncodeAgp32Displays(state);
            var timestamp = (uint)_Uptime.ElapsedMilliseconds;

            return new List<byte[]> {
                BuildReport(CmdWriteFramebuffer, timestamp, framebuffer),  // chunkLen 0x35
                BuildReport(CmdCommit, timestamp, null),                   // chunkLen 0x11
            };
        }

        /// <summary>
        /// Encodes the display state into the 9-word segment framebuffer.
        /// </summary>
        uint[] EncodeAgp32Displays(Agp32State state)
        {
            var framebuffer = new uint[9];      // word 0 stays 0

            PutField(framebuffer, ChrColumn, state.ChrDisplay, 4);
            PutField(framebuffer, ClockColumn, state.ClockDisplay, 6);
            PutField(framebuffer, EtColumn, state.EtDisplay, 4);

            if (!string.IsNullOrEmpty(state.ChrDisplay))
            {
                framebuffer[8] |= PunctChrColon;
            }
            if (!string.IsNullOrEmpty(state.ClockDisplay))
            {
                framebuffer[8] |= PunctClockColons;
            }
            if (!string.IsNullOrEmpty(state.EtDisplay))
            {
                framebuffer[8] |= PunctEtColon;
            }

            return framebuffer;
        }

        static void PutField(uint[] framebuffer, int firstColumn, string text, int width)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;                          // field stays blank (all segments off)
            }
            text = text.PadLeft(width);
            for (var i = 0; i < width; ++i)
            {
                PutDigit(framebuffer, firstColumn + i, text[i]);
            }
        }

        static void PutDigit(uint[] framebuffer, int column, char ch)
        {
            if (ch < '0' || ch > '9')
            {
                return;                          // space (or anything else) = blank digit
            }
            int glyph = _Digit7Seg[ch - '0'];
            for (var segment = 0; segment < 7; ++segment)
            {
                if ((glyph & (1 << segment)) != 0)
                {
                    framebuffer[1 + segment] |= 1u << column;
                }
            }
        }

        /// <summary>
        /// Wraps one protocol message into a 64-byte 0xF0 transport report.
        /// </summary>
        byte[] BuildReport(byte command, uint timestamp, uint[] framebuffer)
        {
            _SequenceNumber++;
            if (_SequenceNumber == 0)
            {
                _SequenceNumber = 1;
            }

            var packet = new byte[64];
            packet[0] = 0xF0;
            packet[1] = (byte)(_SequenceNumber >> 8);     // 16-bit big-endian sequence
            packet[2] = (byte)(_SequenceNumber & 0xFF);
            // packet[3] = chunk length, set below

            var offset = 4;
            packet[offset++] = (byte)(prefix >> 8);       // unit ID u32 LE: 80 BB 00 00
            packet[offset++] = (byte)(prefix & 0xFF);
            packet[offset++] = 0x00;
            packet[offset++] = 0x00;
            packet[offset++] = command;                   // 0x02 write / 0x03 commit
            packet[offset++] = 0x01;
            packet[offset++] = 0x00;
            packet[offset++] = 0x00;
            WriteU32(packet, ref offset, timestamp);      
            packet[offset++] = 0x00;
            var dataLength = framebuffer == null ? 0u : (uint)(framebuffer.Length * 4);
            WriteU32(packet, ref offset, dataLength);     // 0x24 for AGP32 (PAP3: 0xB0)
            if (framebuffer != null)
            {
                foreach (var word in framebuffer)
                {
                    WriteU32(packet, ref offset, word);
                }
            }

            packet[3] = (byte)(offset - 4);               // 0x35 (cmd 02) / 0x11 (cmd 03)
            return packet;
        }

        static void WriteU32(byte[] buffer, ref int offset, uint value)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
        }
    }

    /// <summary>
    /// Event arguments for AGP32 axis-light value changes.
    /// </summary>
    public class Agp32AxisLightChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the fused axis-light value scaled to 0..65535.
        /// </summary>
        public ushort Value { get; }

        /// <summary>
        /// Gets the left raw axis-light sensor value (0..4095).
        /// </summary>
        public ushort LeftRaw { get; }

        /// <summary>
        /// Gets the right raw axis-light sensor value (0..4095).
        /// </summary>
        public ushort RightRaw { get; }

        public Agp32AxisLightChangedEventArgs(ushort value, ushort leftRaw, ushort rightRaw)
        {
            Value = value;
            LeftRaw = leftRaw;
            RightRaw = rightRaw;
        }
    }

    /// <summary>
    /// Display state for the AGP32 panel. Digits only; null or empty fields are blanked.
    /// </summary>
    public class Agp32State : IFrontpanelState
    {
        /// <summary>Chronometer "MMSS" (column mapping presumed — verify with CHR running).</summary>
        public string ChrDisplay { get; set; }

        /// <summary>Clock / UTC "HHMMSS", e.g. "172917".</summary>
        public string ClockDisplay { get; set; }
        public int? Speed { get; set; }
        public int? Heading { get; set; }
        public int? Altitude { get; set; }
        public int? VerticalSpeed { get; set; }
        public string EtDisplay { get; set; }

        /// <summary>
        /// LED on/off type codes for the AGP32 03 49 illumination command.
        /// Mappings below reflect confirmed hardware tests.
        /// </summary>
        public enum Agp32Led : byte
        {
            /// <summary>Landing gear red UNLK indicator #1.</summary>
            Gear1Unlk = 0x03,

            /// <summary>Landing gear red UNLK indicator #2.</summary>
            Gear2Unlk = 0x04,

            /// <summary>Landing gear red UNLK indicator #3.</summary>
            Gear3Unlk = 0x05,

            /// <summary>Brake fan HOT light.</summary>
            BrkFanHot = 0x06,

            /// <summary>Landing gear green DOWN indicator #1.</summary>
            Gear1Down = 0x07,

            /// <summary>Landing gear green DOWN indicator #2.</summary>
            Gear2Down = 0x08,

            /// <summary>Landing gear green DOWN indicator #3.</summary>
            Gear3Down = 0x09,

            /// <summary>Brake fan ON light.</summary>
            BrkFanOn = 0x0A,

            /// <summary>AUTO BRK LO DECEL light.</summary>
            AutoBrkLoDecel = 0x0B,

            /// <summary>AUTO BRK MED DECEL light.</summary>
            AutoBrkMedDecel = 0x0C,

            /// <summary>AUTO BRK MAX DECEL light.</summary>
            AutoBrkMaxDecel = 0x0D,

            /// <summary>AUTO BRK LO ON light.</summary>
            AutoBrkLoOn = 0x0E,

            /// <summary>AUTO BRK MED ON light.</summary>
            AutoBrkMedOn = 0x0F,

            /// <summary>AUTO BRK MAX ON light.</summary>
            AutoBrkMaxOn = 0x10,

            /// <summary>TERR ON ND ON light.</summary>
            TerrOnNdOn = 0x11,

            /// <summary>Landing gear red DOWN light.</summary>
            GearDownRed = 0x12,
        }

        /// <summary>
        /// LED state model for the AGP32 panel.
        /// </summary>
        public class Agp32Leds : IFrontpanelLeds
        {
            readonly Dictionary<Agp32Led, bool> _States = new Dictionary<Agp32Led, bool>();

            /// <summary>The LEDs to set and their desired on/off state.</summary>
            public IEnumerable<KeyValuePair<Agp32Led, bool>> States => _States;

            public void Set(Agp32Led led, bool on) => _States[led] = on;
        }
    }
}
