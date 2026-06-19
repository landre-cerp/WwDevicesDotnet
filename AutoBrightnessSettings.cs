// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// The auto-brightness settings.
    /// </summary>
    public class AutoBrightnessSettings
    {
        /// <summary>
        /// True if the backlight intensities should track the ambient light, false
        /// if they should not. Disabled by default.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The keyboard backlight auto-brightness settings.
        /// </summary>
        public KeyboardAutoBrightnessSettings KeyboardBacklight { get; } = new KeyboardAutoBrightnessSettings();

        /// <summary>
        /// The display backlight auto-brightness settings.
        /// </summary>
        public DisplayAutoBrightnessSettings DisplayBacklight { get; } = new DisplayAutoBrightnessSettings();

        /// <summary>
        /// The LED auto-intensity settings.
        /// </summary>
        public LedAutoIntensitySettings LedIntensity { get; } = new LedAutoIntensitySettings();
    }
}
