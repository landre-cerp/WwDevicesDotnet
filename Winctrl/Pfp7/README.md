# Winctrl PFP-7

> [!NOTE]
> This is what I know about the USB packets that the devices send and/or
expect to receive. There are very large gaps in my understanding, this
is not comprehensive.
>
> If you can fill any of the gaps then please do.

These are the bits that are unique about the PFP-7. See
[Winctrl Panels Readme](../README.md) for everything that it has in common
with other Winctrl panels.

## USB Vendor and Product IDs

The Winctrl vendor ID is `0x4098`.

| Device | Product ID |
| --- | --- |
| PFP-7 (Captain) | 0xBB37 |
| PFP-7 (F/O) | 0xBB3F |
| PFP-7 (Observer) | 0xBB3B |

## Command Prefix

The command prefix is 0x33 0xbb.

## Supported LEDs

The PFP-7 supports the following indicator LEDs:

| LED Name | Code | Description |
| --- | --- | --- |
| DSPY | 0x03 | Display LED |
| FAIL | 0x04 | Fail indicator LED |
| MSG | 0x05 | Message LED |
| OFST | 0x06 | Offset LED |
| EXEC | 0x07 | Execute LED |

## Key Bitflags

The 12 line-select keys (offsets 1-2) are shared with MCDU and PFP-4 - see
[Line Select Key Bitflags](../README.md#line-select-key-bitflags) in the shared
Winctrl notes.

The PFP-7 is otherwise identical to the [PFP-4 keyboard map](../Pfp4/README.md#key-bitflags),
with one difference:

| Key   | Flag | Packet Byte Index | PFP-4 equivalent |
| ---   | ---  | --- | --- |
| Altn  | 0x80 | 2   | `Atc` |

(See `KeyboardMap.cs` for the authoritative mapping.)

## Implementation

The implementation lives in:
- `Pfp7Device.cs` - Main device driver (inherits from `CommonWinctrlPanel`)
- `KeyboardMap.cs` - Input report mapping

## Display

The PFP-7 uses the common Winctrl MCDU display protocol for LCD screen updates. See the
[Winctrl Panels Readme](../README.md) for details on screen rendering, fonts, and color
palette management.
