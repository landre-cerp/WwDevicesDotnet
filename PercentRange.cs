// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// A value type that holds two integers that represent the low and high values in a
    /// pair of percentages. Both values are clamped to between 0 and 100, and low is
    /// always going to be lower than or equal to high.
    /// </summary>
    public readonly struct PercentRange
    {
        public int Low { get; }

        public int High { get; }

        public int Range => High - Low;

        public PercentRange(int low, int high)
        {
            low = Percent.Clamp(low);
            high = Percent.Clamp(high);
            if(low > high) {
                (low, high) = (high, low);
            }
            Low = low;
            High = high;
        }

        public override string ToString() => $"{Low}-{High}";

        public override bool Equals(object obj)
        {
            var result = Object.ReferenceEquals(this, obj);
            if(!result && obj is PercentRange other) {
                result = Low == other.Low && High == other.High;
            }
            return result;
        }

        public override int GetHashCode() => (High << 8) | Low;

        /// <summary>
        /// Clamps the percentage to the <see cref="Low"/> and <see cref="High"/> values.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public int Clamp(int percent) => Math.Max(Low, Math.Min(High, percent));

        /// <summary>
        /// Returns -1 if <paramref name="percent"/> is below <see cref="Low"/>,
        /// 1 if <paramref name="percent"/> is above <see cref="High"/> and 0 if
        /// it is between <see cref="Low"/> and <see cref="High"/> inclusive.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public int Compare(int percent)
        {
            return percent < Low ? -1
                : percent > High ? 1
                : 0;
        }

        /// <summary>
        /// If <paramref name="percent"/> is betwen <see cref="Low"/> and <see cref="High"/>
        /// inclusive then this returns how far between the two points it is, expressed as
        /// a percentage of <see cref="Range"/>. If <see cref="Range"/> is zero (I.E. the
        /// low and high values are the same) then 100 is returned. If
        /// <paramref name="percent"/> is not in range then 0 is returned.
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="inverted">Inverts the result.</param>
        /// <returns></returns>
        public int PercentageOfRange(int percent, bool inverted = false)
        {
            var result = 0;

            percent = Percent.Clamp(percent);
            if(Compare(percent) == 0) {
                if(Range == 0) {
                    result = 100;
                } else {
                    result = (int)(((double)(percent - Low) / (double)Range) * 100.0);
                }
            }

            if(inverted) {
                result = 100 - result;
            }

            return result;
        }

        /// <summary>
        /// Returns the value between <see cref="Low"/> and <see cref="High"/> represented
        /// by the <see cref="percent"/> value passed across.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public int LinearScaling(int percent)
        {
            percent = Percent.Clamp(percent);
            return percent == 0 ? Low
                : percent == 100 ? High
                : Range == 0 ? High
                : Low + (int)((double)Range * ((double)percent / 100.0));
        }

        /// <summary>
        /// Returns the value between <see cref="Low"/> and <see cref="High"/> after scaling
        /// the <see cref="percent"/> value passed across using a power transform centred on
        /// 50 and scaled to [0,100]. If gamma is 1 then we just do linear scaling.
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public int PowerTransformScaling(int percent, double gamma)
        {
            percent = Percent.Clamp(percent);

            if(gamma != 1.0 && gamma > 0.0) {
                var centred = (percent - 50.0) / 50.0;
                var raised = Math.Pow(Math.Abs(centred), gamma);
                var signed = Math.Sign(centred) * raised;
                var adjusted = 50.0 * (1.0 + signed);
                percent = (int)(adjusted + 0.5);
            }

            return LinearScaling(percent);
        }
    }
}
