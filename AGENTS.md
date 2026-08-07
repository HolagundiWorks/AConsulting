# AConsulting — agent contract

**Product:** AConsulting (Accelerated Consulting) — engineering practice desktop OS.  
**Stack:** C# WinUI 3 + C++ `bbs_engine` (fork AQC). **Not** the esti React SPA.

## Do

- Keep **C++ engine** as SoT for every quantity/money number.  
- Persist firm work in **local SQLite**; push allow-listed meta/artifacts via `aorms_bridge`.  
- Follow [docs/SYNC-CONTRACT.md](docs/SYNC-CONTRACT.md).  
- Local AI only. Stay **open source**; SaaS licensing deferred.

## Do not

- Reimplement BBS/estimate outside `bbs_engine`.  
- Sync drafts, AI chats, or unissued drawings.  
- Build browser staff ERP.  
- Diverge the shared engine pin without an explicit engine PR.

## Read first

1. [README.md](README.md)  
2. [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)  
3. [docs/DOMAIN.md](docs/DOMAIN.md)  
4. [docs/PORTAL-PUSH.md](docs/PORTAL-PUSH.md)  
5. [docs/ROADMAP.md](docs/ROADMAP.md)  
