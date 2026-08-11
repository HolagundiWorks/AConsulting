# AConsulting WinUI shell (HCW geography)

**Status:** Unpackaged WinUI 3 · **Updated:** 2026-08-10  
**Canon:** esti [`DESKTOP-WINUI-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WINUI-UX.md) · [`DESKTOP-WEB-PARITY-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WEB-PARITY-UX.md)

## Chrome (manager full shell)

```text
┌─ Floating ribbon 56 — brand · Local AI ──────────────────────────────┐
├─ Stage (Fog) — Practice · Clients · Projects · Office · Tasks ───────┤
│              ╭─ ActionDock Clear · Import · Save · Reload · Publish ─╮│
├─ floating Taskbar 60 — Practice…Tasks | Licence · Re-import · Flush ──┤
└─ (clock / wellness — optional follow-on; AStudio reference) ─────────┘
```

| Token / rule | Value |
| --- | --- |
| Theme | `Themes/HcwTheme.xaml` (AStudio SoT copy) |
| Density | **1×** — no window scale |
| Hits | Dock **44** · taskbar **35** · radius **8** |
| Active nav | Accent underline (not orange fill) |
| Module nav | Taskbar CENTER — not ribbon |
| Licence | **Activate only in AORMS Connect**; import `session.json` |

## Stages

| Taskbar | Stage | Dock Save | Dock Publish |
| --- | --- | --- | --- |
| **Practice** | Capacity · Ask ESTI · firm notes | Save notes | Flush meta |
| **Clients** | `local_clients` | Save client | Publish `clientStatus` |
| **Projects** | `local_engagements` | Save engagement | Publish `engagementStatus` |
| **Office** | `local_office_enquiries` | Save enquiry | Publish `officeEnquiry` |
| **Tasks** | Local tasks board | Save local | Publish hub |

Ask ESTI = local Ollama only. Env: `ESTI_OLLAMA_URL` · `ESTI_OLLAMA_MODEL`.

## Build

```bat
build-winui.cmd
```

firm.db: `%LocalAppData%\AConsulting\firm.db`
