// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// LED auto-intensity settings.
    /// </summary>
    public class LedAutoIntensitySettings : CommonAscendingAutoBrightnessSettings
    {
        /// <summary>
        /// The lowest that the LED intensity can be set to. If a user were to set zero then
        /// none of the LEDs would be visible when switched on.
        /// </summary>
        public const int MinimumIntensityPercent = 10;

        public LedAutoIntensitySettings()
        {
            LowestIntensityPercent = 50;
            HighestIntensityPercent = 100;
            LowIntensityBelowAmbientPercent = 21;
            HighIntensityAboveAmbientPercent = 59;
            ScaleGamma = 1.0;
       }

        public int IntensityForAmbientPercent(int ambientPercent)
        {
            return BrightnessForAmbientPercent(ambientPercent, MinimumIntensityPercent);
        }
    }
}
