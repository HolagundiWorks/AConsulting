# AConsulting WinUI shell (D5 + HCW geography)

**Status:** Unpackaged WinUI 3 shell · **Updated:** 2026-08-10 (Clients)  
**Parity:** esti [`DESKTOP-WEB-PARITY-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WEB-PARITY-UX.md)

## Chrome (HCW scaffold)

```text
┌─ Ribbon (Practice · Clients · Projects · Office · Tasks) ─┐
├─ Stage — Clients | Engagements | Enquiries | Tasks ───────┤
├─ ActionDock — Clear · [Import] · Save · Reload · Publish ─┤
└─ Status tray · hub + licence ─────────────────────────────┘
```

| Ribbon | Stage | Dock Save | Dock Publish |
| --- | --- | --- | --- |
| **Clients** (live) | Client directory · `local_clients` | Save client | Publish client (`clientStatus`) |
| **Projects** (live) | Engagements · `local_engagements` | Save engagement | Publish status (`engagementStatus`) |
| **Office** (live) | Enquiry go/no-go · `local_office_enquiries` | Save enquiry | Publish decision (`officeEnquiry`) |
| **Tasks** (live) | Local tasks board | Save local | Publish to hub (Mongo ops) |
| Practice | Disabled placeholder | — | — |

Materials: Fog Gray · soft neu chrome · Radiant Orange commit · 8px. Dock ≤5; Import only on Projects.

## Stores (firm.db)

Path: `%LocalAppData%\AConsulting\firm.db`

| Table | Meta entity |
| --- | --- |
| `local_clients` | `clientStatus` |
| `local_engagements` | `engagementStatus` (+ Connect catalog import) |
| `local_office_enquiries` | `officeEnquiry` (GO / NO_GO only) |
| Bridge `local_tasks` | Mongo ops publish |

## Build / run

```bat
build-winui.cmd
set ESTI_HUB_URL=http://127.0.0.1:4000
```

MSIX = D6. Practice KPIs = next. AQC technical apps stay separate installers.
