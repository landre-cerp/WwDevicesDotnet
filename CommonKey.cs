// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

namespace WwDevicesDotNet
{
    /// <summary>
    /// An enumeration of keys that the CDUs have in common, or are *very* roughly
    /// equivalent (even if it's just in name and/or location on the keyboard).
    /// </summary>
    public enum CommonKey
    {
        DeviceSpecific = 1000,

        LineSelectLeft1 = Key.LineSelectLeft1,
        LineSelectLeft2 = Key.LineSelectLeft2,
        LineSelectLeft3 = Key.LineSelectLeft3,
        LineSelectLeft4 = Key.LineSelectLeft4,
        LineSelectLeft5 = Key.LineSelectLeft5,
        LineSelectLeft6 = Key.LineSelectLeft6,
        LineSelectRight1 = Key.LineSelectRight1,
        LineSelectRight2 = Key.LineSelectRight2,
        LineSelectRight3 = Key.LineSelectRight3,
        LineSelectRight4 = Key.LineSelectRight4,
        LineSelectRight5 = Key.LineSelectRight5,
        LineSelectRight6 = Key.LineSelectRight6,

        Digit1 = Key.Digit1,
        Digit2 = Key.Digit2,
        Digit3 = Key.Digit3,
        Digit4 = Key.Digit4,
        Digit5 = Key.Digit5,
        Digit6 = Key.Digit6,
        Digit7 = Key.Digit7,
        Digit8 = Key.Digit8,
        Digit9 = Key.Digit9,
        DecimalPoint = Key.DecimalPoint,
        Digit0 = Key.Digit0,
        PositiveNegative = Key.PositiveNegative,
        A = Key.A,
        B = Key.B,
        C = Key.C,
        D = Key.D,
        E = Key.E,
        F = Key.F,
        G = Key.G,
        H = Key.H,
        I = Key.I,
        J = Key.J,
        K = Key.K,
        L = Key.L,
        M = Key.M,
        N = Key.N,
        O = Key.O,
        P = Key.P,
        Q = Key.Q,
        R = Key.R,
        S = Key.S,
        T = Key.T,
        U = Key.U,
        V = Key.V,
        W = Key.W,
        X = Key.X,
        Y = Key.Y,
        Z = Key.Z,
        Slash = Key.Slash,
        Space = Key.Space,
        Clr = Key.Clr,

        Brt = Key.Brt,
        Dim = Key.Dim,
        Prog = Key.Prog,

        EitherOr =                  2000,
        InitOrInitRef =             EitherOr + 1,
        McduMenuOrMenu =            EitherOr + 2,
        FPlnOrRte =                 EitherOr + 3,
        AirportOrDepArr =           EitherOr + 4,
        RadNavOrNavRad =            EitherOr + 5,
        RightArrowOrNextPage =      EitherOr + 6,
        LeftArrowOrPrevPage =       EitherOr + 7,
        SecFPlnOrAltn =             EitherOr + 8,
        OvfyOrDel =                 EitherOr + 9,
        AtcCommOrFmcComm =          EitherOr + 10,

        // unmapped:
        // Legs,        PFP-*
        // Hold,        PFP-*
        // Exec,        PFP-*
        // Fix,         PFP-*
        // Clb,         PFP-3N
        // Crz,         PFP-3N
        // Des,         PFP-3N
        // N1Limit,     PFP-3N
        // Data,        MCDU
        // Dir,         MCDU
        // FuelPred,    MCDU
        // Perf,        MCDU
        // UpArrow,     MCDU
        // DownArrow,   MCDU
        // VNav,        PFP-7
    }
}
