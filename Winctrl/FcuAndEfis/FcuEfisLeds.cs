// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.FcuAndEfis
{
    public class FcuEfisLeds : IFrontpanelLeds
    {
        public bool Loc { get; set; }
        public bool Ap1 { get; set; }
        public bool Ap2 { get; set; }
        public bool AThr { get; set; }
        public bool Exped { get; set; }
        public byte ExpedYellowBrightness { get; set; } = 0;
        public bool Appr { get; set; }

        public bool LeftFd { get; set; }
        public bool LeftLs { get; set; }
        public bool LeftCstr { get; set; }
        public bool LeftWpt { get; set; }
        public bool LeftVorD { get; set; }
        public bool LeftNdb { get; set; }
        public bool LeftArpt { get; set; }

        public bool RightFd { get; set; }
        public bool RightLs { get; set; }
        public bool RightCstr { get; set; }
        public bool RightWpt { get; set; }
        public bool RightVorD { get; set; }
        public bool RightNdb { get; set; }
        public bool RightArpt { get; set; }
    }
}
