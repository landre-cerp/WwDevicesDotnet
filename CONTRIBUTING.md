# Contributing to WW-Devices-Dotnet

This library drives USB HID flight-sim hardware panels reverse-engineered from
their wire protocol. Most contributions add support for a new physical device.
This guide walks through the pieces that need to change.

## Project orientation

There are two device hierarchies, sharing the same registration mechanism but
different base classes:

- **CDU-style devices** (screen + keys - MCDU, PFP-3N, PFP-4, PFP-7): implement
  `ICdu` via the shared abstract base `CommonWinctrlPanel`
  ([`Winctrl\CommonWinctrlPanel.cs`](Winctrl/CommonWinctrlPanel.cs)), created
  through [`CduFactory`](CduFactory.cs).
- **Front-panel devices** (buttons + LEDs, no MCDU-style screen - FCU/EFIS,
  PAP-3, PDC-3N, AGP32): implement `IFrontpanel` via the shared abstract base
  `BaseFrontpanelDevice<TControl>` ([`Winctrl\BaseFrontpanelDevice.cs`](Winctrl/BaseFrontpanelDevice.cs)),
  created through [`FrontpanelFactory`](FrontpanelFactory.cs).

Every device, regardless of hierarchy, needs an entry in three shared enums/registries:

- [`Device.cs`](Device.cs) - one value per product line (e.g. `WinctrlPap3`).
- [`DeviceType.cs`](DeviceType.cs) - the broad category it replicates (e.g.
  `Boeing737FrontPanel`); reuse an existing value if it fits.
- [`SupportedDevices.cs`](SupportedDevices.cs) - one `DeviceIdentifier` per
  physical USB variant (VID/PID/seat), added to `_AllSupportedDevices` (CDU-style)
  or `_AllSupportedFrontpanels` (front-panel).

The project (`WW-Devices-Dotnet.csproj`) is SDK-style and globs all `.cs` files
automatically - **new device files under `Winctrl\<Name>\` don't need any
project file edits.**

## Adding a new device

1. **Register the device.**
   - Add a `Device` enum value in `Device.cs`.
   - Add a `DeviceType` value in `DeviceType.cs` if none of the existing categories fit.
   - Add one `DeviceIdentifier` per USB variant (e.g. per seat position, or per
     daisy-chain configuration) in `SupportedDevices.cs`, and include each one
     in the appropriate `_AllSupportedDevices`/`_AllSupportedFrontpanels` array.

2. **Create the device folder** `Winctrl\<Name>\`.

   **CDU-style device** (has an MCDU-style screen): subclass `CommonWinctrlPanel`
   and supply its three abstract members - `CommandPrefix`, `LedIndicatorCodeMap`,
   `KeyToFlagOffsetCallback`. Use
   [`Winctrl\Pfp7\Pfp7Device.cs`](Winctrl/Pfp7/Pfp7Device.cs) as the template - it's
   the smallest complete example. You'll also need a `KeyboardMap.cs` supplying
   `InputReport01FlagAndOffset(Key)`.

   **Front-panel device** (buttons/LEDs, no MCDU-style screen): subclass
   `BaseFrontpanelDevice<Control>` and supply `SendInitPacket()`, `GetControl()`,
   `UpdateDisplay()`, `UpdateLeds()`, `SetBrightness()`, `Capabilities`. You'll need:
   - `Control.cs` - a device-specific enum of buttons/controls.
   - `ControlMap.cs` - `InputReport01FlagAndOffset(Control)`.
   - `<Name>Capabilities.cs` - implements `IFrontpanelCapabilities` (see
     [`IFrontpanelCapabilities.cs`](IFrontpanelCapabilities.cs) for what each
     flag means). Set flags to `false` rather than guessing if the protocol
     hasn't been fully reverse-engineered yet - don't leave it undocumented
     (see the "Documentation conventions" note on `Agp32Capabilities` below).
   - `<Name>State.cs` / `<Name>Leds.cs` (optional) implementing `IFrontpanelState`/
     `IFrontpanelLeds`, if the device has a display or LEDs beyond what the base
     class covers. Devices downcast the interface parameter inside
     `UpdateDisplay`/`UpdateLeds`, e.g. `if (state is Pap3State pap3State) { ... }`.

   Use [`Winctrl\Agp32\Agp32Device.cs`](Winctrl/Agp32/Agp32Device.cs) as the
   template - it's the most complete and commented example, and defines its
   `State`/`Leds` classes in the same file (an alternative to Pap3's separate-file
   style; either is fine).

   > [!NOTE]
   > [`Winctrl\Pdc3nm\Pdc3Device.cs`](Winctrl/Pdc3nm/Pdc3Device.cs) isn't a useful
   > template if your device has a display or LEDs - its `UpdateDisplay`/`UpdateLeds`
   > are intentionally empty no-ops because the PDC-3N hardware has neither.

3. **Wire up the factory.** Add a `case Device.Winctrl<Name>:` branch:
   - CDU-style: [`CduFactory.ConnectLocal()`](CduFactory.cs) (around line 126).
   - Front-panel: [`FrontpanelFactory.ConnectLocal()`](FrontpanelFactory.cs) (around line 116).

4. **Document the protocol.** Add `Winctrl\<Name>\README.md` following the
   existing per-device convention: Command Prefix, USB Vendor/Product IDs,
   LEDs, Key Bitflags/Controls, and an "Implementation" section listing the
   files involved. Link back to [`Winctrl\README.md`](Winctrl/README.md) for
   anything the device shares with others instead of repeating it (see below).

## Documentation conventions

To avoid re-introducing duplication that's been cleaned up in this repo:

- Behavior shared across multiple devices (packet shapes, common key layouts)
  belongs in [`Winctrl\README.md`](Winctrl/README.md), once. Per-device READMEs
  should link to it and document only what's different for that device - see
  how [`Winctrl\Pfp7\README.md`](Winctrl/Pfp7/README.md) and
  [`Winctrl\Mcdu\README.md`](Winctrl/Mcdu/README.md) reference the shared
  [Line Select Key Bitflags](Winctrl/README.md#line-select-key-bitflags) table
  instead of repeating it.
- If a device's capabilities or protocol are genuinely unknown or incomplete
  (not yet reverse-engineered, or a stub implementation), say so explicitly in
  its README rather than leaving it silent - see the Status notes in
  [`Winctrl\Pdc3nm\README.md`](Winctrl/Pdc3nm/README.md) and
  [`Winctrl\Agp32\README.md`](Winctrl/Agp32/README.md).

## Building and CI

`dotnet build WW-Devices-Dotnet.sln` builds the library. Two GitHub Actions
workflows exist: [`build.yml`](.github/workflows/build.yml) runs on push/PR to
`main`/`develop`, and [`publish.yml`](.github/workflows/publish.yml) publishes
the NuGet package on a GitHub release - neither requires configuration changes
for a new device.
