# PipeDream.Plugins Copilot Instructions

## Overview
NuGet package providing enhanced PluginBase, context extensions, and utilities for Dataverse plugins. Works with PipeDream.Magos tooling.

## Core Principles
- Keep it simple - clarity over cleverness
- Stateless plugins - never store services or context in instance fields
- Fail fast - validate early with clear error messages
- One file per class, filename matches class name

## Code Style
- Singular naming for constants: `Stage`, `Message`, `Mode`
- Explicit over implicit, no magic strings
- One responsibility per class
- Early returns, guard clauses
- Comments only when non-obvious

## Stack
- .NET Framework 4.6.2 (required for Dataverse sandbox)
- Microsoft.CrmSdk.CoreAssemblies 9.0.2+
- Strong-named assembly (Key.snk required)

## References
- [Dataverse Best Practices](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/best-practices/business-logic/)
- [Keep a Changelog](https://keepachangelog.com/)
