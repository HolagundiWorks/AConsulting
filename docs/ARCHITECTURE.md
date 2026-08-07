# AConsulting architecture

Same stack as AStudio / AQC:

```
WinUI 3 shell (C#)
  ├── Engineering domain UI
  ├── Local SQLite firm DB
  ├── Aorms.Bridge
  └── bbs_engine.dll (C++ SoT)
```

See AStudio `docs/ARCHITECTURE.md` and esti PORTAL-SYNC-BRIDGE. Share the bridge
package; specialize domain UI only.
