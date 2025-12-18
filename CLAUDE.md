# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

> **Coding guidelines**: See [docs/ai-guidelines/unity-development.md](docs/ai-guidelines/unity-development.md)

## Project Overview

Unity 6 mobile game project with automated CI/CD for Android (AAB) and iOS (IPA). Uses GameCI for builds and Fastlane for distribution to Firebase App Distribution and stores.

**Unity Version**: 6000.3.2f1 (LTS)
**Targets**: Android, iOS

## Build & Distribution Commands

```bash
# Install Ruby dependencies
bundle install

# Local distribution to Firebase
bundle exec fastlane android distribute notes:"Build description"
bundle exec fastlane ios distribute notes:"Build description"

# With store upload
bundle exec fastlane android distribute upload_to_store:true track:internal
bundle exec fastlane ios distribute upload_to_store:true
```

**CI Triggers**: Push/PR to `master` builds both platforms. Push to `master` also uploads to stores.

## Architecture

### Sorolla SDK (`Packages/com.sorolla.sdk`)
Zero-configuration mobile publisher SDK that auto-initializes on app start via `SorollaBootstrapper`. Key patterns:
- **Adapter Pattern**: Separate adapters for Firebase, MAX, Adjust, GameAnalytics, Facebook
- **Observer Pattern**: `SorollaSDK.OnInitialized` event for components that need SDK state
- **Config Pattern**: `SorollaConfig` ScriptableObject for SDK settings

**Two Modes**:
- **Prototype**: GameAnalytics + Facebook SDK + optional MAX
- **Full**: GameAnalytics + MAX + Adjust

### CI/CD Pipeline
- `.github/workflows/android-build.yml` - GameCI build -> Fastlane distribute -> Play Store
- `.github/workflows/ios-build.yml` - Two jobs: GameCI builds Xcode project (Ubuntu) -> Sign & deploy (macOS)
- `fastlane/Fastfile` - Android/iOS lanes for Firebase App Distribution and store uploads

## Key Files

| File | Purpose |
|------|---------|
| `Packages/com.sorolla.sdk/Runtime/SorollaSDK.cs` | Main SDK entry point |
| `Packages/com.sorolla.sdk/Runtime/SorollaBootstrapper.cs` | Auto-initialization |
| `Assets/Resources/SorollaConfig.asset` | SDK configuration (Prototype/Full mode) |
| `Assets/Scripts/Editor/AndroidAutoKey.cs` | Keystore auto-configuration |
| `docs/development/memory.md` | Development session logs |

## iOS-Specific Notes

- CocoaPods: Use Homebrew installation, not system Ruby
- ATT prompt: Platform toggle (iOS->Android->iOS) forces PATH reload in Unity
- Provisioning profile name must match exactly (including spaces)

## Mobile SDKs Integrated

Firebase (Analytics, Crashlytics, Remote Config), GameAnalytics, AppLovin MAX, Adjust, Facebook SDK
