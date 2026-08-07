# AConsulting

**Accelerated Consulting** — engineering consultancy OS on the AORMS platform.

Native **Windows** desktop app. Fork lineage: [HolagundiWorks/AQC](https://github.com/HolagundiWorks/AQC)
(WinUI 3 + C++ `bbs_engine`). Work, calculations, and AI run **locally**. Metadata,
progress, and final documents/drawings push to the AORMS hub for **firm-branded
portals**.

> **Open source for now.** SaaS commercial licensing deferred.

## Agent entry

Read **[AGENTS.md](AGENTS.md)** before coding. Architecture:
[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Identity

| | |
| --- | --- |
| Title | AConsulting |
| Expansion | Accelerated Consulting |
| Slug | `aconsulting` |
| Marketing host | `https://consultancy.aorms.in` |
| Hub / contracts | [esti](https://github.com/HolagundiWorks/esti) · [PORTAL-SYNC-BRIDGE](https://github.com/HolagundiWorks/esti/blob/main/docs/esti/PORTAL-SYNC-BRIDGE.md) |

## Source of truth

| Concern | Location |
| --- | --- |
| C++ engine / BBS / estimate | AQC `bbs_engine` (pin commit) |
| Hub sync API | esti HUB-API + PORTAL-SYNC-BRIDGE |
| Engineering domain (reference) | esti consultancy modules / NAVIGATION |
| This app UI + local DB | **this repository** |

## Build (target)

Same as AQC: CMake + MSVC for `bbs_engine`, .NET 8 WinUI 3 for the shell.
