// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    public class CommonAutoBrightnessSettings
    {
        public int LowestIntensityPercent { get; set; } = 45;

        public int HighestIntensityPercent { get; set; } = 100;

        public double ScaleGamma { get; set; } = 3.0;
    }
}
