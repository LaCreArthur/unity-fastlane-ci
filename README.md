# Unity Game CI with Fastlane

Reference CI/CD template for Unity 6 mobile games. Automated Android (AAB) + iOS (IPA) builds via [GameCI](https://game.ci/), distribution to Firebase App Distribution, Google Play, and TestFlight via [Fastlane](https://fastlane.tools/).

Designed in 2026 to be bootstrapped by an AI coding agent (Claude Code, Codex, Cursor, Copilot). See [AGENTS.md](AGENTS.md) + [BOOTSTRAP.md](BOOTSTRAP.md).

## Features

- Android AAB + iOS IPA builds on GitHub Actions
- Firebase App Distribution for beta testing
- Google Play Store + App Store Connect / TestFlight upload
- Semantic versioning with automatic Android version code (GameCI)
- Release notes generated from git commits
- Manual `workflow_dispatch` with optional store upload toggle
- Claude Code PR review + `@claude` mention workflows (opt-in)

## Stack (April 2026)

| Component | Version | Notes |
|---|---|---|
| Unity | 6000.3.x LTS | 6.0 LTS EOL Oct 2026 - prefer 6.3 LTS |
| GameCI `unity-builder` | `@v4` | latest major |
| fastlane gem | latest | installed via `bundle install` |
| Ruby | 3.3 | `ruby/setup-ruby@v1` |
| GHA actions | `checkout@v5`, `cache@v5`, `upload-artifact@v5`, `download-artifact@v5` | Node 24 |
| `apple-actions/import-codesign-certs` | `@v6` | Node 24, `productsign` support |
| Xcode (iOS runner) | pinned `16.4` | Xcode 26 regressed silent TestFlight uploads (fastlane #29743) |

## Quick Start

**Human path**: Click "Use this template", then follow [docs/SETUP_GUIDE.md](docs/SETUP_GUIDE.md).

**Agent path**: Clone, then point your agent at [BOOTSTRAP.md](BOOTSTRAP.md) - placeholder table, secret checklist, validation steps formatted for an agent to execute.

## Project Structure

```
├── .github/workflows/         # android-build, ios-build, claude*
├── fastlane/
│   ├── Fastfile               # Android/iOS distribute lanes
│   └── Pluginfile             # firebase_app_distribution plugin
├── docs/
│   ├── SETUP_GUIDE.md         # full setup walkthrough
│   └── ai-guidelines/         # optional Unity coding guidelines
├── AGENTS.md                  # canonical agent instructions
├── BOOTSTRAP.md               # agent-executable adapt checklist
├── CLAUDE.md                  # Claude Code project instructions
└── Gemfile                    # Ruby / fastlane deps
```

## Local Development

```bash
bundle install  # first time only

# Env vars needed locally (CI sets these from secrets):
export FIREBASE_APP_ID_ANDROID="1:xxx:android:xxx"
export ANDROID_AAB_PATH="./build/Android/Android.aab"
export FIREBASE_APP_ID_IOS="1:xxx:ios:xxx"
export IOS_IPA_PATH="./build/ipa/YourApp.ipa"

# Build AAB/IPA from Unity first, then:
bundle exec fastlane android distribute notes:"Build description"
bundle exec fastlane ios distribute notes:"Build description"

# With store upload
bundle exec fastlane android distribute upload_to_store:true track:internal
bundle exec fastlane ios distribute upload_to_store:true
```

## CI Workflows

| Workflow | Trigger | Output |
|---|---|---|
| `android-build.yml` | push/PR to `master`, manual | AAB -> Firebase + Play Store |
| `ios-build.yml` | push/PR to `master`, manual | IPA -> Firebase + TestFlight |
| `claude.yml` | `@claude` mentions | Claude responds in thread |
| `claude-code-review.yml` | PR opened/synchronize | Claude reviews diff |

Store upload fires automatically on push to `master`, or manually via workflow dispatch.

## Required GitHub Secrets

See [SETUP_GUIDE.md](docs/SETUP_GUIDE.md) for how to obtain each.

**Unity**: `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`

**Firebase**: `FIREBASE_SERVICE_ACCOUNT_JSON`, `FIREBASE_APP_ID_ANDROID`, `FIREBASE_APP_ID_IOS`

**Android**: `ANDROID_PACKAGE_NAME`, `ANDROID_KEYSTORE_BASE64`, `ANDROID_KEYSTORE_PASS`, `ANDROID_KEY_ALIAS_NAME`, `ANDROID_KEY_ALIAS_PASS`, `GPLAY_SERVICE_JSON`

**iOS**: `APPLE_TEAM_ID`, `IOS_BUNDLE_ID`, `IOS_DISTRIBUTION_CERTIFICATE_P12_BASE64`, `IOS_DISTRIBUTION_CERTIFICATE_PASSWORD`, `IOS_PROVISIONING_PROFILE_BASE64`, `IOS_PROVISIONING_PROFILE_NAME`, `APP_STORE_CONNECT_KEY_ID`, `APP_STORE_CONNECT_ISSUER_ID`, `APP_STORE_CONNECT_KEY_CONTENT`

**Claude Code (optional)**: `CLAUDE_CODE_OAUTH_TOKEN` - or delete `.github/workflows/claude*.yml` if not using.

## Customization

### Firebase tester groups

Default is `qa-testers`. Override per-invocation:

```bash
bundle exec fastlane android distribute groups:"qa-testers,beta-testers"
```

Or edit the default in `fastlane/Fastfile`.

### Play Store track

```bash
bundle exec fastlane android distribute upload_to_store:true track:beta
# Tracks: internal, alpha, beta, production
```

Play policy requires progression internal -> closed -> open -> production for new apps (Aug 2026 rules).

## Troubleshooting

See the [Troubleshooting section](docs/SETUP_GUIDE.md#troubleshooting) in the setup guide.

## Resources

- [GameCI](https://game.ci/docs)
- [Fastlane](https://docs.fastlane.tools)
- [Firebase App Distribution](https://firebase.google.com/docs/app-distribution)
- [anthropics/claude-code-action](https://github.com/anthropics/claude-code-action)

## License

Provided as-is for reference and educational use.
