# Release notes

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
