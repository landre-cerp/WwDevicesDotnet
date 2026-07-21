# Winctrl PDC-3N

> [!NOTE]
> This is what I know about the USB packets that the device sends and/or
> expects to receive. There are gaps in my understanding, this is not
> comprehensive.

## Status

> [!WARNING]
> **Display and LED output are not implemented yet.** `Pdc3Device.UpdateDisplay()`
> and `Pdc3Device.UpdateLeds()` are both empty no-op methods, and there are no
> `Pdc3State`/`Pdc3Leds` classes. Reading buttons/controls and the ambient light
> sensors, and setting panel backlight brightness, all work. Do not use
> `Pdc3Device.cs` as a copy-paste template for a new device - see
> [`../Agp32/Agp32Device.cs`](../Agp32/Agp32Device.cs) or
> [`../Pfp7/Pfp7Device.cs`](../Pfp7/Pfp7Device.cs) instead.

## USB Vendor and Product IDs

The Winctrl vendor ID is `0x4098`.

| Configuration | Product ID |
| --- | --- |
| PDC-3NL (Left) | 0xBB61 |
| PDC-3NR (Right) | 0xBB62 |

## Command Prefix

LED/brightness commands use the prefix `0x60bb` (verified from hardware testing;
see `Pdc3Device._Pdc3LedPrefix`). The panel backlight is set via type `0x00` in the
same 14-byte packet shape described in the
[shared Illumination notes](../README.md#illumination).

## Controls

The device sends a stream of `0x01` input reports. The mapping between bits and
controls is defined in `ControlMap.InputReport01FlagAndOffset()` - see
`ControlMap.cs` for the authoritative table.

## Ambient Light

`Pdc3Device` exposes `LeftAmbientLightNative`, `RightAmbientLightNative` and a
normalized `AmbientLightPercent` (0-100), raising `AmbientLightChanged` when the
normalized value changes.

## Implementation

The implementation lives in:
- `Pdc3Device.cs` - Main device driver (inherits from `BaseFrontpanelDevice<Control>`)
- `Control.cs` - Button/control enumeration
- `ControlMap.cs` - Input report mapping
- `Pdc3Capabilities.cs` - Capability flags
