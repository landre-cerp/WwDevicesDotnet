// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Pap3
{
    /// <summary>
    /// Represents the LED states for a PAP-3 Primary Autopilot Panel.
    /// LED command codes verified from hardware testing.
    /// </summary>
    public class Pap3Leds : IFrontpanelLeds
    {
        public bool N1 { get; set; }              
        public bool Speed { get; set; }        
        public bool Vnav { get; set; }         
        public bool LvlChg { get; set; }        
        public bool HdgSel { get; set; }        
        public bool Lnav { get; set; }          
        public bool VorLoc { get; set; }         
        public bool App { get; set; }           
        public bool AltHold { get; set; }        
        public bool Vs { get; set; }             
        public bool CmdA { get; set; }            
        public bool CwsA { get; set; }            
        public bool CmdB { get; set; }           
        public bool CwsB { get; set; }            
        public bool AtArm { get; set; }          
        public bool FdL { get; set; }            
        public bool FdR { get; set; }            
    }
}
