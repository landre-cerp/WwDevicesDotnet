// Copyright © 2025 onwards Laurent Andre
// All rights reserved.
//
// Redistribution and use of this software in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//    * Neither the name of the author nor the names of the program's contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS OF THE SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using HidSharp;

namespace WwDevicesDotNet.WinWing.Pap3
{
    /// <summary>
    /// Represents a WinWing PAP-3 Primary Autopilot Panel device.
    /// Handles communication with the physical PAP-3 hardware via HID protocol.
    /// </summary>
    public class Pap3Device : BaseFrontpanelDevice<Control>
    {
        // Command prefix for PAP-3 panel (verified from hardware testing)
        const ushort _Pap3DisplayPrefix = 0x0FBF;
        const ushort _Pap3LedPrefix = 0x0100;

        // Brightness command types (verified from hardware testing)
        const byte _BrightnessPanelBacklight = 0x00;
        const byte _BrightnessDigitalTube = 0x01;
        const byte _BrightnessMarkerLight = 0x02;

        // Seven-segment display digit values.
        // Bit order matches `_SegmentBits`: G, F, E, D, C, B, A, DP
        static readonly byte[] _DigitValues = new byte[] {
            0x7E, // 0: A B C D E F
            0x0C, // 1: B C
            0xB6, // 2: A B D E G
            0x9E, // 3: A B C D G
            0xCC, // 4: B C F G
            0xDA, // 5: A C D F G
            0xFA, // 6: A C D E F G
            0x0E, // 7: A B C
            0xFE, // 8: A B C D E F G
            0xDE  // 9: A B C D F G
        };

        // Character to 7-segment mapping for alphanumeric display
        // Supports digits 0-9 and common letters that can be displayed on 7-segment displays
        // Bit order: G, F, E, D, C, B, A, DP
        static readonly Dictionary<char, byte> _CharacterValues = new Dictionary<char, byte>
        {
            // Digits (same as _DigitValues)
            {'0', 0x7E}, {'1', 0x0C}, {'2', 0xB6}, {'3', 0x9E}, {'4', 0xCC},
            {'5', 0xDA}, {'6', 0xFA}, {'7', 0x0E}, {'8', 0xFE}, {'9', 0xDE},
            
            // Uppercase letters (subset displayable on 7-segment)
            {'A', 0xEE}, // A B C E F G
            {'B', 0xF8}, // C D E F G (same as lowercase b)
            {'C', 0x72}, // A D E F
            {'D', 0xBC}, // B C D E G (same as lowercase d)
            {'E', 0xF2}, // A D E F G
            {'F', 0xE2}, // A E F G
            {'G', 0x7A}, // A C D E F (like 6 without top)
            {'H', 0xEC}, // B C E F G
            {'I', 0x0C}, // B C (same as 1)
            {'J', 0x3C}, // B C D E
            {'L', 0x70}, // D E F
            {'N', 0xA8}, // C E G (simplified)
            {'O', 0x7E}, // A B C D E F (same as 0)
            {'P', 0xE6}, // A B E F G
            {'Q', 0xCE}, // A B C F G
            {'R', 0xA0}, // E G (simplified)
            {'S', 0xDA}, // A C D F G (same as 5)
            {'T', 0xF0}, // D E F G
            {'U', 0x7C}, // B C D E F
            {'Y', 0xDC}, // B C D F G
            {'Z', 0xB6}, // A B D E G (same as 2)
            
            // Lowercase letters
            {'a', 0xBE}, // A B C D E G
            {'b', 0xF8}, // C D E F G
            {'c', 0xB0}, // D E G
            {'d', 0xBC}, // B C D E G
            {'e', 0xF2}, // A D E F G (same as E)
            {'f', 0xE2}, // A E F G (same as F)
            {'h', 0xE8}, // C E F G
            {'i', 0x08}, // C
            {'j', 0x3C}, // B C D E (same as J)
            {'l', 0x0C}, // B C (same as I/1)
            {'n', 0xA8}, // C E G
            {'o', 0xB8}, // C D E G
            {'p', 0xE6}, // A B E F G (same as P)
            {'q', 0xCE}, // A B C F G (same as Q)
            {'r', 0xA0}, // E G
            {'t', 0xF0}, // D E F G (same as T)
            {'u', 0x38}, // C D E
            {'y', 0xDC}, // B C D F G (same as Y)
            
            // Special characters
            {' ', 0x00}, // Blank
            {'-', 0x80}, // G only (minus sign)
            {'_', 0x10}, // D only (underscore)
            {'=', 0x90}, // D G (equals)
            {'[', 0x72}, // A D E F
            {']', 0x1E}, // A B C D
            {'"', 0x44}, // B F (quote marks)
            {'\'', 0x40}, // F (apostrophe)
            {'°', 0xC6}, // A B F G (degree symbol)
        };

