# Engine + bridge pin (D5)

**Status:** Pin ready Â· **Updated:** 2026-08-07  
**Upstream:** [HolagundiWorks/AQC](https://github.com/HolagundiWorks/AQC)  
**Baseline:** tag `aorms-bridge-d2`

Same pin as AStudio â€” share `bbs_engine` + `Aorms.Bridge`; specialize engineering domain UI only.

| Artifact | Consume from |
| --- | --- |
| C++ `bbs_engine` | AQC â€” single SoT at tag `aorms-bridge-d2` |
| `Aorms.Bridge` | AQC `BBSDesktop/Aorms.Bridge` |
| Wire | esti PORTAL-SYNC-BRIDGE Â· HUB-API `2026-08` |

```bash
git submodule add https://github.com/HolagundiWorks/AQC.git vendor/AQC
cd vendor/AQC && git checkout aorms-bridge-d2
```

Open source; SaaS licensing deferred.
