# AConsulting WinUI shell (D5 + HCW geography)

**Status:** Unpackaged WinUI 3 shell · **Updated:** 2026-08-09  
**Parity:** esti [`DESKTOP-WEB-PARITY-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WEB-PARITY-UX.md)

## Chrome (HCW scaffold)

```text
┌─ Ribbon (Practice · Clients · Projects · People · Office · Tasks) ─┐
├─ Stage (Fog Gray) — bridge Activate · Tasks ───────────────────────┤
├─ ActionDock — Clear · Save local · Reload · Publish ───────────────┤
└─ Status tray ──────────────────────────────────────────────────────┘
```

Practice nav labels match consultancy IA (esti AConsulting taskbar groups).  
Materials match AStudio: Fog Gray · soft neu chrome · Radiant Orange commit · 8px.

## Build / run

Same as AStudio — `build-winui.cmd` or VS 2022 F5. Env:

```bat
set ESTI_HUB_URL=http://127.0.0.1:4000
set ESTI_LICENSE_API_URL=http://127.0.0.1:4000/platform
set ESTI_PRODUCT_API_KEY=hlp_sk_...
```

firm.db: `%LocalAppData%\AConsulting\firm.db`

MSIX = D6. Engagements · Office domain UI = next. AQC technical apps stay separate installers.
