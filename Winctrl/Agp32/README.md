# Winctrl AGP32 (32 AGP Metal)

> [!NOTE]
> This is what I know about the USB packets that the device sends and/or
> expects to receive, decoded from a Wireshark trace. There are gaps in
> my understanding, this is not comprehensive.

The AGP32 is an Airbus A320-style clock/chrono panel (CHR, clock and ET digital
displays plus gear/brake/terrain indicator LEDs) - it is not an EFIS or autopilot
panel, so it has no speed/heading/altitude/course displays.

## Capabilities

`Agp32Capabilities` reports `false` for every autopilot/EFIS-oriented flag
(speed, heading, altitude, baro, course, flight level, Mach) - the AGP32 has
none of those displays. It reports `true` for `HasClockDisplay`,
`HasChronometerDisplay` and `HasElapsedTimeDisplay`, matching the panel's three
digital fields (Clock, CHR, ET). `SupportsAlphanumericDisplay` is `false`
since all three fields are digits-only (see `Agp32Device.PutDigit`).

## USB Vendor and Product IDs

The Winctrl vendor ID is `0x4098`.

| Device | Product ID |
| --- | --- |
| 32 AGP Metal | 0xBB80 |

## Protocol

Unlike the CDU-style panels and PAP-3, the AGP32 uses a chunked `0xF0` transport
carrying a byte stream rather than the `{CP}`-prefixed command scheme:

```
F0 [SQ hi] [SQ lo] [chunkLen] [chunk bytes ...]
F0 02 <all zeros>                                = stream reset / init
```

Messages inside the stream:

```
[unitId u32 LE] [cmd u8] 01 00 00 [timestampMs u32 LE] [00] [dataLen u32 LE] [data]
  cmd 0x02 : framebuffer write (dataLen = 0x24 = 36 = 9 x u32 LE)
  cmd 0x03 : commit / end-of-frame (dataLen = 0)
```

The unit ID is `0x0000BB80` (wire bytes `80 BB`), following the same convention
PAP-3 uses for its display prefix. Unlike the PAP-3 (176-byte framebuffer split
over 4 reports), the AGP32 framebuffer fits in a single chunk, so one display
update is exactly two reports: a `0x35`-length cmd-02 report and a `0x11`-length
cmd-03 report.

### Framebuffer layout

9 little-endian `u32` words at packet offset `0x15`:

| Word | Meaning |
| --- | --- |
| 0 | Always 0 |
| 1-7 | Segment planes a,b,c,d,e,f,g - bit *n* = digit column *n* |
| 8 | Punctuation plane (colon dots) - bit *n* = column *n* |

Digit columns 0-3 are CHR (MM SS, presumed - blank in the trace), 4-9 are the
clock (HH MM SS), 10-13 are ET (HH MM).

### Illumination

Panel backlight, LCD backlight and LED brightness/on-off use the same 14-byte
packet shape described in the [shared Illumination notes](../README.md#illumination),
using unit ID `80bb` in place of `{CP}`. LED type codes are listed in
`Agp32State.Agp32Led` (e.g. `Gear1Unlk = 0x03`, `BrkFanHot = 0x06`, `GearDownRed = 0x12`).

## Controls

The device sends a stream of `0x01` input reports, plus two axis-light sensor
readings (`LeftAxisLightRaw`/`RightAxisLightRaw`, exposed fused as `AxisLightValue`).
The mapping between bits and controls is defined in
`ControlMap.InputReport01FlagAndOffset()` - see `ControlMap.cs` for the
authoritative table.

## Implementation

The implementation lives in:
- `Agp32Device.cs` - Main device driver (inherits from `BaseFrontpanelDevice<Control>`),
  also defines `Agp32State`/`Agp32Leds` in the same file
- `Control.cs` - Button/control enumeration
- `ControlMap.cs` - Input report mapping
- `Agp32Capabilities.cs` - Capability flags (see Capabilities above)
