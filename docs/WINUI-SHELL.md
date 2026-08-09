# AConsulting WinUI shell (D5 + HCW geography)

**Status:** Unpackaged WinUI 3 shell · **Updated:** 2026-08-09 (W3+ Office enquiries)  
**Parity:** esti [`DESKTOP-WEB-PARITY-UX.md`](https://github.com/HolagundiWorks/aorms/blob/main/docs/esti/DESKTOP-WEB-PARITY-UX.md)

## Chrome (HCW scaffold)

```text
┌─ Ribbon (Practice · Clients · Projects · Office · Tasks) ─┐
├─ Stage — Engagements | Enquiries | Tasks ─────────────────┤
├─ ActionDock — Clear · [Import] · Save · Reload · Publish ─┤
└─ Status tray · hub + licence ─────────────────────────────┘
```

| Ribbon | Stage | Dock Save | Dock Publish |
| --- | --- | --- | --- |
| **Projects** (live) | Engagements CRUD · `local_engagements` | Save engagement | Publish status (`engagementStatus`) |
| **Office** (live) | Enquiry go/no-go · `local_office_enquiries` | Save enquiry | Publish decision (`officeEnquiry`) |
| **Tasks** (live) | Local tasks board | Save local | Publish to hub (Mongo ops) |
| Practice · Clients | Disabled placeholders | — | — |

Materials: Fog Gray · soft neu chrome · Radiant Orange commit · 8px. Dock ≤5; Import only on Projects.

## Engagements store (W3)

- Table: `local_engagements` in `%LocalAppData%\AConsulting\firm.db`
- Import: Connect catalog → Upsert
- Publish: `EnqueueMeta("engagementStatus", …)` → Flush

## Office enquiries (W3+)

- Table: `local_office_enquiries` — `enquiry_id`, `subject`, `client_name`, `decision` (`DRAFT`/`GO`/`NO_GO`), `notes`, `publish_state`
- Publish only when decision is **GO** or **NO_GO** (`officeEnquiry` meta)

## Build / run

```bat
build-winui.cmd
set ESTI_HUB_URL=http://127.0.0.1:4000
```

firm.db: `%LocalAppData%\AConsulting\firm.db`  
MSIX = D6. Practice/Clients = next. AQC technical apps stay separate installers.