        // Segment bit mapping for 7-segment displays
        // These map to the bits in `_DigitValues`.
        //
        // IMPORTANT: The PAP-3 digit byte encoding uses a non-standard segment order that matches
        // the segment offsets used in the digit mappings (7 offsets per digit):
        //   G (middle), F (top left), E (bottom left), D (bottom), C (bottom right), B (top right), A (top)
        // Decimal point is bit 0.
        static readonly byte[] _SegmentBits = new byte[] {
            0x80, // Bit 7: Segment G (middle)
            0x40, // Bit 6: Segment F (top left)
            0x20, // Bit 5: Segment E (bottom left)
            0x10, // Bit 4: Segment D (bottom)
            0x08, // Bit 3: Segment C (bottom right)
            0x04, // Bit 2: Segment B (top right)
            0x02, // Bit 1: Segment A (top)
            0x01  // Bit 0: Decimal point
        };


        // Display mapping structure: defines which bit mask and offsets to use for each digit
        class DigitMapping
        {
            public byte BitMask { get; set; }
            public int[] SegmentOffsets { get; set; }
        }

        // PLT Course - 3 digits
        static readonly DigitMapping[] _PltCourseMapping = new DigitMapping[]
        {
            new DigitMapping { BitMask = 0x80, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }, // Hundreds
            new DigitMapping { BitMask = 0x40, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }, // Tens
            new DigitMapping { BitMask = 0x20, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }  // Ones
        };

        // CPL Course - 3 digits
        static readonly DigitMapping[] _CplCourseMapping = new DigitMapping[]
        {
            new DigitMapping { BitMask = 0x40, SegmentOffsets = new int[] { 0x20, 0x24, 0x28, 0x2C, 0x30, 0x34, 0x38 } }, // Hundreds
            new DigitMapping { BitMask = 0x20, SegmentOffsets = new int[] { 0x20, 0x24, 0x28, 0x2C, 0x30, 0x34, 0x38 } }, // Tens
            new DigitMapping { BitMask = 0x10, SegmentOffsets = new int[] { 0x20, 0x24, 0x28, 0x2C, 0x30, 0x34, 0x38 } }  // Ones
        };

        // Speed - 4 digits physical window
        static readonly DigitMapping[] _Speed4Mapping = new DigitMapping[]
        {
            new DigitMapping { BitMask = 0x08, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }, // Digit 1 (leftmost)
            new DigitMapping { BitMask = 0x04, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }, // Digit 2
            new DigitMapping { BitMask = 0x02, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }, // Digit 3
            new DigitMapping { BitMask = 0x01, SegmentOffsets = new int[] { 0x1D, 0x21, 0x25, 0x29, 0x2D, 0x31, 0x35 } }  // Digit 4 (rightmost)
        };

        // Convenience mapping to address digits 2-4 (rightmost three)
        static readonly DigitMapping[] _Speed3RightAlignedMapping = new DigitMapping[]
        {
            _Speed4Mapping[1],
            _Speed4Mapping[2],
            _Speed4Mapping[3]
        };

        // Heading (HDG) - 3 digits
        static readonly DigitMapping[] _HeadingMapping = new DigitMapping[]
        {
            new DigitMapping { BitMask = 0x40, SegmentOffsets = new int[] { 0x1E, 0x22, 0x26, 0x2A, 0x2E, 0x32, 0x36 } }, // Hundreds
            new DigitMapping { BitMask = 0x20, SegmentOffsets = new int[] { 0x1E, 0x22, 0x26, 0x2A, 0x2E, 0x32, 0x36 } }, // Tens
            new DigitMapping { BitMask = 0x10, SegmentOffsets = new int[] { 0x1E, 0x22, 0x26, 0x2A, 0x2E, 0x32, 0x36 } }  // Ones
        };

