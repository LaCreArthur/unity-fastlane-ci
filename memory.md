# Agent Memory Log

## Entry Format
Each entry records a problem, solution, and insight for future reference. Focus on reproducibility and learnings.

```
### [YYYY-MM-DD HH:MM] Title
- **Problem**: What issue was encountered
- **Solution**: How it was resolved
- **Insight**: Key learning or rule to remember
```

---

## User Preferences & Rules

> **CRITICAL RULES** — Never violate these:
> - **NEVER null-check `[SerializeField]` fields.** Trust the Inspector; if null, let it crash to reveal the missing reference.
> - **Subscribe methods directly to events** (e.g., `Event += Method`) instead of creating wrapper methods that just call them.
> - **Check existing SDK APIs first** before implementing custom native wrappers (JNI/Obj-C).
> - **UNITY_IOS is defined in Editor** when Target=iOS. Always check `#if UNITY_EDITOR` first to separate Editor simulation from device logic.

---

## Session Logs

### [2025-12-15 00:00] DebugUI Script Reorganization
- **Problem**: Reorganize DebugUI scripts without breaking prefab/Inspector refs.
- **Solution**: Moved scripts with their .meta files (preserves GUIDs). Validated Assets/Samples symlink.
- **Insight**: Unity prefab refs depend on meta GUIDs. Keep prefab-referenced scripts in sample folder.

### [2025-12-15 13:50] SerializeField Null-Check Pattern
- **Problem**: User wanted optional refresh button with visibility toggle.
- **Solution**: Use `showRefreshButton` bool + `SetActive()` instead of null-checking.
- **Insight**: **Strict no null-check rule**: always reference UI elements, use bool for visibility.

### [2025-12-15 14:36] Android AppUIGameActivity Error
- **Problem**: `ClassNotFoundException: com.unity3d.player.appui.AppUIGameActivity`.
- **Solution**: Changed manifest to `UnityPlayerGameActivity`. Removed stale App UI ref from EditorBuildSettings.
- **Insight**: Check EditorBuildSettings.m_configObjects for stale package refs.

### [2025-12-15 16:03] Adjust Initialization Race Condition
- **Problem**: 'SDK needs to be initialized' warning when fetching ADID.
- **Solution**: Added delay or wait for IsInitialized signal. Created DebugUI asmdef with versionDefines.
- **Insight**: Native SDK init is async. DebugUI Samples~ need own .asmdef with defines.

### [2025-12-15 16:39] Advertising ID Implementation
- **Problem**: Implemented custom JNI wrapper; user pointed out SDK already has it.
- **Solution**: Replaced with `Adjust.GetGoogleAdId()` via AdjustAdapter.
- **Insight**: **CRITICAL**: Check existing SDK APIs before building native wrappers.

### [2025-12-16 10:36] iOS Ruby/CocoaPods Error
- **Problem**: iOS Resolver fails with system Ruby 2.6 missing headers.
- **Solution**: Install CocoaPods via Homebrew. Platform switch (iOS→Android→iOS) forces PATH reload.
- **Insight**: Unity iOS Resolver defaults to system Ruby. Use Homebrew. Platform toggle forces env reload.

### [2025-12-16 10:45] iOS ATT Silent Fail in Editor
- **Problem**: ATT prompt did nothing in Editor.
- **Solution**: Check `!UNITY_EDITOR` before calling native iOS bindings.
- **Insight**: **UNITY_IOS includes Editor when Target=iOS.** Check UNITY_EDITOR first.

### [2025-12-16 14:38] Debug UI Mode Detection
- **Problem**: Debug UI showed Prototype mode when SDK was in Full mode.
- **Solution**: Added `SorollaSDK.OnInitialized` event. Used observer pattern in UI components.
- **Insight**: Components in `Start()` may run before SDK init. Use observer pattern + unsubscribe in OnDestroy.

### [2025-12-16 15:06] MAX SDK Consent
- **Problem**: MAX showed "Has User Consent - No value set" despite ATT approved.
- **Solution**: Added `MaxSdk.SetHasUserConsent(consent)` in MaxAdapter.OnSdkInit.
- **Insight**: MAX requires explicit SetHasUserConsent call. ATT reset requires app reinstall.

### [2025-12-16 15:13] Sorolla → SorollaSDK Rename
- **Problem**: Namespace `Sorolla` with class `Sorolla` caused ambiguity.
- **Solution**: Renamed class to `SorollaSDK`. Passed consent as parameter to avoid circular asmdef refs.
- **Insight**: Avoid class name = namespace name. Circular asmdef refs → pass values as parameters.

### [2025-12-16 15:23] Privacy UI Refactor
- **Problem**: Privacy buttons only work in Editor; useless on device builds.
- **Solution**: Editor: show test buttons. Builds: show ATT status. Added Open Settings button.
- **Insight**: iOS: `app-settings:` URL. Android: `APPLICATION_DETAILS_SETTINGS` intent.

### [2025-12-16 15:53] iOS Support Package Reference
- **Problem**: `Unity.Advertisement.IosSupport` not found in DebugUI assembly.
- **Solution**: Added package ref to asmdef + `UNITY_IOS_SUPPORT_INSTALLED` versionDefine. Wrapped code with define.
- **Insight**: Optional packages in separate assemblies need: asmdef reference + versionDefine + wrapped code.
