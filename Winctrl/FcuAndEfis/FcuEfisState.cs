// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.FcuAndEfis
{
    public class FcuEfisState : IFrontpanelState
    {
        /// <summary>
        /// Mach speed are sent without the decimal point. For example, Mach 0.82 is sent as 082
        /// </summary>
        public int? Speed { get; set; }
        public int? Heading { get; set; }
        public int? Altitude { get; set; }
        public int? VerticalSpeed { get; set; }

        public bool SpeedIsMach { get; set; } = false;
        public bool HeadingIsTrack { get; set; } = false;
        public bool VsIsFpa { get; set; } = false;

        public bool AltitudeIsFlightLevel { get; set; } = false;

        public bool SpeedManaged { get; set; } = false;
        public bool HeadingManaged { get; set; } = false;
        public bool AltitudeManaged { get; set; } = false;

        public bool LatIndicator { get; set; } = false;
        
        public bool LvlIndicator { get; set; } = false;
        public bool LvlLeftBracket { get; set; } = false;
        public bool LvlRightBracket { get; set; } = false;
        public bool VsHorzIndicator { get; set; } = false;

        public int? LeftBaroPressure { get; set; }
        public bool LeftBaroQnh { get; set; }
        public bool LeftBaroQfe { get; set; }

        public int? RightBaroPressure { get; set; }
        public bool RightBaroQnh { get; set; }
        public bool RightBaroQfe { get; set; }
    }
}
