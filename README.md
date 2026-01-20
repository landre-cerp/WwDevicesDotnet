# WW-Devices-Dotnet

The NuGet package for the library will be available at:

https://www.nuget.org/packages/ww-devices-dotnet/

*(Package will be published upon first release)*

Note : in 2026, Winwing will be rebranding Winctrl.

## Supported Devices

This library supports the following Winctrl devices:

### CDU Devices

- **Winctrl MCDU** (Airbus A320) - Captain, First Officer, and Observer positions
- **Winctrl PFP-3N** (Boeing 737 NG) - Captain, First Officer, and Observer positions
- **Winctrl PFP-7** (Boeing 777) - Captain, First Officer, and Observer positions
- **Winctrl PFP-4** (Boeing 747) - Captain, First Officer, and Observer positions

### Front Panel Devices

- **Winctrl FCU** (Airbus A320 Flight Control Unit) - Standalone or with EFIS panels (left, right, or both)
- **Winctrl PAP-3** (Boeing 737 Primary Autopilot Panel)
- **Winctrl PDC-3N** (Left and Right configurations)

Each device can be connected via USB and is automatically detected by the library. Use `CduFactory.FindLocalDevices()` to enumerate connected CDU devices or `FrontpanelFactory.FindLocalDevices()` to enumerate connected front panel devices.


## PRELIMINARY DOCUMENTATION

### Working with CDU Devices

To instantiate a CDU object call the `CduFactory` static:

```
using(var cdu = CduFactory.ConnectLocal())
```

You can pass an override to specify which CDU product to instantiate. There is
also a function that enumerates the CDU USB devices that are currently active.

The `ConnectLocal` function returns an instance of an `ICdu` interface. The latest
version of the interface is here:

https://github.com/landre-cerp/WwDevicesDotnet/blob/main/ICdu.cs


### Working with Front Panel Devices

To instantiate a front panel device (FCU, PAP-3, PDC-3N, etc.) call the `FrontpanelFactory` static:

```
using(var frontpanel = FrontpanelFactory.ConnectLocal())
```

Similar to CDU devices, you can specify which device to connect to or enumerate available devices.

The `ConnectLocal` function returns an instance of an `IFrontpanel` interface. The latest
version of the interface is here:

https://github.com/landre-cerp/WwDevicesDotnet/blob/main/IFrontpanel.cs

Front panel devices expose `ControlActivated` and `ControlDeactivated` events for button presses
and control changes, and provide an `UpdateDisplay` method to update the device's display(s).


### Checking Device Capabilities

Each front panel device exposes a `Capabilities` property that describes its high-level features.
This allows you to write adaptive code that works with different device models:

```csharp
using(var frontpanel = FrontpanelFactory.ConnectLocal())
{
    var caps = frontpanel.Capabilities;
    
    // Check if device supports barometric pressure display
    if(caps.CanDisplayBarometricPressure)
    {
        // Update QNH/QFE displays on EFIS panels
        var state = new FcuEfisState();
        state.LeftBaroPressure = 1013;
        state.LeftBaroQnh = true;
        frontpanel.UpdateDisplay(state);
    }
    
    // Check if device has pilot/copilot course displays (PAP-3 specific)
    if(caps.HasPilotCourseDisplay)
    {
        var state = new Pap3State();
        state.PltCourse = "045";
        state.CplCourse = "180";
        frontpanel.UpdateDisplay(state);
    }
    
    // Check if device supports flight level mode
    if(caps.HasFlightLevelMode)
    {
        // Can display "FL350" format (FCU and PAP-3)
        var state = new Pap3State();
        state.Altitude = 35000;
        state.AltitudeIsFlightLevel = true; // Will display as "FL350"
        frontpanel.UpdateDisplay(state);
    }
}
```

Available capability properties:
- `HasSpeedDisplay` - Device has speed/IAS display
- `HasHeadingDisplay` - Device has heading display
- `HasAltitudeDisplay` - Device has altitude display
- `HasVerticalSpeedDisplay` - Device has vertical speed display
- `CanDisplayBarometricPressure` - Device can display barometric pressure (EFIS panels)
- `CanDisplayQnhQfe` - Device can display QNH/QFE indicators (EFIS panels)
- `HasPilotCourseDisplay` - Device has pilot course display (PAP-3)
- `HasCopilotCourseDisplay` - Device has copilot course display (PAP-3)
- `SupportsAlphanumericDisplay` - Device can display letters, not just numbers
- `HasFlightLevelMode` - Device supports flight level display mode
- `HasMachSpeedMode` - Device supports Mach speed display mode



## Reading from the CDU

The `ICdu` interface exposes two events, `KeyDown` and `KeyUp`. These are passed
an event args that tells you which key was pressed or released. There are extension
methods on the `Key` enum to convert keys into different formats.



## Writing to the CDU

This is a two-step process. The `ICdu` exposes a `Screen` property which lets you
set the content of the display. Setting the content of a screen does not update the
CDU's display.

The `ICdu` exposes a function called `RefreshDisplay`. This function sends the
current content of `Screen` to the device.

By default `RefreshDisplay` will not refresh the display if nothing has changed since
the last update.



### Composing Output

The `Screen` class can be cumbersome to work with. There is a higher-level compositing
class called `Compositor` that is exposed on the `ICdu` via the `Output` property. It
offers a fluent interface for setting the content of a screen.



### Screen Buffers

Screens are not tied to an CDU, and they can be instantiated just like any other
object. There are a pair of functions, `CopyFrom` and `CopyTo`, that can be used to
copy the content of a screen buffer into the CDU's screen buffer.



### LEDs

Same process as per screen buffers - there is an `Led` class that carries the state and
brightness of the LED lights. The class is copyable.

There is a `RefreshLeds` function on the CDU object to copy the current state of the
`Led` buffer to the device. If nothing has changed since the last refresh then, by
default, nothing is sent.



### Display, LED and Backlight Brightness

There are properties for each of these on the `ICdu` interface that let you specify the
brightness levels as a percentage from 0 to 100. Note that if you set the display or LED
brightness to 0% then you can't see anything.

The brightness properties will not send commands to the device if it detects no change
from what was last sent. If you want to force the library to send brightness levels then
you can call `RefreshBrightnesses`.



### Colours

The display supports a palette of ten colours. The names and default values of the
colours follow Winctrl's defaults, but you are free to reassign the colours to anything
you like via the `Palette` property. After changing the palette you need to call
`RefreshPalette` to send your changes to the device.



### Cleanup

The CDU device will retain its state after your program stops driving it - I.E. it will
continue to show whatever you last wrote to the screen.

There is a function called `Cleanup` that will clear the screen, turn off all of the LEDs
and set the brightness levels to 0 (overridable).


### Fonts

The CDU device supports 1BPP bitmap fonts at varying widths and heights. However
`mcdu-dotnet` only supports fonts of either 29 or 31 pixels high and between 17 and 23
pixels wide.

Fonts are described by an `McduFontFile` object:

https://github.com/landre-cerp/WwDevicesDotnet/blob/main/McduFontFile.cs

Examples of font files can be found in the resources folder:

https://github.com/landre-cerp/WwDevicesDotnet/tree/main/Resources



### Events

Besides the `KeyDown` and `KeyUp` events referenced elsewhere there is also the
`Disconnected` event, which is raised when the library detects that the device has been
disconnected.