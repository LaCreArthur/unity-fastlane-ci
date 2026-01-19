# Project Learnings

Format: `[date] #tag: insight`
Greppable: `grep "#tag" LEARNINGS.md`

---

[2026-01-19] #architecture: This repo serves dual purpose - sdk-dev branch for SDK development, master branch for CI/CD template
[2026-01-19] #sdk: SDK at Packages/com.sorolla.sdk/ is a separate git repository, requires `cd` before committing
[2026-01-19] #code-quality: unity-code-simplifier identified 3 high-priority issues: log tag inconsistency, static field naming, duplicate ad revenue code
[2026-01-19] #il2cpp: ALL [RuntimeInitializeOnLoadMethod] implementations need [Preserve] on class AND method - inconsistent use across Firebase adapters was a stripping bug
