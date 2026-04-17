# CLAUDE.md

Guidance for Claude Code working in this repo.

**Canonical agent instructions**: [AGENTS.md](AGENTS.md) - read this first, it covers purpose, golden rules, file map, dependency policy, and known hazards.

**Bootstrapping a new game from this template**: [BOOTSTRAP.md](BOOTSTRAP.md).

**Unity coding guidelines (optional)**: [docs/ai-guidelines/unity-development.md](docs/ai-guidelines/unity-development.md) - only relevant if extending with gameplay code.

## Repo in one line

Reference Unity Fastlane CI template. Unity 6 LTS + GameCI for builds + Fastlane for Firebase/Play/TestFlight distribution. Designed so an agent can fork, follow BOOTSTRAP.md, and ship.

## Build & distribution commands

```bash
bundle install  # first time

bundle exec fastlane android distribute notes:"Build description"
bundle exec fastlane ios distribute notes:"Build description"

# Store upload
bundle exec fastlane android distribute upload_to_store:true track:internal
bundle exec fastlane ios distribute upload_to_store:true
```

CI triggers: push/PR to `master` builds both platforms; push to `master` also uploads to stores.

## Branches

Single branch: `master`. This is a reference template; no gameplay or SDK-specific branches.
