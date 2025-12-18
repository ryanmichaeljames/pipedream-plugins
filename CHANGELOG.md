# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `CreateOrganizationService(Guid userId)` method to `ILocalPluginContext` for creating organization services for specific users

## [1.1.0] - 2024-12-18

### Changed

- Renamed `InitiatingUserService` to `OrganizationService` for better naming consistency
- Renamed `PluginUserService` to `ElevatedOrganizationService` for clarity on elevated privileges

## [1.0.1] - 2024-12-18

### Changed

- Made `ExecuteDataversePlugin` abstract instead of virtual to enforce implementation

### Fixed

- Added missing `using Moq;` directive in test files
- CI/CD pipeline updated to run on Windows for .NET Framework support

## [1.0.0] - 2024-12-17

### Added

- `PluginBase` - Enhanced base class for Dataverse plugins
  - Automatic correlation ID and user logging on entry/exit
  - Exception wrapping for `OrganizationServiceFault`
  - Support for secure and unsecure configuration strings
- `ILocalPluginContext` - Simplified access to plugin context and services
  - `Target`, `TargetReference`, `PreImage`, `PostImage` helper properties
  - `MessageName`, `PrimaryEntityName`, `PrimaryEntityId` shortcuts
  - `InitiatingUserService` and `PluginUserService` for organization operations
  - `Trace`, `LogInfo`, `LogWarning`, `LogError` methods with automatic caller info
  - Application Insights integration via `ILogger`
- `LocalTracingService` - Tracing with time delta prefixes for performance diagnostics

[Unreleased]: https://github.com/ryanmichaeljames/pipe-dream-dataverse-plugins/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/ryanmichaeljames/pipe-dream-dataverse-plugins/compare/v1.0.1...v1.1.0
[1.0.1]: https://github.com/ryanmichaeljames/pipe-dream-dataverse-plugins/compare/v1.0.0...v1.0.1
[1.0.0]: https://github.com/ryanmichaeljames/pipe-dream-dataverse-plugins/releases/tag/v1.0.0
