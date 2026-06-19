// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.IO;
using System.Text;
using HidSharp;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Manages the sending of packets to a USB device.
    /// </summary>
    class UsbWriter
    {
        private readonly object _OutputLock = new object();

        // The USB writer does not own this object. It is owned by the parent. It is
        // disposable but the parent is responsible for disposing of it.
        private readonly HidStream _HidStream;

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="hidStream"></param>
        public UsbWriter(HidStream hidStream)
        {
            _HidStream = hidStream;
        }

        /// <summary>
        /// Ensures that only one thread writes to the device at once. This is a great
        /// way to deadlock the program! Be careful that the action does not block on
        /// other locks.
        /// </summary>
        /// <param name="action"></param>
        public void LockForOutput(Action action)
        {
            lock(_OutputLock) {
                action();
            }
        }

        /// <summary>
        /// Sends a packet to the device.
        /// </summary>
        /// <param name="bytes"></param>
        public void SendPacket(byte[] bytes)
        {
            var stream = _HidStream;
            try {
                stream?.Write(bytes);
            } catch(IOException) {
                // This can happen when the device is disconnected mid-write
                ;
            }
        }

        /// <summary>
        /// Sends a hex string packet to the device.
        /// </summary>
        /// <param name="packet"></param>
        public void SendStringPacket(string packet)
        {
            var bytes = packet.ToByteArray();
            SendPacket(bytes);
        }

        /// <summary>
        /// As per <see cref="SendStringPacket"/> but this pads the <paramref name="packet"/>
        /// out with zeros until it reaches <paramref name="packetSize"/>.
        /// </summary>
        /// <param name="packetSize"></param>
        /// <param name="packet"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SendStringPacket(int packetSize, string packet)
        {
            if(packet.Length % 2 != 0) {
                throw new InvalidOperationException($"{packet} is not an even length");
            }
            if(packet.Length == packetSize * 2) {
                SendStringPacket(packet);
            } else {
                var buffer = new StringBuilder(packet);
                buffer.Append(packet);
                while(buffer.Length < packet.Length) {
                    buffer.Append("00");
                }
                SendStringPacket(buffer.ToString());
            }
        }
    }
}
