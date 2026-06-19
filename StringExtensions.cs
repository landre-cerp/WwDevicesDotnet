// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WwDevicesDotNet
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this string hexString)
        {
            hexString = hexString ?? "";

            var buffer = new byte[hexString.Length / 2];
            byte b = 0;
            var low = false;

            for(int chIdx = 0, bufferIdx = 0;chIdx < hexString.Length;++chIdx) {
                var nibble = ConvertNibble(hexString[chIdx]);
                b |= low
                    ? nibble
                    : (byte)(nibble << 4);
                if(low) {
                    buffer[bufferIdx++] = b;
                    b = 0;
                }
                low = !low;
            }
            if(low) {
                throw new InvalidDataException($"{hexString} is missing a hex digit");
            }

            return buffer;
        }

        public static byte ConvertNibble(char ch)
        {
            if(!TryConvertNibble(ch, out var result)) {
                throw new InvalidDataException($"{ch} is not a hex digit");
            }
            return result;
        }

        public static bool TryConvertNibble(char ch, out byte nibble)
        {
            var result = true;
            ch = char.ToLower(ch);
            if(ch >= '0' && ch <= '9') {
                nibble = (byte)(ch - '0');
            } else if(ch >= 'a' && ch <= 'f') {
                nibble = (byte)((ch - 'a') + 10);
            } else if(ch >= 'A' && ch <= 'F') {
                nibble = (byte)((ch - 'A') + 10);
            } else {
                nibble = 0;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Splits a long string into lines at whitespace characters. All whitespace is
        /// turned into a single space. Newlines start a new line. Lines can exceed
        /// <paramref name="lineLength"/> if they do not contain whitespace.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineLength"></param>
        /// <returns></returns>
        public static IReadOnlyList<string> WrapAtWhitespace(this string text, int lineLength)
        {
            var result = new List<string>();
            var lineBuffer = new StringBuilder();

            void addLineToResult()
            {
                if(lineBuffer.Length > 0) {
                    result.Add(lineBuffer.ToString());
                    lineBuffer.Clear();
                }
            }

            var realLines = (text ?? "").Split('\n');
            foreach(var realLine in realLines) {
                var chunks = realLine
                    .Trim('\r')
                    .Split(' ', '\t');
                foreach(var chunk in chunks) {
                    if(chunk.Length + lineBuffer.Length + (lineBuffer.Length > 0 ? 1 : 0) > lineLength) {
                        addLineToResult();
                    }
                    if(lineBuffer.Length > 0) {
                        lineBuffer.Append(' ');
                    }
                    for(var charIdx = 0;charIdx < chunk.Length;++charIdx) {
                        lineBuffer.Append(chunk[charIdx]);
                        if(lineBuffer.Length == lineLength) {
                            addLineToResult();
                        }
                    }
                }
                addLineToResult();
            }

            return result;
        }
    }
}
