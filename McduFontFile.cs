// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Runtime.Serialization;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Holds the collections of glyphs that together describe a font for a CDU device.
    /// </summary>
    [DataContract]
    public class McduFontFile
    {
        public const string CharacterSet =
            " !\"#$%&'()*+,-./0123456789" +
            ":;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "[\\]^_`abcdefghijklmnopqrstuvwxyz" +
            "{|}~°☐←↑→↓Δ⬡◀▶█▲▼■□";

        /// <summary>
        /// The name of the font.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int GlyphWidth { get; set; }

        [DataMember]
        public int GlyphHeight { get; set; }

        [DataMember]
        public int GlyphFullWidth { get; set; }

        /// <summary>
        /// A collection of glyphs that together describe the CDU's large font.
        /// </summary>
        [DataMember]
        public McduFontGlyph[] LargeGlyphs { get; set; } = Array.Empty<McduFontGlyph>();

        /// <summary>
        /// A collection of glyphs that together describe the CDU's small font.
        /// </summary>
        [DataMember]
        public McduFontGlyph[] SmallGlyphs { get; set; } = Array.Empty<McduFontGlyph>();
    }
}
