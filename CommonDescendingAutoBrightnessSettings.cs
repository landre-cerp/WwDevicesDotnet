// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    public class CommonDescendingAutoBrightnessSettings : CommonAutoBrightnessSettings
    {
        public int LowIntensityAboveAmbientPercent { get; set; }

        public int HighIntensityBelowAmbientPercent { get; set; }

        public int BrightnessForAmbientPercent(int ambientPercent)
        {
            var onRange = new PercentRange(
                HighIntensityBelowAmbientPercent,
                LowIntensityAboveAmbientPercent
            );
            var intensity = new PercentRange(
                LowestIntensityPercent,
                HighestIntensityPercent
            );
            switch(onRange.Compare(ambientPercent)) {
                case -1: // Ambient is lower than the point where we're full-on
                    return intensity.High;
                case 1:  // Ambient is higher than the point where we're full-off
                    return intensity.Low;
                default: // Ambient is between full-on and full-off
                    if(intensity.Range == 0) {
                        return intensity.High;
                    }
                    var scaled = intensity.PowerTransformScaling(
                        onRange.PercentageOfRange(ambientPercent, inverted: true),
                        ScaleGamma
                    );
                    return intensity.Clamp(scaled);
            }
        }
    }
}
