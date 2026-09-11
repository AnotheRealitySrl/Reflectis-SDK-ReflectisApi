# Release notes

## Unreleased

The version in `package.json` is still 2.1.0; these entries have accumulated since it was set.

### Changed
- **The package is `Virtuademy-SDK-Library`** (id `com.anotherealitysrl.virtuademy-sdk-library`),
  and the namespace `Virtuademy.SDK.PlatformApi` is `Virtuademy.SDK.ApiData`. What is left here
  after the wire DTOs and the transport left is the application client: the endpoints an app
  calls, and nothing a creator installs.
- The declared dependency `com.anotherealitysrl.virtuademy-sdk-interface` is
  `com.anotherealitysrl.virtuademy-sdk-core`, the same package under the name it now has.

### Fixed
- The dependency block declared one of the three first-party packages this one actually
  compiles against. `spacs-utility` and `virtuademy-systemcore` are now declared too — they
  resolved anyway while every package is embedded, and would not have as soon as one was not.

## v2.1.0

### Added
- Added `NpcDTO` and the runtime `GetNpcs` / `GetNpcDetails` calls to `ReflectisDataAccessSystem`.
- `NpcDTO` carries `IsWorldDefault` and `OrderForPicker`, so a world's favourite NPC is pinned first in the picker.

## v2.0.0

### Changed
- Refactored the API from systems into a static API layer based on `BaseApi`.
- Added class references for system-related types.

### Fixed
- Fixed session participants handling.

## v1.1.1

### Added

- Added 3d asset generation APIs
- Added sessionDTO status and template

## v1.0.1

### Fixed

- Fix context field on send data in `AnalyticDTO`.

## v1.0.0

- Initial release
