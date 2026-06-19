// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet.Winctrl
{
    /// <summary>
    /// Holds the content of an input report sent by a Winctrl panel.
    /// </summary>
    class InputReport
    {
        public const int ReportCode = 1;
        public const int PacketLength = 25;
        private const int _LegacyLeftAmbientLightSensorOffset = 17;
        private const int _LegacyRightAmbientLightSensorOffset = 19;
        private const int _AxisLeftAmbientLightSensorOffset = 16;
        private const int _AxisRightAmbientLightSensorOffset = 18;
        private const int _AxisOutputSwitchMarkerOffset = 20;
        private const byte _AxisOutputSwitchMarkerLow = 0xFE;
        private const byte _AxisOutputSwitchMarkerHigh = 0xFF;

        private readonly byte[] _Packet = new byte[PacketLength];

        public void CopyFrom(byte[] buffer, int offset, int length)
        {
            if(length > 0) {
                if(buffer[offset] != ReportCode) {
                    throw new WwDeviceException($"Unexpected report code {buffer[offset]} for a report code 1 buffer");
                }
                length = Math.Min(PacketLength, length);
                Array.ConstrainedCopy(buffer, offset, _Packet, 0, length);
                for(var idx = length;idx < _Packet.Length;++idx) {
                    _Packet[idx] = 0;
                }
            }
        }

        public void CopyFrom(InputReport other) => CopyFrom(other._Packet, 0, other._Packet.Length);

        public (UInt64, UInt64, UInt64) ToDigest()
        {
            return (
                BitConverter.ToUInt64(_Packet, 1),
                BitConverter.ToUInt64(_Packet, 9),
                BitConverter.ToUInt64(_Packet, 17)
            );
        }

        public bool IsKeyPressed(int flag, int offset) => (_Packet[offset] & flag) != 0;

        public (UInt16 LeftSensor, UInt16 RightSensor) AmbientLightSensorValues()
        {
            var useAxisOffsets = _Packet[_AxisOutputSwitchMarkerOffset] == _AxisOutputSwitchMarkerLow
                && _Packet[_AxisOutputSwitchMarkerOffset + 1] == _AxisOutputSwitchMarkerHigh;

            var leftOffset = useAxisOffsets ? _AxisLeftAmbientLightSensorOffset : _LegacyLeftAmbientLightSensorOffset;
            var rightOffset = useAxisOffsets ? _AxisRightAmbientLightSensorOffset : _LegacyRightAmbientLightSensorOffset;

            var leftSensorValue = ReadSensorValue(leftOffset);
            var rightSensorValue = ReadSensorValue(rightOffset);
            return (leftSensorValue, rightSensorValue);
        }

        private UInt16 ReadSensorValue(int offset)
        {
            UInt16 result = _Packet[offset];
            result |= (UInt16)(_Packet[offset + 1] << 8);
            return result;
        }
    }
}
