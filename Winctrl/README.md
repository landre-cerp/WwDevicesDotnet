# Winctrl Panels

> [!NOTE]
> This is what I know about the USB packets that the devices send and/or
expect to receive. There are very large gaps in my understanding, this
is not comprehensive.
>
> If you can fill any of the gaps then please do.

These are the bits that all(*) Winctrl panels have in common, more or less.

(*) "All" is stretching things a bit, I've only looked at two (so far).


## FCU and EFIS Notes

Notes regarding the FCU and EFIS panels [can be found here](FcuAndEfis/README.md).


## Command Prefix

Many commands begin with a two byte prefix. This prefix changes between panels.
I've been calling this the **Command Prefix** in the code. In these notes you
need to substitute references to `{CP}` with the appropriate two-byte code.

| Product | Command Prefix (hex) |
| ---     | --- |
| PFP-3N  | 31 bb |
| MCDU    | 32 bb |
| PFP-7   | 33 bb |
| PFP-4   | 34 bb |
| AGP32   | 80 bb |

It's only a sample of two so far, but it is interesting that the CPs look similar
to the USB products IDs, and also that the interval between the MCDU and PFP-7 CPs
is the same as the interval between their product IDs.


## Line Select Key Bitflags

The MCDU, PFP-4 and PFP-7 all share the same 12 line-select buttons at the same
flags and packet offsets. Per-device READMEs only list the keys that are unique
to that device; they refer back here for these.

Offsets are zero-based in decimal from the start of the packet.

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


## Illumination

Setting the keyboard backlight, display backlight, LED brightness and
turning LEDs on or off all involves the same 14 byte packet to send
a {CP} ... 03 49 command.

The packet bytes are:

```
02 {CP} 00 00 03 49 <VARIABLE WORD> 00 00 00 00 00
```

The two bytes of the variable word vary depending on what you want to
control.

Brightness bytes range from 00 (off) to FF (full-on).

On / off values are either 0 (off) or 1 (on).

Note that setting the LED brightness to 0 will prevent any LED from displaying.

This 14-byte shape (`02 {ID} 00 00 03 49 {type} {value} ...`) is reused by the
front-panel families too, just with a different 2-byte device marker in place of
`{CP}` - see [FCU/EFIS notes](FcuAndEfis/README.md) (`{LE}`/`{FU}`/`{RE}`) and
[PAP-3 notes](Pap3/README.md) (`0100`) for their device-specific variable-type
tables. Only the CDU-style (MCDU/PFP-x) type/LED codes are listed below.


| Byte 1 | Byte 2                        | Device |
| ---    | ---                           | --- |
| 0x00   | Keyboard backlight brightness | ALL |
| 0x01   | Display backlight brightness  | ALL |
| 0x02   | LED brightness                | ALL |
| 0x03   | DSPY LED on / off             | PFP-7 |
| 0x04   | FAIL LED on / off             | PFP-7 |
| 0x05   | MSG LED on / off              | PFP-7 |
| 0x06   | OFST LED on / off             | PFP-7 |
| 0x07   | EXEC LED on / off             | PFP-7 |
| 0x08   | Fail LED on / off             | MCDU |
| 0x09   | FM LED on / off               | MCDU |
| 0x0a   | Mcdu LED on / off             | MCDU |
| 0x0b   | Menu LED on / off             | MCDU |
| 0x0c   | FM1 LED on / off              | MCDU |
| 0x0d   | IND LED on / off              | MCDU |
| 0x0e   | RDY LED on / off              | MCDU |
| 0x0f   | Blank / Line LED on / off     | MCDU |
| 0x10   | FM2 LED on / off              | MCDU |



## Display Output

These notes apply to all panels, there are no command prefixes, no device-specific
differences.

