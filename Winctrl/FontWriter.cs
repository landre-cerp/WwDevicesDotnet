// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WwDevicesDotNet;

namespace WwDevicesDotNet.Winctrl
{
    /// <summary>
    /// Handles the sending of font glyphs to the Winctrl panels.
    /// </summary>
    class FontWriter
    {
        private UsbWriter _UsbWriter;
        private DisplayFont _DisplayFont = new DisplayFont();

        public Action<FontChangingEventArgs> UpdatingDeviceCallback { get; set; }

        public FontWriter(UsbWriter usbWriter)
        {
            _UsbWriter = usbWriter;
        }

        public bool SendFont(
            McduFontFile fontFileContent,
            string commandPrefix,
            bool useFullWidth,
            Action clearScreenAction,
            int currentDisplayBrightnessPercent,
            int currentDisplayXOffset,
            int currentDisplayYOffset,
            bool skipDuplicateCheck,
            bool suppressUpdatingDeviceCallback = false
        )
        {
            var fontUploaded = false;

            _UsbWriter.LockForOutput(() => {
                var hasChanged = _DisplayFont.CopyFrom(fontFileContent, useFullWidth);
                if(skipDuplicateCheck || hasChanged) {
                    clearScreenAction();

                    var xOffset = 0x24 + currentDisplayXOffset + XOffsetForGlyphWidth(_DisplayFont.PixelWidth);
                    var yOffset = 0x14 + currentDisplayYOffset + YOffsetForGlyphHeight(_DisplayFont.PixelHeight);

                    if(UpdatingDeviceCallback != null && !suppressUpdatingDeviceCallback) {
                        var clone = _DisplayFont.Clone();
                        var args = new FontChangingEventArgs(_DisplayFont.Clone(), xOffset, yOffset);
                        Task.Run(() => UpdatingDeviceCallback?.Invoke(args));
                    }

                    byte[] mapBytes;
                    switch(_DisplayFont.PixelHeight) {
                        case 29:    mapBytes = CduResources.WinctrlFontPacketMap_3x29_json; break;
                        case 30:    mapBytes = CduResources.WinctrlFontPacketMap_3x30_json; break;
                        case 31:    mapBytes = CduResources.WinctrlFontPacketMap_3x31_json; break;
                        case 32:    mapBytes = CduResources.WinctrlFontPacketMap_3x32_json; break;
                        default:    throw new NotImplementedException($"Need packet map for {_DisplayFont.PixelHeight} pixel high fonts");
                    }

                    var mapJson = Encoding.UTF8.GetString(mapBytes);
                    var packetMap = JsonConvert.DeserializeObject<McduFontPacketMap>(mapJson);
                    packetMap.OverwritePacketsWithFontFileContent(
                        commandPrefix,
                        Percent.ToByte(currentDisplayBrightnessPercent),
                        _DisplayFont.PixelWidth,
                        _DisplayFont.PixelHeight,
                        xOffset,
                        yOffset,
                        _DisplayFont?.LargeGlyphs,
                        _DisplayFont?.SmallGlyphs
                    );
                    foreach(var packet in packetMap.Packets) {
                        _UsbWriter.SendStringPacket(packet);
                    }

                    fontUploaded = true;
                }
            });

            return fontUploaded;
        }

        private static int XOffsetForGlyphWidth(int glyphWidth)
        {
            var excess = Metrics.DisplayWidthPixels - (glyphWidth * Metrics.Columns);
            return excess / 2;
        }

        private static int YOffsetForGlyphHeight(int glyphHeight)
        {
            switch(glyphHeight) {
                case 29:    return 17;
                case 30:    return 4;
                case 31:    return 0;
                case 32:    return 0;
                default:    throw new NotImplementedException($"Need base YOffset for {glyphHeight} glyphHeight");
            }
        }
    }
}