        // Altitude - 5 digits 
        static readonly DigitMapping[] _AltitudeMapping = new DigitMapping[]
        {
            new DigitMapping { BitMask = 0x04, SegmentOffsets = new int[] { 0x1E, 0x22, 0x26, 0x2A, 0x2E, 0x32, 0x36 } }, // Ten-thousands
            new DigitMapping { BitMask = 0x02, SegmentOffsets = new int[] { 0x1E, 0x22, 0x26, 0x2A, 0x2E, 0x32, 0x36 } }, // Thousands
            new DigitMapping { BitMask = 0x01, SegmentOffsets = new int[] { 0x1E, 0x22, 0x26, 0x2A, 0x2E, 0x32, 0x36 } }, // Hundreds
            new DigitMapping { BitMask = 0x80, SegmentOffsets = new int[] { 0x1F, 0x23, 0x27, 0x2B, 0x2F, 0x33, 0x37 } }, // Tens
            new DigitMapping { BitMask = 0x40, SegmentOffsets = new int[] { 0x1F, 0x23, 0x27, 0x2B, 0x2F, 0x33, 0x37 } }  // Ones
        };

        // Vertical Speed - 4 digits
        static readonly DigitMapping[] _VerticalSpeedMapping = new DigitMapping[]
        {
            new DigitMapping { BitMask = 0x08, SegmentOffsets = new int[] { 0x1F, 0x23, 0x27, 0x2B, 0x2F, 0x33, 0x37 } }, // Thousands
            new DigitMapping { BitMask = 0x04, SegmentOffsets = new int[] { 0x1F, 0x23, 0x27, 0x2B, 0x2F, 0x33, 0x37 } }, // Hundreds
            new DigitMapping { BitMask = 0x02, SegmentOffsets = new int[] { 0x1F, 0x23, 0x27, 0x2B, 0x2F, 0x33, 0x37 } }, // Tens
            new DigitMapping { BitMask = 0x01, SegmentOffsets = new int[] { 0x1F, 0x23, 0x27, 0x2B, 0x2F, 0x33, 0x37 } }  // Ones
        };

        ushort _SequenceNumber = 0;
        bool _LastMagneticState = false;

        /// <summary>
        /// Gets the native value from the device's left ambient light sensor.
        /// </summary>
        internal int LeftAmbientLightNative { get; private set; }

        /// <summary>
        /// Gets the native value from the device's right ambient light sensor.
        /// </summary>
        internal int RightAmbientLightNative { get; private set; }

        /// <summary>
        /// Gets a normalized ambient light value calculated from left and right sensors,
        /// where 0 is completely dark and 100 is completely illuminated.
        /// </summary>
        public int AmbientLightPercent { get; private set; }

        /// <summary>
        /// Raised when the normalized ambient light percentage changes.
        /// </summary>
        public event EventHandler AmbientLightChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pap3Device"/> class.
        /// </summary>
        /// <param name="hidDevice">The HID device to communicate with.</param>
        /// <param name="deviceId">The device identifier.</param>
        public Pap3Device(HidDevice hidDevice, DeviceIdentifier deviceId)
            : base(hidDevice, deviceId)
        {
        }

        /// <inheritdoc/>
        protected override void SendInitPacket()
        {
            // Initialization packet: F0 02 00... (all zeros)
            var initPacket = new byte[64];
            initPacket[0] = 0xF0;
            initPacket[1] = 0x02;
            // Rest is already 0x00
            
            SendCommand(initPacket);
        }

        /// <inheritdoc/>
        protected override Control? GetControl(int offset, byte flag)
        {
            foreach (Control control in Enum.GetValues(typeof(Control))) {
                var (mapFlag, mapOffset) = ControlMap.InputReport01FlagAndOffset(control);
                if(mapOffset == offset && mapFlag == flag) {
                    return control;
                }
            }
            return null;
        }

