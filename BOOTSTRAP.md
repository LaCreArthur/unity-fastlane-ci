# BOOTSTRAP.md

Agent-executable checklist to adapt this template to a new Unity game. Written for an AI coding agent (Claude Code, Codex, Cursor, Copilot). A human can follow too.

**Invariant**: every step is either an edit to a known file or a validation you can confirm. No "configure Firebase" hand-waves - the exact button path is in [docs/SETUP_GUIDE.md](docs/SETUP_GUIDE.md).

---

## Phase 0 - Fork / Clone

- [ ] Used "Use this template" on GitHub (preferred) OR cloned and pushed to a new repo
- [ ] New repo name + visibility set (public/private)
- [ ] Default branch is `master` (or matches workflow triggers - see `.github/workflows/*-build.yml`)

## Phase 1 - Placeholder replacement

Replace the template defaults below throughout the repo. The agent should `grep` for each placeholder and edit every occurrence unless noted.

| Placeholder | Where | Replace with |
|---|---|---|
| `UNITY_VERSION: 6000.3.2f1` | `.github/workflows/android-build.yml`, `ios-build.yml` | Your `ProjectSettings/ProjectVersion.txt` value |
| `qa-testers` | `fastlane/Fastfile` (default `groups:`) | Your Firebase App Distribution group name |
| `track: options[:track] \|\| "internal"` | `fastlane/Fastfile` | Keep `internal` for first launch; progress per Play policy |
| Firebase tester group creation | Firebase Console -> App Distribution -> Testers & Groups | Match the name in Fastfile |
| `master` branch in workflow triggers | `.github/workflows/*-build.yml` | Match your repo's default branch |

### If you're NOT using Claude Code

- [ ] Delete `.github/workflows/claude.yml`
- [ ] Delete `.github/workflows/claude-code-review.yml`
- [ ] Remove `CLAUDE_CODE_OAUTH_TOKEN` from the secret checklist below

### If you're extending with Unity gameplay code

- [ ] Read `docs/ai-guidelines/unity-development.md` - applies to any added C#
- [ ] Keep `[SerializeField]` inspector-first discipline; don't null-check them

---

## Phase 2 - Secrets (the long one)

Each secret has a collection step documented in [docs/SETUP_GUIDE.md](docs/SETUP_GUIDE.md). Work through that guide, then come back here and tick.

### Unity
- [ ] `UNITY_LICENSE` - contents of `.ulf` from manual activation (see SETUP_GUIDE -> Unity License Activation)
- [ ] `UNITY_EMAIL`
- [ ] `UNITY_PASSWORD`

### Firebase
- [ ] `FIREBASE_SERVICE_ACCOUNT_JSON` - entire JSON
- [ ] `FIREBASE_APP_ID_ANDROID` - format `1:NNN:android:xxx`
- [ ] `FIREBASE_APP_ID_IOS` - format `1:NNN:ios:xxx`
- [ ] Tester group `qa-testers` (or your renamed value) exists in Firebase App Distribution

### Android
- [ ] `ANDROID_PACKAGE_NAME` - e.g. `com.company.game`; matches Unity Player Settings
- [ ] `ANDROID_KEYSTORE_BASE64` - `base64 -i user.keystore`; backup the `.keystore` file offsite (losing it = losing Play Store access)
- [ ] `ANDROID_KEYSTORE_PASS`
- [ ] `ANDROID_KEY_ALIAS_NAME`
- [ ] `ANDROID_KEY_ALIAS_PASS`
- [ ] `GPLAY_SERVICE_JSON` - Play Console service account, granted access to your app
- [ ] First AAB uploaded manually to Play Console internal testing (required before CI can upload)
- [ ] Unity Player Settings -> Target API Level = 36+ (required for Play uploads from Aug 31, 2026)

### iOS
- [ ] `APPLE_TEAM_ID` - 10-char alphanumeric
- [ ] `IOS_BUNDLE_ID` - matches Xcode + App Store Connect app record
- [ ] `IOS_DISTRIBUTION_CERTIFICATE_P12_BASE64`
- [ ] `IOS_DISTRIBUTION_CERTIFICATE_PASSWORD`
- [ ] `IOS_PROVISIONING_PROFILE_BASE64` - App Store distribution profile
- [ ] `IOS_PROVISIONING_PROFILE_NAME` - **exact** name including spaces, case-sensitive
- [ ] `APP_STORE_CONNECT_KEY_ID`
- [ ] `APP_STORE_CONNECT_ISSUER_ID`
- [ ] `APP_STORE_CONNECT_KEY_CONTENT` - base64-encoded `.p8`
- [ ] App record created in App Store Connect with matching bundle ID

### Claude Code (optional)
- [ ] `CLAUDE_CODE_OAUTH_TOKEN` - only if you kept `claude*.yml` workflows

---

## Phase 3 - Local smoke test (optional but recommended)

Before the first push, verify locally that Fastlane can talk to Firebase.

- [ ] `bundle install` succeeds
- [ ] Build an AAB from Unity: File -> Build Settings -> Android -> Build
- [ ] Set env vars:
  ```bash
  export FIREBASE_APP_ID_ANDROID="1:NNN:android:xxx"
  export ANDROID_AAB_PATH="./build/Android/Android.aab"
  export GOOGLE_APPLICATION_CREDENTIALS="./firebase-service-account.json"
  ```
- [ ] `bundle exec fastlane android distribute notes:"local smoke test"` succeeds
- [ ] Build appears in Firebase App Distribution console

Same drill for iOS if on a Mac.

---

## Phase 4 - First CI run

- [ ] Push a commit to `master`
- [ ] `Android CI/CD` workflow completes green
- [ ] `iOS CI/CD` workflow completes green
- [ ] AAB/IPA appears in Firebase App Distribution
- [ ] AAB uploaded to Play Store internal track (if push to master)
- [ ] IPA uploaded to TestFlight (if push to master)
- [ ] Tester emails arrive

---

## Phase 5 - Clean up template-only files

- [ ] Remove `docs/ai-guidelines/` if not extending with gameplay C#
- [ ] Edit `README.md` top section to describe your game, not "reference template"
- [ ] Edit `CLAUDE.md` / `AGENTS.md` to point to your game's conventions
- [ ] Delete `BOOTSTRAP.md` (this file) - bootstrapping is done

---

## Failure recovery

Most first-run failures fall into a small set:

| Symptom | Usual cause |
|---|---|
| `Unity license invalid` | `UNITY_LICENSE` truncated or wrong Unity version - re-activate |
| `Provisioning profile doesn't match` | Name mismatch (case, spaces) OR bundle ID mismatch |
| `Version code already exists` (Play) | GameCI semantic versioning collided - bump `bundleVersion` in Player Settings |
| `App not found` (Firebase) | `FIREBASE_APP_ID_*` format wrong, should start `1:` |
| Xcode/TestFlight upload "succeeds" but build never appears | Xcode 26 regression - confirm iOS workflow still pins `16.4` |
| `AAB not found` | Unity exported APK not AAB - confirm `androidExportType: androidAppBundle` in workflow |

See [docs/SETUP_GUIDE.md](docs/SETUP_GUIDE.md#troubleshooting) for more.
