# AGENTS.md

Canonical instructions for AI agents (Claude Code, Codex, Cursor, Copilot) working on this repo. Vendor-neutral - vendor-specific files (`CLAUDE.md`, `.cursor/rules/*`, `.github/copilot-instructions.md`) should point here rather than duplicate.

## Repo purpose

**Reference Unity Fastlane CI template.** Starting point for new Unity 6 mobile games. Not a product. Keep surface area minimal; add only what every Unity game needs.

Goal: a developer (or their agent) clones/forks this, runs through [BOOTSTRAP.md](BOOTSTRAP.md), and gets working Android + iOS CI with Firebase + store distribution inside an hour.

## Golden rules

1. **Bootstrapping is the primary user flow.** Every change should preserve the "fork -> follow BOOTSTRAP.md -> ship" path. Don't break it for cleverness.
2. **No game code.** Gameplay, SDK-specific shims, sample scenes beyond a minimal default - these belong in a game repo, not here.
3. **Pin dependencies explicitly.** Floating tags (`@main`, `latest-stable`) are supply-chain + reproducibility risks. Use versioned tags; pin SHAs when the source repo is small or high-risk.
4. **Secrets never hit disk in cleartext outside CI runners.** Local env vars only. Base64 in GitHub Secrets; decoded in-workflow.
5. **Keep docs dated.** [SETUP_GUIDE.md](docs/SETUP_GUIDE.md) has "last updated" - bump it when touching platform policy content (Play/App Store rules change yearly).
6. **Verify before claiming done.** Workflow changes require a test run. Don't mark CI work complete based on YAML diff alone.

## File map (what lives where)

| Path | Purpose | Touch when |
|---|---|---|
| `.github/workflows/android-build.yml` | Android GameCI build + Fastlane distribute | Unity version bump, action version bump, Play policy change |
| `.github/workflows/ios-build.yml` | iOS two-job build (Ubuntu build, macOS sign+deploy) | Xcode pin, cert/profile flow, Apple policy change |
| `.github/workflows/claude*.yml` | Optional Claude Code review/mention integration | Only if Anthropic releases action updates |
| `fastlane/Fastfile` | Android + iOS `distribute` lanes | Release-notes format, store options |
| `fastlane/Pluginfile` | Fastlane plugins (`firebase_app_distribution`) | New plugin added |
| `Gemfile` | Ruby deps - single `gem "fastlane"` | Rarely |
| `docs/SETUP_GUIDE.md` | Human setup walkthrough | Platform policy changes, secret additions |
| `docs/ai-guidelines/unity-development.md` | Optional Unity coding guidelines (for extending this template with gameplay) | Unity-specific rules only |
| `AGENTS.md` | THIS FILE | Agent guidance changes |
| `BOOTSTRAP.md` | Agent-executable checklist | Secret list, placeholder set changes |
| `CLAUDE.md` | Claude Code pointer | Minor; points here |
| `README.md` | Overview + stack table | Stack version bumps |

## Dependency policy

- **Review quarterly** or when a workflow run fails for dep-related reasons.
- **Bump aggressively** on majors when the upstream has migration docs + runner support.
- **Pin explicit versions** for:
  - GitHub Actions: `@vN` tags (users of template may pin SHAs per their own security bar)
  - `jlumbroso/free-disk-space`: tag pin minimum, SHA preferred
  - Xcode: exact minor (`16.4`) - `latest-stable` has caused silent TestFlight upload failures
  - Unity: exact patch (`6000.3.2f1`) - GameCI resolves docker image per patch

See README "Stack" table for current pinned versions.

## Known hazards (Apr 2026)

- **Xcode 26 TestFlight silent drop** (fastlane #29743) - pin `16.4` until fixed.
- **Play targetSdkVersion 36** required Aug 31, 2026 for new apps + updates. Document in SETUP_GUIDE when shipping.
- **Unity 6.0 LTS EOL Oct 2026.** Prefer 6.3 LTS for new starts.
- **Ruby 3.4.8 on macOS-14 runner** - native extension compile breakage. Stay on 3.3.
- **Provisioning profile name is case-sensitive and includes spaces.** Users lose hours here.

## Common agent tasks

### Bump Unity version
1. Update `UNITY_VERSION` env in both `android-build.yml` and `ios-build.yml`.
2. Update `ProjectSettings/ProjectVersion.txt` (Unity writes this).
3. Re-run license activation if needed (see SETUP_GUIDE -> Unity License).
4. Update `README.md` stack table.
5. Update `docs/SETUP_GUIDE.md` example value.
6. Test both workflows on a branch before merging.

### Add a secret
1. Add to the secret list in `README.md` + `docs/SETUP_GUIDE.md` "Complete Secrets Checklist".
2. Wire into workflow `env:` block.
3. Add to `BOOTSTRAP.md` placeholder table.
4. Never echo the secret value in workflow logs - use `::add-mask::` if derived at runtime.

### Adapt this template to a new game (bootstrap)
Follow [BOOTSTRAP.md](BOOTSTRAP.md). Do not skip validation steps - "looks right" is not "works".

### Review a change
- Workflow YAML: lint with `actionlint` mentally (common errors: missing `uses:`, wrong indentation, undefined secrets).
- Fastfile: sanity-check that new calls match the firebase_app_distribution + google-play + testflight actions' current parameter names.
- Secrets: never let a secret name leak into a plaintext file, even docs.

## What NOT to do

- **Don't add gameplay code** (scenes, scripts, prefabs) to this template. Fork a separate game repo.
- **Don't vendor `Gemfile.lock`.** It's gitignored for a reason - the lock depends on host platform.
- **Don't commit `*.p8`, `*.p12`, `*.mobileprovision`, `google-services.json`** under `Assets/` - already gitignored.
- **Don't commit `Library/`, `obj/`, `*.csproj`, `*.sln`.** Unity regenerates.
- **Don't expand scope.** Game code belongs in the repo that uses this template, not here.

## Vendor notes

- **Claude Code**: `CLAUDE.md` is the entry point. It should be thin and reference this file.
- **Codex**: reads `AGENTS.md` natively.
- **Cursor**: create `.cursor/rules/*.mdc` files that reference this file if desired; don't duplicate content.
- **Copilot (GitHub)**: if adding `.github/copilot-instructions.md`, have it point here - don't duplicate.

## When things are ambiguous

Ask once, concisely. Default to minimalism. If the change is reversible and low-blast-radius, make the call and note the reasoning in the commit.
