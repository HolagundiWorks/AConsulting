# AConsulting WinUI shell (D5 + HCW geography)

**Status:** Unpackaged WinUI 3 shell · **Updated:** 2026-08-10 (Ask ESTI)  
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
| **Practice** (live) | Capacity · **Ask ESTI** (Ollama) · firm notes | Save notes | Flush meta |
| **Clients** (live) | `local_clients` | Save client | Publish client (`clientStatus`) |
| **Projects** (live) | `local_engagements` | Save engagement | Publish status (`engagementStatus`) |
| **Office** (live) | `local_office_enquiries` | Save enquiry | Publish decision (`officeEnquiry`) |
| **Tasks** (live) | Local tasks board | Save local | Publish to hub (Mongo ops) |

Ask ESTI mirrors AStudio S4 — local Ollama only; transcripts never synced.  
Env: `ESTI_OLLAMA_URL` · `ESTI_OLLAMA_MODEL`.

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
set ESTI_OLLAMA_URL=http://127.0.0.1:11434
set ESTI_OLLAMA_MODEL=llama3.2
```

MSIX = D6. Deep KPIs / RACI = later. AQC technical apps stay separate.
