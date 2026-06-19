// SPDX-FileCopyrightText: 2025 Laurent André
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet.Winctrl.Agp32
{
    /// <summary>
    /// An enumeration of all of the buttons and controls on the Winctrl 32 AGP Metal panel.
    /// Offsets and flags are populated from Wireshark trace in ControlMap.cs.
    /// Items marked TODO need verification against the HID report.
    /// </summary>
    public enum Control
    {
        
        BrkFanOff,
        BrkFanOn,            
        AutoBrkLo,              
        AutoBrkMed,             
        AutoBrkMax,            
        ASkidNwStrOff,            
        ASkidNwStrOn,            

        ChrRstPush,
        ChrRstLeft,
        ChrRstRight,
        ChrPush,
        ChrLeft,
        ChrRight,

        DateSetPush,
        DateSetLeft, 
        DateSetRight,

        TerrOnNd,
        Gps,
        Int,
        Set,
        Run,
        Stp,
        Rst,

        GearDown,                
        GearUp,                  
    }
}
