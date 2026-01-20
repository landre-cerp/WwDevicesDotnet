# Winctrl PFP-4

> [!NOTE]
> These notes describe the current reverse-engineered HID protocol implementation for the
> Winctrl PFP-4 MCDU panel.
>
> The PFP-4 is very similar to the PFP-3N, sharing most of the protocol with the main
> differences being the product ID and command prefix.

These are the bits that are unique about the PFP-4. See
[Winctrl Panels Readme](../README.md) for everything that it has in common
with other Winctrl panels.

## USB Vendor and Product IDs

The Winctrl vendor ID is `0x4098`.

| Device | Product ID | Decimal |
| --- | --- | --- |
| PFP-4 | 0xBB38 | 47928 |

## Command Prefix

The command prefix for the PFP-4 is `0x34`.

## Supported LEDs

The PFP-4 supports the following indicator LEDs:

| LED Name | Code | Description |
| --- | --- | --- |
| DSPY | 0x03 | Display LED |
| FAIL | 0x04 | Fail indicator LED |
| MSG | 0x05 | Message LED |
| OFST | 0x06 | Offset LED |
| EXEC | 0x07 | Execute LED |

## Brightness Control

The PFP-4 supports three independent brightness controls:

- **Display Brightness** (`DisplayBrightnessPercent`): Controls the LCD screen backlight (0-100%)
- **Backlight Brightness** (`BacklightBrightnessPercent`): Controls keyboard key illumination (0-100%)
- **LED Brightness** (`LedBrightnessPercent`): Controls the intensity of indicator LEDs/markers (0-100%)

## Key Bitflags

The device sends keyboard input via `0x01` input reports. Offsets are zero-based in decimal 
from the start of the packet.

| Key              | Flag | Packet Byte Index |
| ---              | ---  | --- |
| LineSelectLeft1  | 0x01 | 1 |
| LineSelectLeft2  | 0x02 | 1 |
| LineSelectLeft3  | 0x04 | 1 |
| LineSelectLeft4  | 0x08 | 1 |
| LineSelectLeft5  | 0x10 | 1 |
| LineSelectLeft6  | 0x20 | 1 |
| LineSelectRight1 | 0x40 | 1 |
| LineSelectRight2 | 0x80 | 1 |
| LineSelectRight3 | 0x01 | 2 |
| LineSelectRight4 | 0x02 | 2 |
| LineSelectRight5 | 0x04 | 2 |
| LineSelectRight6 | 0x08 | 2 |
| InitRef          | 0x10 | 2 |
| Rte              | 0x20 | 2 |
| DepArr           | 0x40 | 2 |
| Atc              | 0x80 | 2 |
| VNav             | 0x01 | 3 |
| Dim              | 0x02 | 3 |
| Brt              | 0x04 | 3 |
| Fix              | 0x08 | 3 |
| Legs             | 0x10 | 3 |
| Hold             | 0x20 | 3 |
| FmcComm          | 0x40 | 3 |
| Prog             | 0x80 | 3 |
| Exec             | 0x01 | 4 |
| Menu             | 0x02 | 4 |
| NavRad           | 0x04 | 4 |
| PrevPage         | 0x08 | 4 |
| NextPage         | 0x10 | 4 |
| Digit1           | 0x20 | 4 |
| Digit2           | 0x40 | 4 |
| Digit3           | 0x80 | 4 |
| Digit4           | 0x01 | 5 |
| Digit5           | 0x02 | 5 |
| Digit6           | 0x04 | 5 |
| Digit7           | 0x08 | 5 |
| Digit8           | 0x10 | 5 |
| Digit9           | 0x20 | 5 |
| DecimalPoint     | 0x40 | 5 |
| Digit0           | 0x80 | 5 |
| PositiveNegative | 0x01 | 6 |
| A                | 0x02 | 6 |
| B                | 0x04 | 6 |
| C                | 0x08 | 6 |
| D                | 0x10 | 6 |
| E                | 0x20 | 6 |
| F                | 0x40 | 6 |
| G                | 0x80 | 6 |
| H                | 0x01 | 7 |
| I                | 0x02 | 7 |
| J                | 0x04 | 7 |
| K                | 0x08 | 7 |
| L                | 0x10 | 7 |
| M                | 0x20 | 7 |
| N                | 0x40 | 7 |
| O                | 0x80 | 7 |
| P                | 0x01 | 8 |
| Q                | 0x02 | 8 |
| R                | 0x04 | 8 |
| S                | 0x08 | 8 |
| T                | 0x10 | 8 |
| U                | 0x20 | 8 |
| V                | 0x40 | 8 |
| W                | 0x80 | 8 |
| X                | 0x01 | 9 |
| Y                | 0x02 | 9 |
| Z                | 0x04 | 9 |
| Space            | 0x08 | 9 |
| Del              | 0x10 | 9 |
| Slash            | 0x20 | 9 |
| Clr              | 0x40 | 9 |

(See `KeyboardMap.cs` for the authoritative mapping.)

## Implementation

The implementation lives in:
- `Pfp4Device.cs` - Main device driver (inherits from `CommonWinctrlPanel`)
- `KeyboardMap.cs` - Input report mapping

## Display

The PFP-4 uses the common Winctrl MCDU display protocol for LCD screen updates. See the
[Winctrl Panels Readme](../README.md) for details on screen rendering, fonts, and color
palette management.

