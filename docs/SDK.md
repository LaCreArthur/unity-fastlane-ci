# Sorolla Palette SDK - Context for Claude

Purpose of this file: quick orientation for AI agents working on the `sdk-dev` branch, where this Unity project hosts both the CI template AND the Sorolla Palette SDK under development. `master` is the clean CI reference; `sdk-dev` is where SDK code lives.

## Layout

| Path | What it is |
|---|---|
| `Packages/com.sorolla.sdk/` | The SDK package itself. **Separate git repository** (gitignored here). Has its own `CLAUDE.md` with commit rules. |
| `Packages/com.sorolla.sdk/Documentation~/` | SDK user documentation |
| `Assets/DebugUI` -> `Packages/com.sorolla.sdk/Samples~/DebugUI` | Symlink; DebugUI sample scripts live in the SDK repo |
| `Assets/Sorolla.link.xml` | IL2CPP link preservation |

## SDK basics

- **Repo**: https://github.com/sorolla-studio/sorolla-palette
- **Namespace**: `Sorolla.Palette`
- **Entry point**: `Palette.Initialize(consent)`
- **Modes**:
  - **Prototype**: GameAnalytics + Facebook SDK + Firebase (+ optional MAX)
  - **Full**: GameAnalytics + MAX + Adjust + Firebase

## Stub + Implementation pattern

Optional SDKs (MAX, Adjust) may not be installed. To keep Prototype mode compiling without them:

- `Sorolla.Adapters.asmdef` - stub classes, always compile
- `Sorolla.Adapters.MAX.asmdef` - real impl, `defineConstraints: APPLOVIN_MAX_INSTALLED`
- `Sorolla.Adapters.Adjust.asmdef` - real impl, `defineConstraints: ADJUST_INSTALLED`

Unity resolves assembly references BEFORE `#if` blocks, so without this split, a missing MAX/Adjust would break the build in Prototype mode.

## Committing to the SDK subrepo

The `Packages/com.sorolla.sdk/` directory is a separate git repo.

```bash
cd Packages/com.sorolla.sdk
# Stage by explicit filename - NEVER git add -A or git add .
git add Runtime/Foo.cs Runtime/Foo.cs.meta
git commit -m "..."
```

## Committing to this (outer) repo on sdk-dev

The outer repo tracks project-level changes (Assets, ProjectSettings, workflows, manifest.json). `Packages/com.sorolla.sdk/` is gitignored here - changes inside it must be committed in the SDK's own repo, not this one.

## Strict SDK rules (from SDK CLAUDE.md - preserved here for context)

1. **NEVER null-check `[SerializeField]`** - trust Inspector; crash on null reveals missing refs.
2. **Subscribe methods directly to events** (`Event += Method`), no wrapper methods.
3. **Check existing SDK APIs first** before writing JNI/Obj-C (e.g. `Adjust.GetGoogleAdId()` exists, do not reimplement).
4. **`UNITY_IOS` is defined in the Editor when Target = iOS.** Always gate native iOS calls with `#if !UNITY_EDITOR`.
5. **Manifest.json is the source of truth** for SDK mode. Assembly detection is unreliable during domain reloads.
6. **Only make requested changes.** Don't over-engineer or expand scope.
7. **Read files before editing.** No speculation.

## When working here

- **Gameplay / game-level code** changes belong in a game repo, not this template branch.
- **CI / fastlane / workflow** changes should match `master` - cherry-pick between branches.
- **SDK functional changes** go in `Packages/com.sorolla.sdk/` (separate repo).
- **SDK integration surface** (`Assets/Resources/SorollaConfig.asset`, `manifest.json`) changes go in this outer repo on `sdk-dev`.

## Other integrated SDKs (when Sorolla Palette is in Full mode)

Firebase (Analytics, Crashlytics, Remote Config), GameAnalytics, AppLovin MAX, Adjust, Facebook SDK. Each has its own dependency resolution via Unity External Dependency Manager.

## iOS development gotchas

- CocoaPods: install via Homebrew, not system Ruby 2.6.
- Platform toggle (iOS -> Android -> iOS) forces PATH reload in Unity, useful when iOS Resolver can't find pod.
- ATT prompt: native-only; wrap with `#if !UNITY_EDITOR` and use the iOS Support package.
