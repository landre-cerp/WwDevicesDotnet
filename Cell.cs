// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Text;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes a single character cell in the MCDU display.
    /// </summary>
    public class Cell
    {
        public char Character { get; set; }

        public Colour Colour { get; set; }

        // US spelling alias preserved for backwards compatibility.
        public Colour Color
        {
            get => Colour;
            set => Colour = value;
        }
        public Colour BackgroundColour { get; set; } = Colour.Black;

        public bool Small { get; set; }

        public Cell() : this(' ', Colour.White, false)
        {
        }

        public Cell(char character, Colour colour, bool small)
        {
            Character = character;
            Colour = colour;
            Small = small;
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append(Character);
            return result.ToString();
        }

        public void Clear()
        {
            Character = ' ';
            Colour = Colour.White;
            BackgroundColour = Colour.Black;
            Small = false;
        }

        public void Set(char character, Colour colour, bool small, Colour? backgroundColor = null)
        {
            Character = character;
            Colour = colour;
            Small = small;
            BackgroundColour = backgroundColor ?? Colour.Black;
        }

        public void CopyFrom(Cell other)
        {
            if(other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            Set(other.Character, other.Colour, other.Small, other.BackgroundColour);
        }

        public void CopyTo(Cell other)  => other?.CopyFrom(this);
    }
}
