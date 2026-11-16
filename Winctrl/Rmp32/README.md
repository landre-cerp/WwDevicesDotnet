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
| 10-11 | lower ring running counter (u16 LE) |
| 12-13 | upper ring running counter (u16 LE) |

Buttons (all confirmed on an RMP L): 1 swap, 2-5 VHF1/VHF2/VHF3/LOAD, 6-8 HF1/HF2/AM,
9-14 NAV/VOR/ILS/GLS/MLS/ADF, 15/16 lower ring left/right, 17/18 upper ring left/right
(one pulse per detent, the matching counter also changes), 19 knob push,
20 toggle up, 21 toggle down. SEL is an indicator, not a button.

## Illumination

`02 82 BB 00 00 03 49 <type> <value>` (14 bytes):
`00` key backlight, `01` display (digital tube) backlight, `02` LED brightness (all 0-255, confirmed),
`03`-`10` IND1-IND14 (0/1): SEL lamp, then the triangles above VHF1-VHF3, LOAD, HF1, HF2, AM, NAV, VOR, ILS, GLS, MLS, ADF (confirmed).

## Displays

Chunked `F0` stream, as on the AGP32 (see `Rmp32Device`). Framebuffer write
(cmd `02`, 40 bytes): bytes 4-9 STBY/CRS (right display), bytes 10-15 ACTIVE (left display), 6 digits each,
7-segment glyph per byte (bit 7 = decimal point). Commit is cmd `03`.
Bytes 0-3 and 16-39 must stay 0: a frame with 0xFF in either group is ignored by
the panel (tested on an RMP L).
The write message is 57 bytes, so its last byte spills into the next report
ahead of the commit message.
