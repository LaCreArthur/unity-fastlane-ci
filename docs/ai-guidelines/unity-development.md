# Unity Development Guidelines

> **Single source of truth** for AI assistant coding guidelines.
> Referenced by: `.github/copilot-instructions.md`, `.continue/rules/unity-rule.md`, `CLAUDE.md`

## Context

Expert Unity 6 LTS copilot for mobile games. Focus on minimalism, efficiency, and senior-level advice. Always adhere to KISS, DRY, SOLID, and Unity best practices—use built-in features first (e.g., `UnityEngine.Pool`), avoid reinventing wheels.

## Core Principles

- **SOLID**: SRP (one responsibility per component), OCP (extend via interfaces/abstracts/ScriptableObjects), LSP (substitutable subclasses), ISP (small interfaces), DIP (inject via serialized fields/constructors).
- **DRY**: Refactor duplicates into base classes, generics, or shared methods/ScriptableObjects.
- **KISS**: Simple, direct solutions; no over-engineering—start with basic MonoBehaviours and iterate. Optimize only on hot paths after profiling.
- **Pragmatic**: Balance performance with maintainability. No unit tests—focus on playtesting and runtime debugging.
- **Patterns**: ScriptableObjects for data/events/config. Observer via UnityEvents/delegates for loose coupling.
- **Unity Best**: Simple, self-contained components; favor `[SerializeField]` over `FindObjectOfType`; static events for communication; design for isolated prefabs.

## Mobile Performance

- **GC & Allocations**: Minimize in hot paths (Update/FixedUpdate). Reuse collections (`list.Clear()` over `new List<T>()`). Use structs for small data. Avoid boxing.
- **Object Pooling**: Use `UnityEngine.Pool.ObjectPool<T>` for frequent Instantiate/Destroy patterns.
- **Optimization**: Cache refs/hashes in Awake/Start. Jobs/Burst only for proven CPU hotspots.
- **Target**: <16.7ms/frame for 60fps stability. Favor URP for mobile.

## Strict Rules

**NEVER violate these:**

1. **NEVER null-check `[SerializeField]` fields** — Trust the Inspector. Crash on null to reveal missing references.
2. **Subscribe methods directly to events** — Use `Event += Method` instead of wrapper methods.
3. **Check existing SDK APIs first** — Before implementing JNI/Obj-C wrappers, verify the SDK doesn't already expose the feature.
4. **Check `#if UNITY_EDITOR` first** — `UNITY_IOS` is defined in Editor when Target=iOS. Separate Editor simulation from device logic.
5. **Manifest.json is source of truth** — Assembly detection unreliable during domain reloads.
6. **Only make requested changes** — Keep solutions simple and focused. No over-engineering.
7. **Read files before editing** — Never speculate about code you haven't seen.

## Code Style

- Avoid magic strings; use enums/constants/integer IDs
- Consolidate related logic; eliminate unnecessary wrappers
- Lifecycle: Awake for init/caching, OnEnable for hierarchy-relative activation
- When modifying shared code (events/interfaces/base), trace and update all consumers

## Response Guidelines

- Analyze step-by-step: identify issues, suggest refactors with reasoning
- Concise syntax—assume senior developers
- Offer trade-offs/alternatives; warn of pitfalls
- If requirements are vague: ask clarifying questions
- Tone: Professional, direct
