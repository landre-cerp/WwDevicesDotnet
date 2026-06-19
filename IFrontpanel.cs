// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    /// <summary>
    /// The common interface for all frontpanel devices (FCU, EFIS, etc.) that the library
    /// can interact with.
    /// </summary>
    public interface IFrontpanel : IDisposable
    {
        /// <summary>
        /// The USB and library identifiers for the device.
        /// </summary>
        DeviceIdentifier DeviceId { get; }

        /// <summary>
        /// Gets a value indicating whether the device is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the capabilities of this frontpanel device.
        /// </summary>
        IFrontpanelCapabilities Capabilities { get; }

        /// <summary>
        /// Raised when a button or control is pressed/activated.
        /// </summary>
        event EventHandler<FrontpanelEventArgs> ControlActivated;

        /// <summary>
        /// Raised when a button or control is released/deactivated.
        /// </summary>
        event EventHandler<FrontpanelEventArgs> ControlDeactivated;

        /// <summary>
        /// Raised when the USB device has been disconnected.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        /// Raised whenever an input report is received from the device.
        /// </summary>
        event EventHandler<FrontpanelInputReportEventArgs> InputReportReceived;

        /// <summary>
        /// Updates the display(s) on the frontpanel.
        /// </summary>
        /// <param name="state">The state to display.</param>
        void UpdateDisplay(IFrontpanelState state);

        /// <summary>
        /// Updates the LEDs on the frontpanel.
        /// </summary>
        /// <param name="leds">The LED states to apply.</param>
        void UpdateLeds(IFrontpanelLeds leds);

        /// <summary>
        /// Sets the brightness levels for the frontpanel.
        /// </summary>
        /// <param name="panelBacklight">Panel backlight brightness (0-255).</param>
        /// <param name="lcdBacklight">LCD backlight brightness (0-255).</param>
        /// <param name="ledBacklight">LED backlight brightness (0-255).</param>
        void SetBrightness(byte panelBacklight, byte lcdBacklight, byte ledBacklight);

        /// <summary>
        /// Puts the device into its known-good default state: visible brightness,
        /// blank displays, all LEDs off. The device keeps its state across host
        /// restarts, so call this after connecting to recover from whatever a
        /// previous (possibly crashed) session left behind.
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// Event arguments for frontpanel control events.
    /// </summary>
    public class FrontpanelEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the control identifier.
        /// </summary>
        public string ControlId { get; }

        /// <summary>
        /// Gets the raw data associated with this event.
        /// </summary>
        public byte[] RawData { get; }

        public FrontpanelEventArgs(string controlId, byte[] rawData)
        {
            ControlId = controlId;
            RawData = rawData;
        }
    }

    /// <summary>
    /// Event arguments for frontpanel input report events.
    /// </summary>
    public class FrontpanelInputReportEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the report data from the device.
        /// </summary>
        public byte[] RawData { get; }

        public FrontpanelInputReportEventArgs(byte[] rawData)
        {
            RawData = rawData;
        }
    }

    /// <summary>
    /// Event arguments for frontpanel rotary encoder events.
    /// </summary>
    public class FrontpanelRotaryEventArgs : FrontpanelEventArgs
    {
        /// <summary>
        /// Gets the direction of rotation. Positive for clockwise, negative for counter-clockwise.
        /// </summary>
        public int Direction { get; }

        public FrontpanelRotaryEventArgs(string controlId, int direction, byte[] rawData)
            : base(controlId, rawData)
        {
            Direction = direction;
        }
    }

    /// <summary>
    /// Interface for frontpanel display state.
    /// </summary>
    public interface IFrontpanelState
    {
        /// <summary>
        /// Speed value (knots or Mach). Supported by all frontpanels.
        /// </summary>
        int? Speed { get; set; }

        /// <summary>
        /// Heading value (0-359 degrees). Supported by all frontpanels.
        /// </summary>
        int? Heading { get; set; }

        /// <summary>
        /// Altitude value (feet). Supported by all frontpanels.
        /// </summary>
        int? Altitude { get; set; }

        /// <summary>
        /// Vertical speed value (feet per minute). Supported by all frontpanels.
        /// </summary>
        int? VerticalSpeed { get; set; }
    }

    /// <summary>
    /// Interface for frontpanel LED states.
    /// </summary>
    public interface IFrontpanelLeds
    {
    }
}
