# Winctrl 32 RMP

Radio Management Panel. Three USB configurations (vendor `0x4098`), all
using the same protocol and the same command prefix **82 BB**:

| Config | Product ID |
| ---    | ---        |
| RMP L  | 0xBB83     |
| RMP R  | 0xBB84     |
| RMP C  | 0xBB85     |

Decoded from USB captures of the L and C configurations (R assumed identical).
The C capture only contains the init sequence, LED/backlight test and displays;
no key presses.

## Input report (24 bytes, report ID 01)

| Bytes | Content |
| ---   | --- |
| 1-3   | 21 buttons, button N = bit N-1 (LSB first) |
| 10-11 | encoder A running counter (u16 LE) |
| 12-13 | encoder B running counter (u16 LE) |

Buttons: 1 swap, 2-5 VHF1/VHF2/VHF3/LOAD, 6-8 HF1/HF2/AM, 9-14 NAV/VOR/ILS/GLS/MLS/ADF
(1-14 per the SimAppPro drawing; only 1-7 were pressed in the capture),
15/16 encoder A -/+, 17/18 encoder B -/+ (one pulse per detent, counter changes too),
19 knob push, 20 toggle OFF, 21 toggle ON.

## Illumination

`02 82 BB 00 00 03 49 <type> <value>` (14 bytes):
`00` backlight, `01` digital tube backlight (0-255), `02` marker light (assumed),
`03`-`10` IND1-IND14 (0/1).

## Displays

Chunked `F0` stream, as on the AGP32 (see `Rmp32Device`). Framebuffer write
(cmd `02`, 40 bytes): bytes 4-9 ACTIVE, bytes 10-15 STBY/CRS, 6 digits each,
7-segment glyph per byte (bit 7 = decimal point). Commit is cmd `03`.
The write message is 57 bytes, so its last byte spills into the next report
ahead of the commit message.
