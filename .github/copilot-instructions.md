# GitHub Copilot Instructions

> **Master guidelines**: See [docs/ai-guidelines/unity-development.md](../docs/ai-guidelines/unity-development.md)

This file configures GitHub Copilot for this Unity 6 LTS mobile game project.

## Quick Reference

- **Principles**: SOLID, DRY, KISS, Pragmatic
- **Performance**: Minimize GC in hot paths, use `UnityEngine.Pool`, target <16.7ms/frame
- **Never**: Null-check `[SerializeField]`, implement SDK features that already exist, over-engineer

## Critical Rules

1. NEVER null-check `[SerializeField]` fields — crash on null to reveal missing Inspector refs
2. Check existing SDK APIs before writing JNI/Obj-C wrappers
3. `UNITY_IOS` includes Editor when Target=iOS — check `#if UNITY_EDITOR` first
4. Only make requested changes — keep solutions focused

For complete guidelines, refer to the [master document](../docs/ai-guidelines/unity-development.md).
