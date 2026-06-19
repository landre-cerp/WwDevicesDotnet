// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    public class DisplayAutoBrightnessSettings : CommonAscendingAutoBrightnessSettings
    {
        /// <summary>
        /// The lowest that the display intensity can be set to. If a user were to set zero then
        /// nothing would be visible on the display.
        /// </summary>
        public const int MinimumIntensityPercent = 5;

        public DisplayAutoBrightnessSettings()
        {
            LowestIntensityPercent = 70;
            HighestIntensityPercent = 100;
            LowIntensityBelowAmbientPercent =   6;
            HighIntensityAboveAmbientPercent =  69;
            ScaleGamma = 2;
        }

        public int BrightnessForAmbientPercent(int ambientPercent)
        {
            return BrightnessForAmbientPercent(ambientPercent, MinimumIntensityPercent);
        }
    }
}
