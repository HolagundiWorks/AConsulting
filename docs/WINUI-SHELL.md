# AConsulting WinUI shell (D5 + HCW geography)

**Status:** Unpackaged WinUI 3 shell · **Updated:** 2026-08-09 (W3 Engagements)  
**Parity:** esti [`DESKTOP-WEB-PARITY-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WEB-PARITY-UX.md)

## Chrome (HCW scaffold)

```text
┌─ Ribbon (Practice · Clients · Projects · Office · Tasks) ─┐
├─ Stage — Engagements (Projects) | Tasks ──────────────────┤
├─ ActionDock — Clear · Import · Save · Reload · Publish ───┤
└─ Status tray · hub + licence ─────────────────────────────┘
```

| Ribbon | Stage | Dock Save | Dock Publish |
| --- | --- | --- | --- |
| **Projects** (live) | Engagements CRUD · `local_engagements` | Save engagement | Publish status (`engagementStatus` meta) |
| **Tasks** (live) | Local tasks board | Save local | Publish to hub (Mongo ops) |
| Practice · Clients · Office | Disabled placeholders | — | — |

Practice nav labels match consultancy IA (esti AConsulting taskbar groups).  
Materials match AStudio: Fog Gray · soft neu chrome · Radiant Orange commit · 8px.

## Engagements store (W3)

- Table: `local_engagements` in `%LocalAppData%\AConsulting\firm.db` (same path as bridge).
- Columns: `engagement_id`, `code`, `title`, `status`, `stage`, `discipline`, `notes`, `publish_state`, `updated_at`.
- Import: Connect catalog (`ConnectCatalog.List()` → Upsert).
- Publish: `EnqueueMeta("engagementStatus", id, payload)` → `QUEUED` → `FlushAsync` → `PUBLISHED`.

## Build / run

Same as AStudio — `build-winui.cmd` or VS 2022 F5. Env:

```bat
set ESTI_HUB_URL=http://127.0.0.1:4000
set ESTI_LICENSE_API_URL=http://127.0.0.1:4000/platform
set ESTI_PRODUCT_API_KEY=hlp_sk_...
```

firm.db: `%LocalAppData%\AConsulting\firm.db`

MSIX = D6. Office domain UI = next. AQC technical apps stay separate installers.
