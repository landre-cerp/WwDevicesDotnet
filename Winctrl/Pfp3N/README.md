# Winctrl PFP-3N

I don't have one of these, so support is provisional. I think it should work,
if it doesn't then let me know.

See [Winctrl Panels Readme](../README.md) for everything that it has in common
with other Winctrl panels, including the shared
[Line Select Key Bitflags](../README.md#line-select-key-bitflags).

## Command Prefix

The command prefix is 0x31 0xbb (see `Winctrl\README.md`'s Command Prefix
table). Confirmed from a USBPcap capture of SimAppPro driving a PFP-3N.

## Implementation

The implementation lives in:
- `Pfp3NDevice.cs` - Main device driver (inherits from `CommonWinctrlPanel`)
- `KeyboardMap.cs` - Input report mapping
