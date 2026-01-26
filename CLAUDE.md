# CLAUDE.md

**Navigation:** See `.claude/INDEX.md` for keywords and file routing.

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

**Always invoke the `developing-unity-games` skill when working in this repo.**

## Project Overview

This is a **Unity 6 LTS mobile game project** with two main components:
1. **Sorolla SDK** (`Packages/com.sorolla.sdk/`) - A plug-and-play mobile publisher SDK
2. **CI/CD Pipeline** - GameCI + Fastlane for automated builds and distribution

### Branch Structure

This repository serves two purposes depending on the branch:

- **`sdk-dev`**: SDK development environment - minimal game code, focus on `Packages/com.sorolla.sdk/`
- **`master`**: Production template - complete Unity project with GameCI + Fastlane CI/CD setup

When working on SDK features, use `sdk-dev` branch. The SDK package is gitignored here but has its own repository at https://github.com/sorolla-studio/sorolla-palette

### Important: SDK is a Separate Git Repository
The `Packages/com.sorolla.sdk/` folder is gitignored in this project but contains its own `.git` repository. When making SDK changes:
```bash
cd Packages/com.sorolla.sdk && git add . && git commit -m "message"
```

## Build & Distribution Commands

### Local Testing
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

### CI/CD Pipeline
Builds run automatically on push/PR to `master`, or manually via GitHub Actions:
- **Workflow dispatch**: Allows triggering builds with optional `upload_to_store` parameter
- **Secrets**: All required secrets documented in `docs/SETUP_GUIDE.md` and `README.md`
- **Artifacts**: AAB/IPA uploaded to Firebase App Distribution and optionally to stores

## Sorolla SDK Architecture

### Two Operating Modes
- **Prototype**: GameAnalytics + Facebook + Firebase (rapid UA testing)
- **Full**: GameAnalytics + Facebook + MAX + Adjust + Firebase (production)

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

## Game Scripts Structure

```
Assets/
├── Scripts/
│   ├── AdjustTestController.cs       # SDK testing controller
│   └── Editor/
│       └── AndroidAutoKey.cs          # Build automation
├── DebugUI/                           # Symlink → Packages/com.sorolla.sdk/Samples~/DebugUI
├── Resources/                         # Game resources
└── GoogleService-Info.plist           # Firebase iOS config
```

Game-specific code uses the SDK via `Sorolla.Palette` namespace. The DebugUI is symlinked from the SDK samples for rapid testing.

## CI/CD Structure

```
.github/workflows/
├── android-build.yml    # GameCI → AAB → Firebase/Play Store
└── ios-build.yml        # GameCI → IPA → Firebase/TestFlight

fastlane/
├── Fastfile             # distribute lanes for android/ios
└── Pluginfile           # firebase_app_distribution plugin
```

The Fastfile handles both Firebase App Distribution and store uploads. It reads release notes from:
1. Provided `notes:` parameter
2. `notes_path:` file (relative to project root)
3. Default fallback message

Required secrets documented in `docs/SETUP_GUIDE.md` and `README.md`.

## SDK Internal Documentation

The SDK has comprehensive documentation at `Packages/com.sorolla.sdk/`:
- **DEVLOG.md**: Critical validated learnings (read this first!) - Unity asmdef patterns, IL2CPP stripping, EDM4U Gradle issues
- **Documentation~/internal/architecture.md**: Complete technical architecture reference
- **Documentation~/internal/ai-agents.md**: AI agent collaboration guidelines
- **CLAUDE.md**: SDK-specific guidance for Claude Code sessions

When working on SDK code:
1. Check `DEVLOG.md` first for critical learnings about Unity quirks
2. Reference `architecture.md` for structural decisions
3. Session-specific planning docs should be cleaned up when abandoned
