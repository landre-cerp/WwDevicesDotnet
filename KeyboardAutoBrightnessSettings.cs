// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    public class KeyboardAutoBrightnessSettings : CommonDescendingAutoBrightnessSettings
    {
        public KeyboardAutoBrightnessSettings()
        {
            LowestIntensityPercent = 0;
            HighestIntensityPercent = 80;
            LowIntensityAboveAmbientPercent = 16;
            HighIntensityBelowAmbientPercent = 6;
            ScaleGamma = 1.0;
        }
    }
}
