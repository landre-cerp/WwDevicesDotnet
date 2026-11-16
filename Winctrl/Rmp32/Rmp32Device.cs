// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

using HidSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WwDevicesDotNet.Winctrl.Rmp32
{
    /// <summary>
    /// Winctrl 32 RMP (Radio Management Panel), Left (0xBB83), Right (0xBB84)
    /// or Centre (0xBB85) configuration. The L and C captures use the same
    /// protocol and the same command prefix 82 BB.
    ///
    /// Display protocol (same chunked 0xF0 stream as the AGP32):
    ///   cmd 0x02 framebuffer write, dataLen 0x28 (40 bytes):
    ///     bytes 4..9   ACTIVE display, 6 digits left to right
    ///     bytes 10..15 STBY/CRS display, 6 digits left to right
    ///     other bytes  0 (not yet understood)
    ///   Each digit byte is a 7-segment glyph, bit 0 = a .. bit 6 = g, bit 7 = decimal point.
    ///   cmd 0x03 commit (dataLen 0).
    ///   The 57-byte write message does not fit the 56-byte first chunk: its
    ///   last byte spills into the second report, ahead of the commit message.
    ///
    /// Illumination (14 byte 02 82 BB 00 00 03 49 type value):
    ///   type 00 = backlight, 01 = digital tube backlight (0..255),
    ///   type 02 = marker light (assumed, not seen in captures),
    ///   type 03..10 = IND1..IND14 (0 / 1).
    ///
    /// Input report (24 bytes, ID 01): 21 buttons in bytes 1-3, see <see cref="ControlMap"/>.
    /// </summary>
    public class Rmp32Device : BaseFrontpanelDevice<Control>
    {
        const ushort prefix = 0x82BB;

        const byte CmdWriteFramebuffer = 0x02;
        const byte CmdCommit = 0x03;
        const int FramebufferLength = 40;
        const int ActiveOffset = 4;
        const int StandbyOffset = 10;
        const int DigitCount = 6;
        const int FirstChunkLength = 56;

        ushort _SequenceNumber;
        readonly Stopwatch _Uptime = Stopwatch.StartNew();

        /// <inheritdoc/>
        public override IFrontpanelCapabilities Capabilities { get; } = new Rmp32Capabilities();

        public Rmp32Device(HidDevice hidDevice, DeviceIdentifier deviceId)
            : base(hidDevice, deviceId)
        {
        }

        /// <inheritdoc/>
        protected override void SendInitPacket()
        {
            // Stream reset: F0 02 followed by zeros.
            var packet = new byte[64];
            packet[0] = 0xF0;
            packet[1] = 0x02;
            SendCommand(packet);
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
            if (!IsConnected || !(state is Rmp32State rmpState))
            {
                return;
            }

            var framebuffer = new byte[FramebufferLength];
            PutText(framebuffer, ActiveOffset, rmpState.ActiveDisplay);
            PutText(framebuffer, StandbyOffset, rmpState.StandbyDisplay);

            var timestamp = (uint)_Uptime.ElapsedMilliseconds;
            var stream = new List<byte>();
            stream.AddRange(BuildMessage(CmdWriteFramebuffer, timestamp, framebuffer));
            stream.AddRange(BuildMessage(CmdCommit, timestamp, null));

            // Split the byte stream into F0 reports: the first chunk is 56 bytes,
            // the remainder (1 spilled data byte + the commit message) follows.
            var first = Math.Min(FirstChunkLength, stream.Count);
            SendCommand(BuildReport(stream.GetRange(0, first)));
            if (stream.Count > first)
            {
                SendCommand(BuildReport(stream.GetRange(first, stream.Count - first)));
            }
        }

        /// <inheritdoc/>
        public override void UpdateLeds(IFrontpanelLeds leds)
        {
            if (!IsConnected || !(leds is Rmp32Leds rmpLeds))
            {
                return;
            }

            foreach (var kvp in rmpLeds.States)
            {
                SendCommand(BuildIlluminationCommand((byte)kvp.Key, kvp.Value ? (byte)1 : (byte)0));
            }
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();

            UpdateDisplay(new Rmp32State());

            var leds = new Rmp32Leds();
            foreach (Rmp32Led led in Enum.GetValues(typeof(Rmp32Led)))
            {
                leds.Set(led, false);
            }
            UpdateLeds(leds);
        }

        /// <summary>
        /// Turns a single indicator LED on or off.
        /// </summary>
        public void SetLed(Rmp32Led led, bool on)
        {
            if (IsConnected)
            {
                SendCommand(BuildIlluminationCommand((byte)led, on ? (byte)1 : (byte)0));
            }
        }

        static byte[] BuildIlluminationCommand(byte type, byte value)
        {
            return new byte[14] {
                0x02,
                (byte)(prefix >> 8),
                (byte)(prefix & 0xFF),
                0x00, 0x00,
                0x03, 0x49,
                type, value,
                0x00, 0x00, 0x00, 0x00, 0x00,
            };
        }

        /// <summary>
        /// Builds one stream message: unit ID, command, timestamp, length, data.
        /// </summary>
        static byte[] BuildMessage(byte command, uint timestamp, byte[] data)
        {
            var message = new List<byte> {
                (byte)(prefix >> 8), (byte)(prefix & 0xFF), 0x00, 0x00,   // unit ID: 82 BB 00 00
                command, 0x01, 0x00, 0x00,
                (byte)timestamp, (byte)(timestamp >> 8), (byte)(timestamp >> 16), (byte)(timestamp >> 24),
                0x00,
            };
            var length = data == null ? 0 : data.Length;
            message.AddRange(new byte[] { (byte)length, (byte)(length >> 8), 0x00, 0x00 });
            if (data != null)
            {
                message.AddRange(data);
            }
            return message.ToArray();
        }

        byte[] BuildReport(List<byte> chunk)
        {
            _SequenceNumber++;
            if (_SequenceNumber == 0)
            {
                _SequenceNumber = 1;
            }

            var packet = new byte[64];
            packet[0] = 0xF0;
            packet[1] = (byte)(_SequenceNumber >> 8);
            packet[2] = (byte)(_SequenceNumber & 0xFF);
            packet[3] = (byte)chunk.Count;
            chunk.CopyTo(packet, 4);
            return packet;
        }

        static void PutText(byte[] framebuffer, int offset, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // Each '.' lights the decimal point of the digit before it.
            var glyphs = new List<byte>();
            foreach (var ch in text)
            {
                if (ch == '.' && glyphs.Count > 0 && (glyphs[glyphs.Count - 1] & 0x80) == 0)
                {
                    glyphs[glyphs.Count - 1] |= 0x80;
                }
                else
                {
                    glyphs.Add(Glyph(ch));
                }
            }

            // Right-align, like a number on a 7-segment display.
            var skip = Math.Max(0, glyphs.Count - DigitCount);
            var count = glyphs.Count - skip;
            var pad = DigitCount - count;
            for (var i = 0; i < count; ++i)
            {
                framebuffer[offset + pad + i] = glyphs[skip + i];
            }
        }

        static byte Glyph(char ch)
        {
            switch (char.ToUpperInvariant(ch))
            {
                case '0': return 0x3F;
                case '1': return 0x06;
                case '2': return 0x5B;
                case '3': return 0x4F;
                case '4': return 0x66;
                case '5': return 0x6D;
                case '6': return 0x7D;
                case '7': return 0x07;
                case '8': return 0x7F;
                case '9': return 0x6F;
                case '-': return 0x40;
                case '_': return 0x08;
                case 'A': return 0x77;
                case 'B': return 0x7C;
                case 'C': return 0x39;
                case 'D': return 0x5E;
                case 'E': return 0x79;
                case 'F': return 0x71;
                case 'H': return 0x76;
                case 'L': return 0x38;
                case 'N': return 0x54;
                case 'O': return 0x5C;
                case 'P': return 0x73;
                case 'R': return 0x50;
                case 'T': return 0x78;
                case 'U': return 0x3E;
                default: return 0x00;
            }
        }
    }
}
