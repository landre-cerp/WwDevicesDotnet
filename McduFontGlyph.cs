// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Runtime.Serialization;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes a single bitmap glyph in a font.
    /// </summary>
    [DataContract]
    public class McduFontGlyph
    {
        /// <summary>
        /// The character being described.
        /// </summary>
        [DataMember]
        public char Character { get; set; }

        /// <summary>
        /// The 1BPP bitmap expressed as strings of either 1 or X for ones and anything else
        /// for zeros. Each row must have the same number of bits.
        /// </summary>
        [DataMember]
        public string[] BitArray { get; set; } = Array.Empty<string>();

        /// <inheritdoc/>
        public override string ToString() => new String(Character, 1);

        /// <summary>
        /// Parses <see cref="BitArray"/> into a two dimensional array of rows and bytes. If the length of the
        /// rows in <see cref="BitArray"/> are not wholly divisible by 8 then the LSB(s) of the last byte on
        /// each row will be zero.
        /// </summary>
        /// <returns>A two dimensional array of bytes in each row.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the number of bits in each line is variable.
        /// </exception>
        public byte[,] GetBytes()
        {
            byte[,] result = null;

            if(BitArray?.Length > 0) {
                var bitLength = BitArray[0].Length;
                var byteWidth = (bitLength / 8) + (bitLength % 8 != 0 ? 1 : 0);
                result = new byte[BitArray.Length, byteWidth];
                for(var rowIdx = 0;rowIdx < BitArray.Length;++rowIdx) {
                    var bitText = BitArray[rowIdx];
                    if(bitText.Length != bitLength) {
                        throw new InvalidOperationException(
                            $"Invalid bitarray for {Character} - first row is {bitLength} bits, " +
                            $"row {rowIdx + 1} is {bitText.Length}"
                        );
                    }

                    for(int colIdx = 0, byteIdx = 0, bitShift = 7;colIdx < bitText.Length;++colIdx) {
                        var bit = 0;
                        switch(bitText[colIdx]) {
                            case '1':
                            case 'X':
                                bit = 1;
                                break;
                        }
                        result[rowIdx, byteIdx] |= (byte)(bit << bitShift);

                        --bitShift;
                        if(bitShift == -1) {
                            bitShift = 7;
                            ++byteIdx;
                        }
                    }
                }
            }

            return result ?? new byte[0,0];
        }
    }
}