        /// <inheritdoc/>
        protected override void ProcessReport(byte[] data, int length)
        {
            if(length < 25)
                return;

            // Process ambient light sensor data (offsets 17-18 for left, 19-20 for right)
            var leftSensor = (ushort)(data[17] | (data[18] << 8));
            var rightSensor = (ushort)(data[19] | (data[20] << 8));

            var leftChanged = leftSensor != LeftAmbientLightNative;
            var rightChanged = rightSensor != RightAmbientLightNative;

            if(leftChanged || rightChanged) {
                LeftAmbientLightNative = leftSensor;
                RightAmbientLightNative = rightSensor;

                // Calculate normalized percentage (0-100)
                var avg = ((double)LeftAmbientLightNative + (double)RightAmbientLightNative) / 2.0;
                avg /= 0xfff; // Normalize to 0-1
                var newPercent = (int)(100.0 * avg);
                var percentChanged = newPercent != AmbientLightPercent;
                AmbientLightPercent = newPercent;

                // Raise events
                if(percentChanged) {
                    AmbientLightChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            // Call base implementation for standard control processing
            base.ProcessReport(data, length);
        }

        /// <inheritdoc/>
        public override void UpdateDisplay(IFrontpanelState state)
        {
            if(!IsConnected)
                return;

            if(state is Pap3State pap3State) {
                // Update displays only - solenoid managed separately via UpdateSolenoid()
                var commands = BuildDisplayCommands(pap3State);
                foreach(var data in commands) {
                    SendCommand(data);
                }
            }
        }

        /// <summary>
        /// Updates the solenoid state based on the provided state.
        /// Only sends command if the magnetic state has changed to reduce USB traffic.
        /// Call this method explicitly when you want to sync the solenoid with your state.
        /// </summary>
        /// <param name="state">The frontpanel state containing MagneticActivated property.</param>
        public void UpdateSolenoid(Pap3State state)
        {
            if(!IsConnected)
                return;

            if(state.MagneticActivated != _LastMagneticState) {
                if(state.MagneticActivated) {
                    SendCommand(BuildEngageSolenoidCommand());
                } else {
                    SendCommand(BuildReleaseSolenoidCommand());
                }
                _LastMagneticState = state.MagneticActivated;
            }
            
        }

        /// <inheritdoc/>
        public override void UpdateLeds(IFrontpanelLeds leds)
        {
            if(!IsConnected)
                return;

            if(leds is Pap3Leds pap3Leds) {
                var commands = BuildLedCommands(pap3Leds);
                foreach(var data in commands) {
                    SendCommand(data);
                }
            }
        }

        /// <inheritdoc/>
        public override void SetBrightness(byte panelBacklight, byte lcdBacklight, byte ledBacklight)
        {
            if(!IsConnected)
                return;

            SendBrightnessCommand(_Pap3LedPrefix, _BrightnessPanelBacklight, panelBacklight);
            SendBrightnessCommand(_Pap3LedPrefix, _BrightnessDigitalTube, lcdBacklight);
            SendBrightnessCommand(_Pap3LedPrefix, _BrightnessMarkerLight, ledBacklight);
        }

        List<byte[]> BuildDisplayCommands(Pap3State state)
        {
            var commands = new List<byte[]>();

            commands.AddRange(BuildPap3DisplayCommands(state));
            return commands;
        }

        List<byte[]> BuildPap3DisplayCommands(Pap3State state)
        {
            var commands = new List<byte[]>();
            var payload = new byte[64];
            var followup = new byte[64];

            // Increment sequence number for each display update
            _SequenceNumber++;
            if(_SequenceNumber > 255) _SequenceNumber = 1;

            // Packet 1: 38 command with display data to unit 0F BF
            // Packet 2: 38 command empty (to unit 00 00 - no device)
            // Packet 3: 38 command empty (to unit 00 00 - no device)
            // Packet 4: 2A acknowledgment packet
                        
            // Packet 1: Main display data (38 command)
            payload[0] = 0xF0;
            payload[1] = 0x00;
            payload[2] = (byte)_SequenceNumber;
            payload[3] = 0x38; // Command to device ?
            payload[4] = (byte)((_Pap3DisplayPrefix >> 8) & 0xFF); // 0x0F
            payload[5] = (byte)(_Pap3DisplayPrefix & 0xFF);        // 0xBF
            payload[6] = 0x00; // Checksum bytes (set to 00 00 for now)
            payload[7] = 0x00;
            
            // Fixed sequence from hardware capture (bytes 08-1E)
            payload[8] = 0x02;
            payload[9] = 0x01;
            payload[10] = 0x00;
            payload[11] = 0x00;
            // Bytes 12-15 appear to be a checksum/identifier that gets echoed in 2A packet
            // For now, use captured values
            payload[12] = 0xC3;
            payload[13] = 0x29;
            payload[14] = 0x20;
            payload[15] = 0x00;
            payload[16] = 0x00;
            payload[17] = 0xB0;
            
                        
            EncodePap3Displays(payload, state);

            // Speed mode indicators (IAS vs MACH)
            if (!string.IsNullOrEmpty(state.SpeedDisplay))
            {
                if (state.SpeedIsMach)
                {
                    payload[0x2E] |= 0x80;
                    payload[0x32] |= 0x80;  
                    payload[0x19] |= 0x04;
                }
                else
                {
                    payload[0x36] |= 0x80; 
                    payload[0x1A] |= 0x80;  
                }
            }

            // Heading mode indicators (HDG vs TRK)
            if (!string.IsNullOrEmpty(state.HeadingDisplay))
            {
                if (state.HeadingIsTrack)
                {
                    payload[0x2A] |= 0x08;  
                    payload[0x2E] |= 0x08;  
                }
                else
                {
                    payload[0x32] |= 0x08;  
                    payload[0x36] |= 0x08;  
                }
            }

            // Vertical Speed indicators
            if (!string.IsNullOrEmpty(state.VerticalSpeedDisplay))
            {
                payload[0x1F] |= 0x10;
                
                // Show climb/descent indicator
                if (state.VerticalSpeedPositive)
                {
                    payload[0x23] |= 0x10;
                    payload[0x2C] |= 0x80;
                    payload[0x28] |= 0x80;
                }

                // V/S vs FPA mode
                if (state.VsIsFpa)
                {
                    payload[0x30] |= 0x80;  
                    payload[0x34] |= 0x80;
                    payload[0x1B] |= 0x04;
                }
                else
                {
                    payload[0x1C] |= 0x80;  
                    payload[0x38] |= 0x80;  
                }
                
            }

            commands.Add(payload);

            // Packet 2: Empty 38 packet (to unit 00 00)
            var empty2 = new byte[64];
            empty2[0] = 0xF0;
            empty2[1] = 0x00;
            empty2[2] = (byte)((_SequenceNumber + 1) % 256);
            empty2[3] = 0x38;
            empty2[4] = 0x00; // Unit ID 00 00 (no device)
            empty2[5] = 0x00;
            commands.Add(empty2);

            // Packet 3: Empty 38 packet (to unit 00 00)
            var empty3 = new byte[64];
            empty3[0] = 0xF0;
            empty3[1] = 0x00;
            empty3[2] = (byte)((_SequenceNumber + 2) % 256);
            empty3[3] = 0x38;
            empty3[4] = 0x00; // Unit ID 00 00 (no device)
            empty3[5] = 0x00;
            commands.Add(empty3);

            // Packet 2: Acknowledgment (2A packet)
            followup[0] = 0xF0;
            followup[1] = 0x00;
            followup[2] = (byte)((_SequenceNumber + 3) % 256);
            followup[3] = 0x2A; // Acknowledgment command
            // Bytes 04-1C are zeros
            followup[29] = (byte)((_Pap3DisplayPrefix >> 8) & 0xFF); // 0x0F (Unit ID being acknowledged)
            followup[30] = (byte)(_Pap3DisplayPrefix & 0xFF);        // 0xBF
            followup[31] = 0x00;
            followup[32] = 0x00;
            followup[33] = 0x03;
            followup[34] = 0x01;
            followup[35] = 0x00;
            followup[36] = 0x00;
            // Bytes 37-40 echo the checksum from the 38 packet (bytes 12-15)
            followup[37] = payload[12]; // 0xC3
            followup[38] = payload[13]; // 0x29
            followup[39] = payload[14]; // 0x20
            followup[40] = 0x00;
            // Remaining bytes are zeros

            commands.Add(followup);

            // Update sequence number to account for all 4 packets
            _SequenceNumber = (ushort)((_SequenceNumber + 4) % 256);

            return commands;
        }

        void EncodePap3Displays(byte[] buffer, Pap3State state)
        {
            // Clear the display area first to avoid stale data
            for (int i = 0x19; i <= 0x38; i++)
            {
                buffer[i] = 0x00;
            }

            // Encode Speed display (3-4 characters depending on mode)
            if (!string.IsNullOrEmpty(state.SpeedDisplay)) {
                if (state.SpeedIsMach) {
                    // MACH mode: use rightmost 3 digits
                    EncodeStringWithMapping(buffer, state.SpeedDisplay, _Speed3RightAlignedMapping, 3);
                } else {
                    // IAS mode: up to 4 digits, but use 3-digit if value fits
                    if (state.SpeedDisplay.Length <= 3) {
                        EncodeStringWithMapping(buffer, state.SpeedDisplay, _Speed3RightAlignedMapping, 3);
                    } else {
                        EncodeStringWithMapping(buffer, state.SpeedDisplay, _Speed4Mapping, 4);
                    }
                }
            }
            
            // Encode Pilot Course display (3 characters)
            if (!string.IsNullOrEmpty(state.PltCourse)) {
                EncodeStringWithMapping(buffer, state.PltCourse, _PltCourseMapping, 3);
            }

            // Encode Copilot Course display (3 characters)
            if (!string.IsNullOrEmpty(state.CplCourse)) {
                EncodeStringWithMapping(buffer, state.CplCourse, _CplCourseMapping, 3);
            }

            // Encode Heading display (3 characters)
            if (!string.IsNullOrEmpty(state.HeadingDisplay)) {
                EncodeStringWithMapping(buffer, state.HeadingDisplay, _HeadingMapping, 3);
            }

            // Encode Altitude display (5 characters)
            if (!string.IsNullOrEmpty(state.AltitudeDisplay)) {
                EncodeStringWithMapping(buffer, state.AltitudeDisplay, _AltitudeMapping, 5);
            }

            // Encode Vertical Speed display (4 characters)
            if (!string.IsNullOrEmpty(state.VerticalSpeedDisplay)) {
                EncodeStringWithMapping(buffer, state.VerticalSpeedDisplay, _VerticalSpeedMapping, 4);
            }
        }

        void EncodeMultiDigitValue(byte[] buffer, int value, int numDigits, DigitMapping[] mapping)
        {
            var valueStr = value.ToString().PadLeft(numDigits, '0');
            
            for (int i = 0; i < numDigits && i < mapping.Length; i++)
            {
                int digit = valueStr[i] - '0';
                EncodeDigitWithMapping(buffer, digit, mapping[i]);
            }
        }

        void EncodeDigitWithMapping(byte[] buffer, int digit, DigitMapping mapping)
        {
            if (digit < 0 || digit > 9)
                return;

            var segmentValue = _DigitValues[digit];
            
            // For each segment bit in the 7-segment encoding
            for (int segIdx = 0; segIdx < _SegmentBits.Length && segIdx < mapping.SegmentOffsets.Length; segIdx++)
            {
                // Check if this segment should be lit for this digit
                if ((segmentValue & _SegmentBits[segIdx]) != 0)
                {
                    // Set the bit at the corresponding offset
                    buffer[mapping.SegmentOffsets[segIdx]] |= mapping.BitMask;
                }
            }
        }

        void EncodeCharWithMapping(byte[] buffer, char character, DigitMapping mapping)
        {
            // Try to get the 7-segment pattern for this character
            if (!_CharacterValues.TryGetValue(character, out var segmentValue))
            {
                // Character not found, try uppercase conversion
                var upperChar = char.ToUpper(character);
                if (!_CharacterValues.TryGetValue(upperChar, out segmentValue))
                {
                    // Still not found, display as blank
                    segmentValue = 0x00;
                }
            }

            // Apply the segment pattern to the display buffer
            for (int segIdx = 0; segIdx < _SegmentBits.Length && segIdx < mapping.SegmentOffsets.Length; segIdx++)
            {
                if ((segmentValue & _SegmentBits[segIdx]) != 0)
                {
                    buffer[mapping.SegmentOffsets[segIdx]] |= mapping.BitMask;
                }
            }
        }

        void EncodeStringWithMapping(byte[] buffer, string value, DigitMapping[] mapping, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return;

            // Pad or truncate to fit the display
            var displayValue = value.Length > maxLength 
                ? value.Substring(0, maxLength) 
                : value.PadLeft(maxLength, ' ');

            for (int i = 0; i < displayValue.Length && i < mapping.Length; i++)
            {
                EncodeCharWithMapping(buffer, displayValue[i], mapping[i]);
            }
        }
                
        List<byte[]> BuildLedCommands(Pap3Leds leds)
        {
            var commands = new List<byte[]>();

            // LED command codes verified from hardware testing
            // Format: 02 01 00 00 00 03 49 [code] [value] 00 00 00 00 00
            // LED commands use prefix 0x0100
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x03, leds.N1));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x04, leds.Speed));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x05, leds.Vnav));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x06, leds.LvlChg));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x07, leds.HdgSel));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x08, leds.Lnav));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x09, leds.VorLoc));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x0A, leds.App));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x0B, leds.AltHold));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x0C, leds.Vs));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x0D, leds.CmdA));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x0E, leds.CwsA));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x0F, leds.CmdB));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x10, leds.CwsB));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x11, leds.AtArm));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x12, leds.FdL));
            commands.Add(BuildLedCommand(_Pap3LedPrefix, 0x13, leds.FdR));

            return commands;
        }

        byte[] BuildLedCommand(ushort prefix, byte ledCode, bool on)
        {
            var packet = new byte[14];
            packet[0] = 0x02;
            packet[1] = (byte)((prefix >> 8) & 0xFF);
            packet[2] = (byte)(prefix & 0xFF);
            packet[3] = 0x00;
            packet[4] = 0x00;
            packet[5] = 0x03;
            packet[6] = 0x49;
            packet[7] = ledCode;
            packet[8] = (byte)(on ? 0x01 : 0x00);

            return packet;
        }

        /// <summary>
        /// Engages (arms) the auto-throttle solenoid, locking the A/T ARM switch in position.
        /// This prevents the switch from being moved until released.
        /// Based on captured USB traffic: sends value 0x01.
        /// </summary>
        public void EngageSolenoid()
        {
            if(!IsConnected)
                return;

            SendCommand(BuildEngageSolenoidCommand());
            _LastMagneticState = true;
        }

        /// <summary>
        /// Releases (disarms) the auto-throttle solenoid, allowing the A/T ARM switch to be moved freely.
        /// The solenoid will remain released until explicitly re-engaged with EngageSolenoid().
        /// Based on captured USB traffic: sends value 0x00.
        /// </summary>
        public void ReleaseSolenoid()
        {
            if(!IsConnected)
                return;

            SendCommand(BuildReleaseSolenoidCommand());
            _LastMagneticState = false;
        }

        /// <summary>
        /// Triggers a full solenoid cycle for testing.
        /// This performs: release (disarm), wait 50ms, then engage (arm).
        /// Useful for testing that the solenoid mechanism works correctly.
        /// </summary>
        public void TriggerSolenoid()
        {
            if(!IsConnected)
                return;

            // Release first (allow knob to pop up)
            SendCommand(BuildReleaseSolenoidCommand());
            
            // Small delay
            System.Threading.Thread.Sleep(50);
            
            // Then engage (lock knob down)
            SendCommand(BuildEngageSolenoidCommand());
            _LastMagneticState = true;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        byte[] BuildEngageSolenoidCommand()
        {
            // Engage (arm) solenoid command uses display prefix (0x0FBF)
            // Captured behavior: This LOCKS the knob down (arms the solenoid)
            // Packet: 02 0f bf 00 00 03 49 1e 01 00 00 00 00 00
            var packet = new byte[14];
            packet[0] = 0x02;
            packet[1] = 0x0F;  // Display prefix high byte
            packet[2] = 0xBF;  // Display prefix low byte
            packet[3] = 0x00;
            packet[4] = 0x00;
            packet[5] = 0x03;
            packet[6] = 0x49;
            packet[7] = 0x1E;  // Solenoid command code
            packet[8] = 0x01;  // Engage (arm) value - locks knob down
            // Remaining bytes 9-13 are 0x00
            return packet;
        }

        byte[] BuildReleaseSolenoidCommand()
        {
            // Release (disarm) solenoid command
            // Captured behavior: This RELEASES the knob (allows it to pop up)
            // Packet: 02 0f bf 00 00 03 49 1e 00 00 00 00 00 00
            var packet = new byte[14];
            packet[0] = 0x02;
            packet[1] = 0x0F;  // Display prefix high byte
            packet[2] = 0xBF;  // Display prefix low byte
            packet[3] = 0x00;
            packet[4] = 0x00;
            packet[5] = 0x03;
            packet[6] = 0x49;
            packet[7] = 0x1E;  // Solenoid command code
            packet[8] = 0x00;  // Release (disarm) value - allows knob to pop up
            // Remaining bytes 9-13 are 0x00
            return packet;
        }
    }
}
