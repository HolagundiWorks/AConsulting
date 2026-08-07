# Engine + bridge pin (D5 kickoff)

**Status:** Kickoff · **Updated:** 2026-08-07  
**Upstream:** [HolagundiWorks/AQC](https://github.com/HolagundiWorks/AQC) `main`

Same pin as AStudio — share `bbs_engine` + `Aorms.Bridge`; specialize engineering domain UI only.

| Artifact | Consume from |
| --- | --- |
| C++ `bbs_engine` | AQC — single SoT |
| `Aorms.Bridge` | AQC `BBSDesktop/Aorms.Bridge` (merged) |
| Wire | esti PORTAL-SYNC-BRIDGE · HUB-API `2026-08` |

Open source; SaaS licensing deferred.
