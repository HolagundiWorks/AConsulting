# AConsulting WinUI shell (D5 + HCW geography)

**Status:** Unpackaged WinUI 3 shell · **Updated:** 2026-08-10 (Practice live)  
**Parity:** esti [`DESKTOP-WEB-PARITY-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WEB-PARITY-UX.md)

## Chrome (HCW scaffold)

```text
┌─ Ribbon (Practice · Clients · Projects · Office · Tasks) ─┐
├─ Stage — capacity / notes | clients | engagements | … ───┤
├─ ActionDock — Clear · [Import] · Save · Reload · Publish ─┤
└─ Status tray · hub + licence ─────────────────────────────┘
```

| Ribbon | Stage | Dock Save | Dock Publish |
| --- | --- | --- | --- |
| **Practice** (live) | Capacity counts · hub sync · firm notes (`local_practice`) | Save notes | Flush meta |
| **Clients** (live) | `local_clients` | Save client | Publish client (`clientStatus`) |
| **Projects** (live) | `local_engagements` | Save engagement | Publish status (`engagementStatus`) |
| **Office** (live) | `local_office_enquiries` | Save enquiry | Publish decision (`officeEnquiry`) |
| **Tasks** (live) | Local tasks board | Save local | Publish to hub (Mongo ops) |

All five ribbon peers live (≤5). Materials: Fog Gray · soft neu · Radiant Orange · 8px. Import only on Projects.

## Stores (firm.db)

Path: `%LocalAppData%\AConsulting\firm.db`

| Table | Role |
| --- | --- |
| `local_practice` | Firm name + notes (single row) |
| `local_clients` | `clientStatus` meta |
| `local_engagements` | `engagementStatus` (+ Connect import) |
| `local_office_enquiries` | `officeEnquiry` (GO / NO_GO) |
| Bridge `local_tasks` | Mongo ops |

## Build / run

```bat
build-winui.cmd
set ESTI_HUB_URL=http://127.0.0.1:4000
```

MSIX = D6. Deep KPIs / RACI = later. AQC technical apps stay separate.
