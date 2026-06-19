// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WwDevicesDotNet.Winctrl
{
    /// <summary>
    /// Manages the sending of an entire screen buffer to the device.
    /// </summary>
    class ScreenWriter
    {
        private UsbWriter _UsbWriter;
        private DisplayBuffer _DisplayBuffer;
        private readonly int _ProcessingPauseMilliseconds = 40;
        private readonly byte[] _DisplayPacket = new byte[64];
        private readonly char[] _DisplayCharacterBuffer = new char[1];
        private int _DisplayPacketOffset = 0;

        public Action<DisplayChangingEventArgs> UpdatingDeviceCallback { get; set; }

        public ScreenWriter(UsbWriter usbWriter)
        {
            _UsbWriter = usbWriter;
            _DisplayPacket[0] = 0xF2;
        }

        /// <summary>
        /// Sends a screen buffer to the device.
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="skipDuplicateCheck"></param>
        /// <param name="suppressUpdatingDeviceCallback"></param>
        public void SendScreenToDisplay(
            Screen screen,
            bool skipDuplicateCheck,
            bool suppressUpdatingDeviceCallback
        )
        {
            _UsbWriter.LockForOutput(() => {
                if(_DisplayBuffer == null) {
                    _DisplayBuffer = new DisplayBuffer(screen.Rows.Length, screen.Rows[0].Cells.Length);
                }
                var hasChanged = _DisplayBuffer.CopyFrom(screen);
                if(skipDuplicateCheck || hasChanged) {
                    if(UpdatingDeviceCallback != null && !suppressUpdatingDeviceCallback) {
                        var args = new DisplayChangingEventArgs(_DisplayBuffer.Clone());
                        Task.Run(() => UpdatingDeviceCallback?.Invoke(args));
                    }

                    InitialiseDisplayPacket();
                    for(var rowIdx = 0;rowIdx < _DisplayBuffer.CountRows;++rowIdx) {
                        for(var cellIdx = 0;cellIdx < _DisplayBuffer.CountCells;++cellIdx) {
                            AddCellToDisplayPacket(
                                _DisplayBuffer.Characters[rowIdx, cellIdx],
                                _DisplayBuffer.FontsAndColours[rowIdx, cellIdx],
                                rowIdx == 0 && cellIdx == 0,
                                rowIdx == _DisplayBuffer.CountRows - 1 && cellIdx == _DisplayBuffer.CountCells - 1
                            );
                        }
                    }
                    PadAndSendDisplayPacket();

                    if(_ProcessingPauseMilliseconds > 0) {
                        // If we send packets too quickly then the device can freak out.
                        // This forces a pause so that a set of very fast updates won't
                        // corrupt the display.
                        Thread.Sleep(_ProcessingPauseMilliseconds);
                    }
                }
            });
        }

        private void InitialiseDisplayPacket()
        {
            _DisplayPacketOffset = 1;
        }

        private void AddCellToDisplayPacket(
            char character,
            DisplayBufferFontAndColour fontAndColour,
            bool isFirstCell,
            bool isLastCell
        )
        {
            _DisplayCharacterBuffer[0] = character;
            var utf8Bytes = Encoding.UTF8.GetBytes(_DisplayCharacterBuffer);
            AddColourIndexAndBytesToDisplayPacket(
                fontAndColour,
                isFirstCell,
                isLastCell
            );
            for(var chIdx = 0;chIdx < utf8Bytes.Length;++chIdx) {
                AddToDisplayPacketSendWhenFull(utf8Bytes[chIdx]);
            }
        }

        private void AddColourIndexAndBytesToDisplayPacket(
            DisplayBufferFontAndColour fontAndColour,
            bool isFirstCell,
            bool isLastCell
        )
        {
            (var b1, var b2) = fontAndColour.ToWinctrlUsbColourAndFontCode(isFirstCell, isLastCell);
            AddToDisplayPacketSendWhenFull(b1);
            AddToDisplayPacketSendWhenFull(b2);
        }

        private void AddToDisplayPacketSendWhenFull(byte value)
        {
            _DisplayPacket[_DisplayPacketOffset++] = value;
            if(_DisplayPacketOffset == _DisplayPacket.Length) {
                _UsbWriter.SendPacket(_DisplayPacket);
                InitialiseDisplayPacket();
            }
        }

        private void PadAndSendDisplayPacket()
        {
            if(_DisplayPacketOffset > 1) {
                for(var idx = _DisplayPacketOffset;idx < _DisplayPacket.Length;++idx) {
                    _DisplayPacket[idx] = 0;
                }
                _UsbWriter.SendPacket(_DisplayPacket);
                InitialiseDisplayPacket();
            }
        }
    }
}
