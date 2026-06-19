// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using HidSharp;

namespace WwDevicesDotNet.Winctrl
{
    /// <summary>
    /// Reads a Winctrl panel's keyboard and raises events on the parent device when keys
    /// are pressed or released, or when the ambient light sensors change value.
    /// </summary>
    class KeyboardReader : UsbPollingReader
    {
        private readonly Func<Key, (int Flag, int Offset)> _KeyToFlagOffsetCallback;
        private readonly Action<Key, bool> _KeyPressAction;
        private readonly Action<UInt16, UInt16> _AmbientLightChangedAction;
        private readonly InputReport _InputReport_Previous = new InputReport();
        private readonly InputReport _InputReport_Current = new InputReport();
        private (UInt64, UInt64, UInt64) _PreviousInputReportDigest = (0,0,0);

        /// <inheritdoc/>
        protected override int PacketSize => InputReport.PacketLength;

        public KeyboardReader(
            HidStream hidStream,
            Func<Key, (int Flag, int Offset)> keyToFlagOffsetCallback,
            Action<Key, bool> keyPressAction,
            Action<UInt16, UInt16> ambientLightChangedAction
        ) : base(hidStream)
        {
            _KeyToFlagOffsetCallback = keyToFlagOffsetCallback;
            _KeyPressAction = keyPressAction;
            _AmbientLightChangedAction = ambientLightChangedAction;
        }

        protected override void ReportReceived(byte[] readBuffer, int bytesRead)
        {
            _InputReport_Current.CopyFrom(readBuffer, 0, bytesRead);
            var digest = _InputReport_Current.ToDigest();
            if(   digest.Item1 != _PreviousInputReportDigest.Item1
               || digest.Item2 != _PreviousInputReportDigest.Item2
               || digest.Item3 != _PreviousInputReportDigest.Item3
            ) {
                try {
                    foreach(Key key in Enum.GetValues(typeof(Key))) {
                        (var flag, var offset) = _KeyToFlagOffsetCallback(key);
                        if(flag != 0) {
                            var pressed = _InputReport_Current.IsKeyPressed(flag, offset);
                            var wasPressed = _InputReport_Previous.IsKeyPressed(flag, offset);
                            if(pressed != wasPressed) {
                                _KeyPressAction(key, pressed);
                            }
                        }
                    }
                } catch {
                    // Swallow exceptions for now - ultimately we want the events raised on a different thread
                }

                var previousAmbient = _InputReport_Previous.AmbientLightSensorValues();
                var currentAmbient = _InputReport_Current.AmbientLightSensorValues();
                if(previousAmbient != currentAmbient) {
                    _AmbientLightChangedAction(
                        currentAmbient.LeftSensor,
                        currentAmbient.RightSensor
                    );
                }

                _InputReport_Previous.CopyFrom(_InputReport_Current);
                _PreviousInputReportDigest = digest;
            }
        }
    }
}