The screen is a 640 x 480 addressable framebuffer. The 24 x 14 character
grid is not a hardware limit, it is the layout this F2 path imposes on it -
see [Structured Commands](#structured-commands) for the pixel-level
functions that sit underneath.

Over the F2 path individual cells are not addressable, you need to send the
entire screen to the device.

The screen is filled by sending multiple 64 byte packets. Each packet always
starts with 0xF2, and then a sequence is generated for each cell and appended
to the packet. When the packet is filled it is sent, even if it contains a partial
sequence for a cell - the remaining bytes of the sequence are sent at the start
of the next packet.

E.G:

```
F2 <cell sequence><cell sequence>...<partial cell sequence>
F2 <remainder of cell sequence><cell sequence> etc.
```

The last F2 packet is padded with zeros to 64 bytes before sending.



### Cell Sequence

The length of a cell sequence depends on the size of the codepoint for the
character occupying the cell.

| Length | Meaning |
| ---    | --- |
| 2      | Colour and font value, big-endian |
| 1-4    | UTF-8 byte sequence for the character, big-endian |

To calculate the colour and font value you start by looking up the foreground
colour ordinal:

| Ordinal | Winctrl Default Colour |
| ---     | --- |
| 0       | Black |
| 1       | Amber |
| 2       | White |
| 3       | Cyan |
| 4       | Green |
| 5       | Magenta |
| 6       | Red |
| 7       | Yellow |
| 8       | Brown |
| 9       | Grey |
| 10      | Khaki |

Multiply the ordinal by 0x21 (33 decimal).

If the character is to be rendered in the large font then add 0.

If the character is to be rendered in the small font then add 0x16B (363).

If this is the first cell of the screen then add 1.

If this is the last cell of the screen then add 2.

### Where those numbers come from

They are not arbitrary. The device is told, at init, a set of *features*
and the values each can take; it then builds the cartesian product of them
all and indexes cells into that table. The multipliers are just the strides
of that product, in the order the features are declared:

| Feature | Values | Stride |
| ---     | ---    | --- |
| Font (large / small)          | 2  | 363 = `0x16B` |
| Foreground colour             | 11 | 33 = `0x21` |
| Background colour             | 11 | 3 |
| Cell marker (none/first/last) | 3  | 1 |

2 x 11 x 11 x 3 = 726 combinations, which is why the index needs two bytes.
So "add 1 for the first cell, 2 for the last" is the fourth feature, and
33 is simply 11 background colours x 3 markers.

See [Structured Commands](#structured-commands) for the init sequence that
declares all of this.



## Fonts

What I did here was dump SimAppPro sending a font to the device and then pore
over the dump to figure out where the glyphs were being sent and how they were
arranged. I then wrote a utility to turn that dump into a structured packet map
(currently JSON, and very large) that identifies the positions of all of the
glpyhs, and then I have some code take any set of glyphs you like and write them
into the packet map. The packets are then replayed to the device. It works, but
it's not elegant. It would be better if the code could take the font glyphs and
build the required `{CP}` commands from them.

The "replay a modified set of packets" approach is not intended to be the end of
the matter, it's just a way of getting the fonts working so that I can concentrate
on other things.

I have some notes elsewhere on the `{CP}` commands that carry font glyphs to the
device, I'll write them up later. Those notes can be seen in code form in the
`extract-font` utility's `WinctrlMcduUsbExtractor` class, which is what's responsible
for building the packet maps that this library uses.


### Glyphs

The font glyphs sent to the device are 1 bit-per-pixel bitmaps. The height and width
of the bitmaps is variable. Widths that are not divisible by 8 have a final byte on
each row that is padded to keep the rows aligned to a byte.

There's no sub-pixel rendering, no kerning, no line height - if you want spacing
between the glyphs then you need to build that spacing into the glyph itself.



### X and Y offset

Part of the set of `{CP}` commands sent to describe the font includes the setting up
of the X and Y offsets for the display. These supply the top-left corner of the
display, the point at which the device will start drawing characters when F2 packets
get sent to it.


## The Format Table

Before the device will accept screen data it has to be told what a cell can
look like. You declare a set of **features** and the values each may take;
the device builds the cartesian product of them and gives every combination
an index. F2 cell data then refers to a cell's appearance by that index
rather than spelling out its attributes - which is where the multipliers in
[Cell Sequence](#cell-sequence) come from.

There are four features. Foreground and background colour are two of them,
which is why those commands are the ones that look like a colour palette,
and why they do nothing on their own.

Each value is declared by one `{CP}` ... 19 01 command - `setFeatureInfo`,
function id 0x119, see [Structured Commands](#structured-commands) for the
encoding. Like previous commands, if they run past the end of a packet then
the sequence continues at the start of the next packet. Each colour must be
sent in ordinal order.

```
{CP} 00 00 19 01 00 00 04 17 01 00 00 0e 00 00 00 FB 00 BB GG RR AA SS 00 00 00 00 00 00 00
```

where:

| Byte Code | Meaning |
| ---       | --- |
| FB        | Feature id: 02 for foreground colours, 03 for background colours |
| BB        | Blue |
| GG        | Green |
| RR        | Red |
| AA        | Alpha (seems broken, SimAppPro always sends FF) |
| SS        | Format id - this value's index in the combination table |

`SS` looks like a sequence number because the ids happen to be allocated in
order, but it is the index the F2 cell data refers back to, not a counter.

The other 19 01 commands sent before and after are the remaining two
features:

| Feature id | Meaning | Values sent |
| ---        | ---     | --- |
| 01 | Font slot         | 5 and 6 |
| 02 | Foreground colour | the 11 palette colours |
| 03 | Background colour | the 11 palette colours |
| 04 | Cell marker       | 0, 1, 2 - the "first cell / last cell" flags |

The whole sequence is bracketed by `clearFeatureInfo` before, and
`setCompositeIndexBytes` + `buildFormatTable` after. That is exactly what
`InitialiseBasicFontsAndColours` sends: 31 sub-commands across 17 packets.
Removing any of it leaves the device without a table to index into, which is
why colours alone were never enough.

Changing the table has no effect on what is already on screen - you need to
redraw before you will see new colours.


## Structured Commands

Report 0xF0 carries a stream of length-delimited sub-commands. This is the
layer underneath the F2 screen path, and it is not restricted to a
character grid.

```
F0 <type:1> <seq:1> <len:1> <payload, up to 56 bytes>   padded to 64
```

`type` is 0x00 for host commands; the device answers 0x01 with a zero-length
payload as an ACK. Each sub-command in the payload is:

```
{CP} 00 00       prefix       (xx cb rather than xx bb on replies)
<fnId:4 LE>
<timestamp:4 LE> host clock in milliseconds
<isRespon:1>     ask the device to answer
<dataLen:4 LE>
<data>
```

A sub-command may straddle a packet boundary; the device reassembles the
stream.

The timestamp field is a millisecond counter taken when the command is
built - in a capture of SimAppPro its deltas match the elapsed time between
packets exactly, and commands built as one batch share a value. **The device
does not appear to validate it.** This library replays whatever values were
recorded in its dumps (`5f 63 31 00` in the init packets, an incrementing
`0x0001xxxx` series in the font maps) and the panels accept all of them, so
there is no need to generate real timestamps.

| Function | Id | Payload |
| ---      | ---| --- |
| refreshLCD              | 0x103 | - |
| getLastErrorString      | 0x105 | isRespon=1; replies 1 byte, 00 = no error |
| downLoadFontHead        | 0x106 | |
| downLoadFontData        | 0x107 | 512-byte chunks |
| cleanFont               | 0x108 | erases the font library |
| getAllFontVersion       | 0x109 | isRespon=1; count, offset, then id/version pairs |
| getAllFontInFlash       | 0x10A | isRespon=1; count, offset, then id + flag records |
| lcdFillRect             | 0x110 | `x:2 y:2 w:2 h:2` LE |
| setFont                 | 0x111 | `fontId:4` LE |
| setTextColor            | 0x112 | `A R G B` |
| setBackColor            | 0x113 | `A R G B` |
| setUTF8String           | 0x114 | `x:2 y:2` LE, UTF-8 bytes, NUL |
| setScreenInfo           | 0x118 | `x:2 y:2 rows:2 columns:2` LE |
| setFeatureInfo          | 0x119 | `featureId:2 value:4 formatId:8` |
| setCompositeIndexBytes  | 0x11A | 1 byte: index width |
| getCDUScreenLogicVersion| 0x11B | replies 2 bytes despite being sent isRespon=0 |
| buildFormatTable        | 0x11C | - |
| drawScreenGrip          | 0x11D | - |
| clearFeatureInfo        | 0x11E | - |

Function ids and payload layouts were established by observing USB traffic
to and from panels, and confirmed against the panels themselves. The names
are the ones SimAppPro uses for the same functions, kept so that anyone
comparing notes against a capture recognises them.

Ids 0x10B-0x10F and 0x115-0x117 are unaccounted for. A 0x104 appears
periodically in captures with a one-byte payload and has no known name.

`lcdFillRect` and `setUTF8String` both take raw pixel coordinates, so
rectangles and text compose anywhere on the 640 x 480 surface, independently
of the grid `setScreenInfo` declares. `setUTF8String` advances by the loaded
font's own width and paints an opaque background box per glyph in the
current `setBackColor`.


## Fonts Are Not Persistent

`getAllFontInFlash` reports `isInFlash = 0` for every slot on every panel
tested (PFP-3N, MCDU, PFP-4, PFP-7). Fonts live in RAM and have to be
uploaded once per session - a panel power-cycled without a host having run
has no glyphs at all, and `setUTF8String` draws nothing.

The format table above declares font slots **5 and 6**, not 1 and 2, and the
packet maps in `Resources` confirm it: each contains one `downLoadFontHead`
per slot 5 and 6, followed by 22 `downLoadFontData` chunks each. Slots 1 and
2 are reported by `getAllFontVersion` but are never written or selected.
