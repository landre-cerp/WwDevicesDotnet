// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes the point in a string where the style changes.
    /// </summary>
    public class CompositorStringStyleChange
    {
        /// <summary>
        /// The point in the string where the style changes.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The new style.
        /// </summary>
        public CompositorStringStyle Style { get; }

        /// <summary>
        /// Creates an object.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="style"></param>
        public CompositorStringStyleChange(int index, CompositorStringStyle style)
        {
            Index = index;
            Style = style;
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{Index}]={Style}";
    }
}
