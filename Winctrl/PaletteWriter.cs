// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Text;
using System.Threading.Tasks;

namespace WwDevicesDotNet.Winctrl
{
    /// <summary>
    /// Manages the sending of colour palettes to the Winctrl MCDU device.
    /// </summary>
    class PaletteWriter
    {
#pragma warning disable IDE1006 // Naming Styles
        private readonly string CP; // See notes elsewhere as to why this name is exactly 2 chars
#pragma warning restore IDE1006 // Naming Styles
        private UsbWriter _UsbWriter;
        private DisplayPalette _DisplayPalette;

        public Action<PaletteChangingEventArgs> UpdatingDeviceCallback { get; set; }

        public PaletteWriter(UsbWriter usbWriter, string commandPrefix)
        {
            _UsbWriter = usbWriter;
            CP = commandPrefix;
        }

        /// <summary>
        /// See the notes against <see cref="McduDevice.UseFont"/> as to why this exists.
        /// It will resend the palette if one has been established, and then in all cases
        /// it refreshes the display.
        /// </summary>
        public void ReestablishPaletteAndRefreshDisplay(
            ScreenWriter screenWriter,
            Screen screen
        )
        {
            if(_DisplayPalette == null) {
                screenWriter.SendScreenToDisplay(
                    screen,
                    skipDuplicateCheck: true,
                    suppressUpdatingDeviceCallback: false
                );
            } else {
                SendPalette(
                    null,
                    screenWriter,
                    screen,
                    skipDuplicateCheck: true,
                    forceDisplayRefresh: true
                );
            }
        }

        public void SendPalette(
            PaletteColour[] colourArray,
            ScreenWriter screenWriter,
            Screen screen,
            bool skipDuplicateCheck = false,
            bool forceDisplayRefresh = true,
            bool suppressUpdatingDeviceCallback = false
        )
        {
            _UsbWriter.LockForOutput(() => {
                if(_DisplayPalette == null) {
                    _DisplayPalette = new DisplayPalette(colourArray.Length);
                }
                var hasChanged = colourArray != null
                    && _DisplayPalette.CopyFrom(colourArray);
                if(skipDuplicateCheck || hasChanged) {
                    if(UpdatingDeviceCallback != null && !suppressUpdatingDeviceCallback) {
                        var args = new PaletteChangingEventArgs(_DisplayPalette.Clone());
                        Task.Run(() => UpdatingDeviceCallback?.Invoke(args));
                    }

                    byte seq = 1;

                    var buffer = new StringBuilder();

                    AddToPacketBuffer(buffer, 0x1f, ref seq, $"{CP}00001901000004170100000e00000001000500000002");
                    SendPacketBuffer(buffer);
                    AddToPacketBuffer(buffer, 0x3c, ref seq, $"{CP}00001901000004170100000e0000000100060000000300000000000000");

                    var colourSeq = 4;
                    foreach(var colour in _DisplayPalette.Colours) {
                        var setForeground = $"{CP}00001901000004170100000e0000000200{colour.WinctrlColourString}{colourSeq++:x2}00000000000000";
                        AddToPacketBuffer(buffer, 0x3c, ref seq, setForeground);
                    }
                    foreach(var colour in _DisplayPalette.Colours) {
                        var setBackground = $"{CP}00001901000004170100000e0000000300{colour.WinctrlColourString}{colourSeq++:x2}00000000000000";
                        AddToPacketBuffer(buffer, 0x3c, ref seq, setBackground);
                    }
                    AddToPacketBuffer(buffer, 0x3c, ref seq, $"{CP}00001901000004170100000e000000040000000000{colourSeq++:x2}00000000000000");
                    AddToPacketBuffer(buffer, 0x3c, ref seq, $"{CP}00001901000004170100000e000000040001000000{colourSeq++:x2}00000000000000");
                    AddToPacketBuffer(buffer, 0x2b, ref seq, $"{CP}00001901000004170100000e000000040002000000{colourSeq++:x2}00000000000000");
                    AddToPacketBuffer(buffer, 0x2b, ref seq, $"{CP}0000050100000417010001");
                    SendPacketBuffer(buffer);
                    AddToPacketBuffer(buffer, 0x34, ref seq, $"{CP}00001a01000025170100000100000002");
                    AddToPacketBuffer(buffer, 0x34, ref seq, $"{CP}00001c010000251701000000000000");
                    AddToPacketBuffer(buffer, 0x34, ref seq, $"{CP}0000050100002517010001");
                    SendPacketBuffer(buffer);

                    if(forceDisplayRefresh) {
                        screenWriter.SendScreenToDisplay(
                            screen,
                            skipDuplicateCheck: true,
                            suppressUpdatingDeviceCallback: false
                        );
                    }
                }
            });
        }

        void SendPacketBuffer(StringBuilder packetBuffer)
        {
            if(packetBuffer.Length > 0) {
                while(packetBuffer.Length < 128) {
                    packetBuffer.Append("00");
                }
                _UsbWriter.SendStringPacket(packetBuffer.ToString());
                packetBuffer.Clear();
            }
        }

        void AddToPacketBuffer(
            StringBuilder packetBuffer,
            int f0Code,
            ref byte sequenceNumber,
            string chunk
        )
        {
            for(var idx = 0;idx < chunk.Length;++idx) {
                if(packetBuffer.Length == 128) {
                    SendPacketBuffer(packetBuffer);
                }
                if(packetBuffer.Length == 0) {
                    packetBuffer.Append("f000");
                    packetBuffer.Append(sequenceNumber++.ToString("x2"));
                    packetBuffer.Append(f0Code.ToString("x2"));
                }
                packetBuffer.Append(chunk[idx]);
            }
        }
    }
}
