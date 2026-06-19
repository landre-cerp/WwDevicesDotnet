// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;
using System.Text;

namespace WwDevicesDotNet
{
    /// <summary>
    /// Describes a row of cells on the MCDU display.
    /// </summary>
    public class Row
    {
        public Cell[] Cells { get; } = new Cell[Metrics.Columns];

        public Row()
        {
            for(var idx = 0;idx < Cells.Length;++idx) {
                Cells[idx] = new Cell();
            }
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            foreach(var cell in Cells) {
                buffer.Append(cell.Character);
            }
            return buffer.ToString();
        }

        public void Clear()
        {
            for(var idx = 0;idx < Cells.Length;++idx) {
                Cells[idx].Clear();
            }
        }

        public void CopyTo(Row other)
        {
            if(other == null) {
                throw new ArgumentNullException(nameof(other));
            }
            for(var idx = 0;idx < Cells.Length;++idx) {
                other.Cells[idx].CopyFrom(Cells[idx]);
            }
        }

        public void CopyFrom(Row other) => other?.CopyTo(this);

        public void ShiftRight(int startColumn, int length, int count)
        {
            if(count > 0 && length > 0) {
                for(var idx = length - 1;idx >= 0;--idx) {
                    var fromIdx = startColumn + idx;
                    var toIdx = fromIdx + count;
                    Cells[fromIdx].CopyTo(Cells[toIdx]);
                }
                for(var idx = 0;idx < count;++idx) {
                    Cells[startColumn + idx].Clear();
                }
            }
        }
    }
}
