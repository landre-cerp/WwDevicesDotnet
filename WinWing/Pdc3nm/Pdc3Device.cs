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

namespace WwDevicesDotNet.WinWing.Pdc3nm
{
    /// <summary>
    /// Represents a WinWing PDC-3 Navigation Display device.
    /// Handles communication with the physical PDC3 hardware via HID protocol.
    /// </summary>
    public class Pdc3Device : BaseFrontpanelDevice<Control>
    {
        // Command prefix for PDC-3 panel (verified from hardware testing)
        const ushort _Pdc3LedPrefix = 0x60BB;


        // Brightness command types (verified from hardware testing)
        const byte _BrightnessPanelBacklight = 0x00;


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
        /// Initializes a new instance of the <see cref="Pdc3Device"/> class.
        /// </summary>
        /// <param name="hidDevice">The HID device to communicate with.</param>
        /// <param name="deviceId">The device identifier.</param>
        public Pdc3Device(HidDevice hidDevice, DeviceIdentifier deviceId)
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
        public override void SetBrightness(byte panelBacklight, byte lcdBacklight, byte ledBacklight)
        {
            if(!IsConnected)
                return;

            SendBrightnessCommand(_Pdc3LedPrefix, _BrightnessPanelBacklight, ledBacklight);
        }

       
     
        public override void UpdateDisplay(IFrontpanelState state)
        {

        }

        public override void UpdateLeds(IFrontpanelLeds leds)
        {
            
        }
    }
}
