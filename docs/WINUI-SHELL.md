# AConsulting WinUI shell (D5)

**Status:** Unpackaged WinUI 3 shell builds on VS 2022 Community · **Updated:** 2026-08-07

## Build

```bat
build-winui.cmd
```

Uses VS 2022 MSBuild (`dotnet build` alone fails on SDK 10 Appx Pri tasks).

## Pin

- `vendor/AQC` @ `aorms-bridge-d2`
- firm.db: `%LocalAppData%\AConsulting\firm.db`

Same activate / enqueue / Flush UI pattern as AStudio. Domain engineering UI next.
