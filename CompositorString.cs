// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes a string that has style changes embedded within it in faux-HTML markup.
    /// </summary>
    public class CompositorString
    {
        /// <summary>
        /// The plain text with all embedding removed.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// A collection of style changes.
        /// </summary>
        public CompositorStringStyleChange[] StyleChanges { get; } = Array.Empty<CompositorStringStyleChange>();

        /// <summary>
        /// Creates a new object.
        /// </summary>
        /// <param name="text"></param>
        public CompositorString(string text)
        {
            (Text, StyleChanges) = Parse(text);
        }

        static Regex _EmbeddedStyleRegex = new Regex(
            @"\<(?<style>\<?(amber|brown|cyan|gray|green|grey|khaki|large|magenta|red|small|white|yellow))\>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static (string text, CompositorStringStyleChange[] styleChanges) Parse(string text)
        {
            var textBuffer = new StringBuilder();
            var styleChanges = new List<CompositorStringStyleChange>();
            var matches = _EmbeddedStyleRegex.Matches(text ?? "");

            var textStart = 0;
            void extractText(int toIndex)
            {
                for(var idx = textStart;idx < toIndex;++idx) {
                    textBuffer.Append(text[idx]);
                }
                textStart = toIndex;
            }

            foreach(Match match in matches) {
                extractText(match.Index);
                textStart = match.Index + match.Length;

                var style = match.Groups["style"].Value;
                if(style[0] == '<') {
                    textBuffer.Append(style);
                    textBuffer.Append('>');
                } else {
                    CompositorStringStyle styleEnum;
                    switch(style.ToLower()) {
                        case "amber":   styleEnum = CompositorStringStyle.Amber; break;
                        case "brown":   styleEnum = CompositorStringStyle.Brown; break;
                        case "cyan":    styleEnum = CompositorStringStyle.Cyan; break;
                        case "gray":    styleEnum = CompositorStringStyle.Grey; break;
                        case "green":   styleEnum = CompositorStringStyle.Green; break;
                        case "grey":    styleEnum = CompositorStringStyle.Grey; break;
                        case "khaki":   styleEnum = CompositorStringStyle.Khaki; break;
                        case "large":   styleEnum = CompositorStringStyle.Large; break;
                        case "magenta": styleEnum = CompositorStringStyle.Magenta; break;
                        case "red":     styleEnum = CompositorStringStyle.Red; break;
                        case "small":   styleEnum = CompositorStringStyle.Small; break;
                        case "white":   styleEnum = CompositorStringStyle.White; break;
                        case "yellow":  styleEnum = CompositorStringStyle.Yellow; break;
                        default:        throw new NotImplementedException();
                    }
                    styleChanges.Add(new CompositorStringStyleChange(textBuffer.Length, styleEnum));
                }
            }

            extractText(text.Length);

            return (textBuffer.ToString(), styleChanges.ToArray());
        }

        /// <inheritdoc/>
        public override string ToString() => Text;
    }
}
