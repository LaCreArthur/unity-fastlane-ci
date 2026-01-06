# CLAUDE.md

**Always invoke the `developing-unity-games` skill when working in this repo.**

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Unity 6 LTS mobile game project** with two main components:
1. **Sorolla SDK** (`Packages/com.sorolla.sdk/`) - A plug-and-play mobile publisher SDK
2. **CI/CD Pipeline** - GameCI + Fastlane for automated builds and distribution

### Important: SDK is a Separate Git Repository
The `Packages/com.sorolla.sdk/` folder is gitignored in this project but contains its own `.git` repository. When making SDK changes:
```bash
cd Packages/com.sorolla.sdk && git add . && git commit -m "message"
```
The SDK repo is hosted at: https://github.com/LaCreArthur/sorolla-palette-upm

## Build & Distribution Commands

```bash
# Install Ruby dependencies (first time)
bundle install

# Android distribution to Firebase
bundle exec fastlane android distribute notes:"Build description"

# iOS distribution to Firebase
bundle exec fastlane ios distribute notes:"Build description"

# With store upload
bundle exec fastlane android distribute upload_to_store:true track:internal
bundle exec fastlane ios distribute upload_to_store:true
```

## Sorolla SDK Architecture

### Two Operating Modes
- **Prototype**: GameAnalytics only (rapid UA testing)
- **Full**: GameAnalytics + MAX + Adjust (production)

### Assembly Structure (Stub + Implementation Pattern)
The SDK uses separate assemblies to allow compilation without external SDK dependencies:

```
Runtime/
├── Sorolla.Runtime.asmdef          # Core SDK (Palette.cs, SorollaBootstrapper.cs)
└── Adapters/
    ├── Sorolla.Adapters.asmdef     # Stubs - no external refs, always compiles
    ├── MaxAdapter.cs               # Stub → delegates to IMaxAdapter impl
    ├── AdjustAdapter.cs            # Stub → delegates to IAdjustAdapter impl
    ├── FirebaseAdapter.cs          # Stub → delegates to IFirebaseAdapter impl
    ├── MAX/
    │   └── Sorolla.Adapters.MAX.asmdef   # defineConstraints: APPLOVIN_MAX_INSTALLED
    ├── Adjust/
    │   └── Sorolla.Adapters.Adjust.asmdef
    └── Firebase/
        └── Sorolla.Adapters.Firebase.asmdef
```

**Why this pattern?** Unity resolves assembly references before evaluating `#if` preprocessor blocks. This ensures the SDK compiles cleanly in Prototype mode without MAX/Adjust installed.

### Key Preprocessor Defines (auto-set via versionDefines)
- `GAMEANALYTICS_INSTALLED` - GameAnalytics SDK present
- `SOROLLA_MAX_ENABLED` / `APPLOVIN_MAX_INSTALLED` - AppLovin MAX
- `SOROLLA_ADJUST_ENABLED` / `ADJUST_SDK_INSTALLED` - Adjust
- `FIREBASE_ANALYTICS_INSTALLED`, `FIREBASE_CRASHLYTICS_INSTALLED`, `FIREBASE_REMOTE_CONFIG_INSTALLED`

### Initialization Flow
1. `SorollaBootstrapper` auto-creates via `[RuntimeInitializeOnLoadMethod]`
2. On iOS: Shows ATT context screen, waits for user decision
3. Calls `Palette.Initialize(consent)` (namespace: `Sorolla.Palette`)
4. Initializes SDKs based on mode and installed packages

## Code Style (from .github/copilot-instructions.md)

- **SOLID/DRY/KISS**: Simple, direct solutions; no over-engineering
- **Never null-check `[SerializeField]`** - trust Inspector, crash on null is desired
- **Avoid magic strings** - use enums/constants/integer IDs
- **ScriptableObjects** for data/events/config
- **Static events** for communication, avoid god managers
- **Favor `[SerializeField]`** over `FindObjectOfType`
- **Mobile**: Minimize GC in hot paths, use `UnityEngine.Pool.ObjectPool<T>` for pooling
- **No unit tests** - focus on playtesting and runtime debugging

## CI/CD Structure

```
.github/workflows/
├── android-build.yml    # GameCI → AAB → Firebase/Play Store
└── ios-build.yml        # GameCI → IPA → Firebase/TestFlight

fastlane/
├── Fastfile             # distribute lanes for android/ios
└── Pluginfile           # firebase_app_distribution plugin
```

Required secrets documented in `docs/SETUP_GUIDE.md` and `README.md`.

## SDK Internal Documentation

The SDK has internal docs at `Packages/com.sorolla.sdk/Documentation~/internal/`. Key maintenance notes:
- **Task-tracking files rot quickly** - Prefer `devlog.md` (validated learnings) over sprint-style task lists
- **Session-specific planning docs** should be cleaned up when sessions are abandoned
- **Check `devlog.md` first** for critical learnings about Unity asmdef patterns, IL2CPP stripping, etc.
