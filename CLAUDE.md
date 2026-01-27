# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

**Navigation:** See `.claude/INDEX.md` for keywords and file routing.

**Always invoke the `developing-unity-games` skill when working in this repo.**

## Project Overview

**Unity 6 LTS (6000.0.62f1)** mobile game template with:
1. **Sorolla SDK** (`Packages/com.sorolla.sdk/`) - Mobile publisher SDK (separate git repo)
2. **CI/CD Pipeline** - GameCI + Fastlane for automated builds and distribution

### Branch Structure
- **`sdk-dev`**: SDK development - minimal game code, focus on SDK package
- **`master`**: Production template - full CI/CD setup

### SDK is a Separate Repository
The SDK folder is gitignored here but has its own `.git`. Commits:
```bash
cd Packages/com.sorolla.sdk && git add . && git commit -m "message"
```
Repo: https://github.com/sorolla-studio/sorolla-palette

## Build & Distribution

### Local Fastlane (requires pre-built artifacts)
```bash
bundle install  # First time only

# Required env vars for local runs:
export FIREBASE_APP_ID_ANDROID="1:xxx:android:xxx"
export ANDROID_AAB_PATH="./build/Android.aab"
# For iOS: FIREBASE_APP_ID_IOS, IOS_IPA_PATH

# Distribute to Firebase
bundle exec fastlane android distribute notes:"Build description"
bundle exec fastlane ios distribute notes:"Build description"

# With store upload (needs GPLAY_SERVICE_JSON, ANDROID_PACKAGE_NAME for Android)
bundle exec fastlane android distribute upload_to_store:true track:internal
bundle exec fastlane ios distribute upload_to_store:true
```

### CI/CD (GitHub Actions)
- **Trigger**: Push/PR to `master`, or manual workflow dispatch
- **Outputs**: AAB/IPA → Firebase App Distribution, optionally to stores
- **Secrets**: See `docs/SETUP_GUIDE.md` for full list

## Sorolla SDK

For SDK-specific development, see `Packages/com.sorolla.sdk/CLAUDE.md`.

### Quick Reference
- **Modes**: Prototype (GA + FB + Firebase) | Full (+ MAX + Adjust)
- **API**: `Sorolla.Palette` namespace, `Palette.Initialize(consent)`
- **Context**: Read SDK's `.claude/INDEX.md` first, then drill into specific docs as needed

### Stub + Implementation Pattern
SDK uses separate assemblies so optional SDKs don't break compilation:
- `Sorolla.Adapters.asmdef` - Stubs (always compile)
- `Sorolla.Adapters.MAX.asmdef` - Implementation with `defineConstraints: APPLOVIN_MAX_INSTALLED`

Unity resolves assembly references *before* `#if` blocks - this pattern ensures Prototype mode compiles without MAX/Adjust installed.

## Code Style

From `.github/copilot-instructions.md`:
- **SOLID/DRY/KISS** - Simple solutions, no over-engineering
- **Never null-check `[SerializeField]`** - Trust Inspector, crash on null is desired
- **Avoid magic strings** - Use enums/constants/integer IDs
- **ScriptableObjects** for data/events/config
- **Static events** for communication, avoid god managers
- **Mobile**: Minimize GC in hot paths, use `UnityEngine.Pool.ObjectPool<T>`
- **No unit tests** - Focus on playtesting and runtime debugging
