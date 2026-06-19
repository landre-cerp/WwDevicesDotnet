// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pap3
{
    /// <summary>
    /// Represents the display state for a PAP-3 Primary Autopilot Panel.
    /// Supports both numeric values (for backward compatibility) and string values (for alphanumeric display).
    /// </summary>
    public class Pap3State : IFrontpanelState
    {
        private string _speedDisplay;
        private string _pltCourse;
        private string _cplCourse;
        private string _headingDisplay;
        private string _altitudeDisplay;
        private string _verticalSpeedDisplay;

        /// <summary>
        /// Speed value (numeric). Use SpeedDisplay for alphanumeric control.
        /// Mach speeds are sent without decimal point (e.g., Mach 0.82 = 82).
        /// </summary>
        public int? Speed 
        { 
            get => int.TryParse(_speedDisplay, out var val) ? val : (int?)null;
            set => _speedDisplay = value?.ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// Speed display value (3-4 characters). Allows alphanumeric display (e.g., "FLt", "---").
        /// Setting this overrides the numeric Speed property.
        /// </summary>
        public string SpeedDisplay 
        { 
            get => _speedDisplay;
            set => _speedDisplay = value;
        }

        /// <summary>
        /// Pilot course value (3 digits, 0-359). For backward compatibility.
        /// </summary>
        public int? PltCourseValue
        {
            get => int.TryParse(_pltCourse, out var val) ? val : (int?)null;
            set => _pltCourse = value?.ToString("000");
        }

        /// <summary>
        /// Pilot course display (3 characters). Allows alphanumeric display.
        /// Can also be set with numeric values for backward compatibility.
        /// </summary>
        public string PltCourse 
        { 
            get => _pltCourse;
            set => _pltCourse = value;
        }

        /// <summary>
        /// Copilot course value (3 digits, 0-359). For backward compatibility.
        /// </summary>
        public int? CplCourseValue
        {
            get => int.TryParse(_cplCourse, out var val) ? val : (int?)null;
            set => _cplCourse = value?.ToString("000");
        }

        /// <summary>
        /// Copilot course display (3 characters). Allows alphanumeric display.
        /// Can also be set with numeric values for backward compatibility.
        /// </summary>
        public string CplCourse 
        { 
            get => _cplCourse;
            set => _cplCourse = value;
        }

        /// <summary>
        /// Heading value (numeric, 0-359). Use HeadingDisplay for alphanumeric control.
        /// </summary>
        public int? Heading
        {
            get => int.TryParse(_headingDisplay, out var val) ? val : (int?)null;
            set
            {
                if (value.HasValue)
                {
                    var v = value.Value;
                    if (v < 0)
                    {
                        v = 0;
                    }
                    else if (v > 359)
                    {
                        v = 359;
                    }
                    _headingDisplay = v.ToString("000");
                }
                else
                {
                    _headingDisplay = null;
                }
            }
        }

        /// <summary>
        /// Heading display value (3 characters). Allows alphanumeric display (e.g., "HDG", "---").
        /// Setting this overrides the numeric Heading property.
        /// </summary>
        public string HeadingDisplay
        {
            get => _headingDisplay;
            set => _headingDisplay = value;
        }

        /// <summary>
        /// Altitude value (numeric). Use AltitudeDisplay for alphanumeric control.
        /// </summary>
        public int? Altitude
        {
            get => int.TryParse(_altitudeDisplay, out var val) ? val : (int?)null;
            set => _altitudeDisplay = value?.ToString("00000");
        }

        /// <summary>
        /// Altitude display value (5 characters). Allows alphanumeric display (e.g., "FL350", "-----").
        /// Setting this overrides the numeric Altitude property.
        /// </summary>
        public string AltitudeDisplay
        {
            get => _altitudeDisplay;
            set => _altitudeDisplay = value;
        }

        /// <summary>
        /// Vertical speed value (numeric). Use VerticalSpeedDisplay for alphanumeric control.
        /// Absolute values are clamped to 0-9999 range (4-digit display limit).
        /// </summary>
        public int? VerticalSpeed
        {
            get => int.TryParse(_verticalSpeedDisplay, out var val) ? (VerticalSpeedPositive ? val : -val) : (int?)null;
            set
            {
                if (value.HasValue)
                {
                    VerticalSpeedPositive = value.Value >= 0;
                    var absValue = System.Math.Abs(value.Value);
                    absValue = System.Math.Min(9999, absValue);
                    _verticalSpeedDisplay = absValue.ToString("0000");
                }
                else
                {
                    _verticalSpeedDisplay = null;
                }
            }
        }

        /// <summary>
        /// Vertical speed display value (4 characters). Allows alphanumeric display (e.g., "----", "HOLD").
        /// Setting this overrides the numeric VerticalSpeed property. Absolute value only, sign via VerticalSpeedPositive.
        /// </summary>
        public string VerticalSpeedDisplay
        {
            get => _verticalSpeedDisplay;
            set => _verticalSpeedDisplay = value;
        }

        /// <summary>
        /// True if speed display is in Mach mode, false for knots.
        /// </summary>
        public bool SpeedIsMach { get; set; } = false;

        /// <summary>
        /// True if altitude is displayed as flight level.
        /// </summary>
        public bool AltitudeIsFlightLevel { get; set; } = false;

        /// <summary>
        /// True if heading display is in Track mode, false for Heading mode.
        /// </summary>
        public bool HeadingIsTrack { get; set; } = false;

        /// <summary>
        /// True if vertical speed display is in FPA mode, false for V/S mode.
        /// </summary>
        public bool VsIsFpa { get; set; } = false;

        /// <summary>
        /// True if vertical speed is positive (climb), false if negative (descent).
        /// Only used when VerticalSpeed is not null/empty.
        /// </summary>
        public bool VerticalSpeedPositive { get; set; } = true;

        /// <summary>
        /// True if the auto-throttle (A/T ARM) magnetic solenoid should be engaged (locked).
        /// When true, the A/T ARM switch is locked in position and cannot be moved.
        /// When false, the A/T ARM switch is free to be moved.
        /// 
        /// This controls the magnetic hold mechanism on the A/T ARM switch, not the altitude controls.
        /// Use UpdateSolenoid() to sync this state with the hardware.
        /// </summary>
        public bool MagneticActivated { get; set; } = false;
    }
}
