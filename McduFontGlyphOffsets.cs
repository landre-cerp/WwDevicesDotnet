// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WwDevicesDotNet
{
    [DataContract]
    public class McduFontGlyphOffsets
    {
        [DataMember]
        public char Character { get; set; }

        [DataMember]
        public int[] CodepointMap { get; set; } = Array.Empty<int>();

        [DataMember]
        public int[] GlyphMap { get; set; } = Array.Empty<int>();

        public static int[] CompressOffsetMap(int[] offsets)
        {
            var result = new List<int>();

            int? previousOffset = null;
            for(var idx = 0;idx < offsets.Length;++idx) {
                var offset = offsets[idx];
                if(offset - 1 == previousOffset) {
                    previousOffset = offset;
                    result[result.Count - 1] = offset;
                } else {
                    result.Add(offset);
                    if(result.Count >= 2) {
                        if(result[result.Count - 2] == offset - 1) {
                            result[result.Count - 1] = result[result.Count - 2];
                            result[result.Count - 2] = -1;
                            result.Add(offset);
                            previousOffset = offset;
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public static int[] DecompressMap(int[] map)
        {
            var result = new List<int>();

            for(var idx = 0;idx < map.Length;++idx) {
                var offset = map[idx];
                if(offset != -1) {
                    result.Add(offset);
                } else if(offset == -1 && idx + 2 < map.Length) {
                    var endOffset = map[idx + 2];
                    for(offset = map[idx + 1];offset <= endOffset;++offset) {
                        result.Add(offset);
                    }
                    idx += 2;
                }
            }

            return result.ToArray();
        }
    }
}
