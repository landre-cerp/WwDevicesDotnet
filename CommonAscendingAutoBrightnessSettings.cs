// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    public class CommonAscendingAutoBrightnessSettings : CommonAutoBrightnessSettings
    {
        public int LowIntensityBelowAmbientPercent { get; set; }

        public int HighIntensityAboveAmbientPercent { get; set; }

        protected int BrightnessForAmbientPercent(int ambientPercent, int minimumIntensityPercent = 0)
        {
            var onRange = new PercentRange(
                Math.Max(minimumIntensityPercent, LowIntensityBelowAmbientPercent),
                HighIntensityAboveAmbientPercent
            );
            var intensity = new PercentRange(
                LowestIntensityPercent,
                HighestIntensityPercent
            );
            int result;
            switch(onRange.Compare(ambientPercent)) {
                case -1: // Ambient is lower than the point where we're full-off
                    result = intensity.Low;
                    break;
                case 1:  // Ambient is higher than the point where we're full-on
                    result = intensity.High;
                    break;
                default: // Ambient is between full-off and full-on
                    if(intensity.Range == 0) {
                        return intensity.High;
                    }
                    result = intensity.PowerTransformScaling(
                        onRange.PercentageOfRange(ambientPercent),
                        ScaleGamma
                    );
                    break;
            }

            return intensity.Clamp(result);
        }
    }
}
