# LESSONS.md

Hard-won gotchas + decisions from real bootstraps of this template into live games. Read alongside [BOOTSTRAP.md](BOOTSTRAP.md). When the agent hits one of the symptoms below during bootstrap, the fix is documented here — don't re-derive.

Ordered roughly by likelihood of biting on a fresh bootstrap.

---

## 1. ASC API key does NOT auto-thread into xcodebuild

**Symptom:** CI macOS runner errors during archive or export:
```
error: No Accounts: Add a new account in Accounts settings.
error: No profiles for 'com.x.y' were found
error: exportArchive Cloud signing permission error
error: exportArchive No signing certificate "iOS Distribution" found
```

**Cause:** `app_store_connect_api_key` action sets `lane_context[SharedValues::APP_STORE_CONNECT_API_KEY]`, but only **Spaceship-based actions** (`pilot`, `match`, `sigh`, `cert`, `pem`, `download_dsyms`, `deliver`) read from there. `build_app`/gym shells out to `xcodebuild`, which has no access to that lane context. `build_app` does NOT accept an `api_key` parameter (verified against fastlane source — error lists the available options, `api_key` isn't there).

**Fix:** Decode the `.p8` to disk and pass auth flags to xcodebuild via `xcargs`. **Both phases** need it — gym splits archive (`xcargs:`) and export (`export_xcargs:`). Putting flags only on archive will pass the archive then fail export.

```ruby
key_path = File.expand_path("~/.appstoreconnect/private_keys/AuthKey_#{ENV['APP_STORE_CONNECT_KEY_ID']}.p8")
FileUtils.mkdir_p(File.dirname(key_path))
File.write(key_path, Base64.decode64(ENV["APP_STORE_CONNECT_KEY_CONTENT"]))
sign_xcargs = "-allowProvisioningUpdates" \
              " -authenticationKeyPath #{key_path}" \
              " -authenticationKeyID #{ENV['APP_STORE_CONNECT_KEY_ID']}" \
              " -authenticationKeyIssuerID #{ENV['APP_STORE_CONNECT_ISSUER_ID']}"

build_app(
  scheme: "Unity-iPhone",
  xcargs: sign_xcargs,
  export_xcargs: sign_xcargs,
  # ...
)
```

**Why:** With Cloud Managed Cert + `-allowProvisioningUpdates`, Xcode auto-creates the distribution profile. No `match`, no manual cert/profile management. Replaces the entire keychain-import + provisioning-profile-install dance.

References: docs.fastlane.tools/actions/build_app, fastlane Discussion #19973.

---

## 2. Versioning model: three numbers, three namespaces

Solo-dev safe model:

| Field | Where | Type | Bumped by |
|---|---|---|---|
| Marketing version | `bundleVersion` in PlayerSettings (`2.5`) | semver string | Manual in Unity (per public release) |
| Build number | `iPhone:` + `AndroidBundleVersionCode` | int, unified | Fastlane: `max(latestASC, latestPlay) + 1` |
| TestFlight train | `CFBundleShortVersionString` (iOS only) | int | Fastlane: `max(ASC preReleaseVersions integer-only) + 1` |

**Critical: build number is store-derived, not file-derived.** Querying ASC + Play and taking `max+1` is the only way to stay monotonic across machines / CI / local without git collisions. Patch `ProjectSettings.asset` directly with sed-style ruby gsub — no env var indirection through the Unity build script (simpler, no rewriting of build scripts).

```ruby
def patch_project_settings_build_number(n)
  path = File.join(PROJECT_ROOT, "ProjectSettings/ProjectSettings.asset")
  content = File.read(path)
  content.sub!(/^(  AndroidBundleVersionCode: )\d+/, "\\1#{n}")
  content.sub!(/(^  buildNumber:\n(?:    \w+: [^\n]*\n)*?    iPhone: )\d+/, "\\1#{n}")
  File.write(path, content)
end
```

---

## 3. Apple TestFlight integer-train trap

**Symptom:** TestFlight upload rejected with "version 2.5 is less than closed version 8" or similar. Or rejected for matching a closed train.

**Cause:** Apple semver-compares `CFBundleShortVersionString`. `"2.5"` < `"8"` because `"8"` parses as `"8.0.0"`. If the project has a legacy history of integer marketing versions on TestFlight (trains 2-17 etc.), a switch to semver `"2.5"` blocks uploads.

**Also critical:** `CFBundleShortVersionString` (binary embedded) is a **separate namespace** from the App Store version (ASC metadata). The App Store can be on `"2.5"` while the attached build's `CFBundleShortVersionString` is `"18"`. Apple decouples them. Don't conflate.

**Fix:** Auto-bump the integer train per build. Override `CFBundleShortVersionString` in `PostProcessBuild` from an env var Fastlane sets:

```ruby
# Fastfile
def compute_next_testflight_train
  # ... query ASC /v1/apps/{id}/preReleaseVersions, parse integer-only
  ints.max.to_i + 1
end

ENV["IOS_MARKETING_VERSION"] = compute_next_testflight_train.to_s
```

```csharp
// PostProcessBuild.cs
string iosMarketing = System.Environment.GetEnvironmentVariable("IOS_MARKETING_VERSION");
if (!string.IsNullOrEmpty(iosMarketing))
    rootDict.SetString("CFBundleShortVersionString", iosMarketing);
```

Marketing version (`bundleVersion`) stays semver in Unity, manually bumped per App Store release. App Store release version is set manually in ASC (or by `deliver`/`upload_to_app_store` with `app_version:`).

---

## 4. PostProcessBuild iOS guard (`#if UNITY_IOS`)

**Symptom:** Android build fails to compile because `UnityEditor.iOS.Xcode` references don't exist.

**Fix:** Wrap the entire file:
```csharp
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
// ...
public static class PostProcessBuild { /* ... */ }
#endif
```

Also `if (buildTarget != BuildTarget.iOS) return;` inside the method is redundant once the file is guarded but harmless.

---

## 5. PostProcessBuild absolute paths break two-stage CI

**Symptom:** xcodebuild errors during archive on macOS runner:
```
error: Build input file cannot be found:
  '/github/workspace/build/iOS/iOS/GameCenter.entitlements'
```

**Cause:** `proj.AddFile(absolutePath, ...)` bakes the build host's filesystem path into pbxproj. Works locally and on a single-machine CI. Breaks GameCI two-stage pipeline: Ubuntu container builds at `/github/workspace/...`, then macOS runner downloads the project to `/Users/runner/work/...` — the embedded absolute path no longer resolves.

**Fix:** Use `PBXSourceTree.Source` (project-relative):
```csharp
string entitlementsGuid = proj.AddFile(entitlementsFileName, entitlementsFileName, PBXSourceTree.Source);
```

---

## 6. GameCI Xcode output is nested

GameCI nests output as `build/iOS/iOS/` (= `buildPath/buildName`). Fastlane archive lane needs to point at the inner dir, not the outer. Workflow:

```yaml
env:
  IOS_XCODE_PROJECT_PATH: ${{ env.IOS_BUILD_PATH }}/iOS  # double-iOS
```

Local builds (no GameCI) put the project directly at `build/iOS/`. Fastlane handles either via `ENV["IOS_XCODE_PROJECT_PATH"] || "build/iOS"`.

---

## 7. Auto-derive Unity version from ProjectVersion.txt

Don't hardcode the Unity version in workflows. Resolve it once:

```yaml
- name: Resolve Unity version
  id: unity
  run: |
    VERSION=$(awk '/^m_EditorVersion:/ {print $2}' ProjectSettings/ProjectVersion.txt)
    echo "version=$VERSION" >> "$GITHUB_OUTPUT"

- uses: game-ci/unity-builder@v4
  with:
    unityVersion: ${{ steps.unity.outputs.version }}
```

Same pattern in Fastfile for local lanes:
```ruby
def unity_binary
  version = File.read("ProjectSettings/ProjectVersion.txt")[/^m_EditorVersion:\s*(\S+)/, 1]
  "/Applications/Unity/Hub/Editor/#{version}/Unity.app/Contents/MacOS/Unity"
end
```

Bumping Unity = single file edit, no hunt for hardcodes.

---

## 8. Bundler env isolation for `pod install`

**Symptom:** `pod install` (run from inside a fastlane lane) fails with mysterious gem version conflicts — usually brew's system Ruby (e.g. 4.0.2) gets picked up instead of rbenv's project Ruby.

**Fix:** Wrap subprocess calls in `Bundler.with_unbundled_env`:
```ruby
if File.exist?(File.join(xc, "Podfile"))
  Bundler.with_unbundled_env do
    Dir.chdir(xc) { sh("pod", "install") }
  end
end
```

---

## 9. dotenv single-quote requirement for JSON values

**Symptom:** `Could not parse service account json` then `OpenSSL: Neither PUB key nor PRIV key`. The JSON's escaped newlines (`\n`) inside the private key get eaten by dotenv.

**Fix:** Single-quote the entire JSON value in `fastlane/.env`:
```bash
GPLAY_SERVICE_JSON='{"type":"service_account","private_key":"-----BEGIN PRIVATE KEY-----\nMIIE...\n-----END PRIVATE KEY-----\n","client_email":"..."}'
```

Double quotes and unquoted both fail. Single quotes preserve `\n` literally.

---

## 10. Cross-platform package name divergence (legacy projects)

**Symptom:** Play Store upload errors `Package not found com.PascalCase.Game`.

**Cause:** Live Play app may use legacy lowercase package (`com.lowercase.legacyname`) while iOS App Store record uses modern PascalCase (`com.Brand.NewName`). Unity Player Settings has separate `applicationIdentifier` per platform — they CAN diverge, and on legacy projects they often do.

**Fix:** Before assuming Player Settings is authoritative, query the actual stores:
```bash
gcloud auth application-default login
# Then check Play Console live package via Play API
```

If they differ:
1. Update Unity's Android `applicationIdentifier` to match live Play package
2. Recreate Firebase Android app for that package, get new `1:NNN:android:xxx`
3. **Delete the ghost Firebase Android app** for the wrong package (keeps Firebase Console clean)
4. Regenerate `google-services.json`, place in `Assets/`
5. Update `.env` and CI secret `ANDROID_PACKAGE_NAME`

Don't try to rename the live Play app — Google won't let you change package name.

---

## 11. AndroidAutoKey editor script flapping

**Symptom:** `PlayerSettings.Android.keystoreName` resets to wrong path every Unity Editor open.

**Cause:** Editor script with `[InitializeOnLoad]` hardcodes a path that doesn't exist on the current machine, overwriting it on every domain reload.

**Fix:** Either (A) edit the script to point at the correct path on this machine, or (B) delete the script and let GameCI workflow inputs set the keystore via `androidKeystoreBase64`/`androidKeystorePass`.

```csharp
#if UNITY_EDITOR_OSX
PlayerSettings.Android.keystoreName = "/Users/<you>/path/to/user.keystore";
#elif UNITY_EDITOR_WIN
PlayerSettings.Android.keystoreName = "C:/path/to/user.keystore";
#endif
```

For multi-machine teams: prefer (B) and remove the script entirely.

---

## 12. Unity license handshake: one instance only

**Symptom:** Local `fastlane ios ship` fails immediately with license error.

**Cause:** Unity GUI is open. Headless batch can't acquire the license while another Unity instance holds it.

**Fix:** Close the Unity Editor before running fastlane batch lanes. There's no way around this except to use Unity Pro Floating License with multiple seats (overkill for solo).

---

## 13. Unity 6.4 GameCI Linux audio crash (UNRESOLVED upstream)

**Symptom:** GameCI Ubuntu Android batch build crashes with:
```
::::Assertion failed on expression: 'm_PodArrays.empty()'
::::ADTM: Toggling realtime enabled while a mix is ongoing
Got a SIGSEGV while executing native code.
Aborted (core dumped)
Build failed, with exit code 134
```

**Cause:** Unknown. Crash signature returns zero hits across GitHub / Unity issue tracker / forums (verified Apr 2026). Unity 6 has confirmed Linux batchmode regressions in adjacent areas (video driver init), but this specific audio (`ADTM`/`m_PodArrays`) crash is undocumented.

**Workarounds tried in NewDoge bootstrap:**
- `SDL_AUDIODRIVER=dummy` — alone, untested isolation
- `SDL_AUDIODRIVER=dummy + SDL_VIDEODRIVER=dummy` — turned 6min crash into 2h+ hang (Unity needed video context for shader compilation)
- Last fallback (untried): `m_DisableAudio: 1` in `ProjectSettings/AudioManager.asset` — disables Unity's audio engine entirely during build (mobile runtime audio comes from device, not editor mixer)

**Decision tree if you hit this on bootstrap:**
1. Try `SDL_AUDIODRIVER=dummy` only (env on the GameCI step)
2. If still crashes, try `m_DisableAudio: 1` in AudioManager.asset (commit the change)
3. If still crashes, switch Android CI to macOS runner (10x more Actions minutes but sidesteps the entire Linux Docker image)
4. If burning Actions minutes is unacceptable, drop CI for Android — see #15

---

## 14. GitHub Actions free minutes burn fast on Unity builds

A single Unity Android batch on Ubuntu GameCI = ~30-60min on free runner. iOS macOS runner job = ~25-50min. Iteration on a broken pipeline (10 attempts) easily caps the monthly free budget.

**Implications:**
- Don't iterate on CI when the bug isn't CI-specific. Reproduce locally first.
- If the upstream bug is unfixable from outside (e.g. #13), parking CI is the right call.
- Self-hosted runner on dev Mac costs no Actions minutes but adds operational overhead (Mac must be on, can't use Unity GUI during build, license handshake).

---

## 15. Solo-dev pattern: drop CI, keep local fastlane

For solo dev with a working local pipeline + auto-changelog from git log, CI adds no value:
- No team merge gate
- No multi-developer parallelism
- No external contributor PRs to validate
- Local `fastlane ship` is faster than CI cold-start anyway

**Replacement workflow:**
```
fastlane {ios,android} ship    # daily test build
fastlane android promote       # production rollout (interactive changelog edit)
fastlane ios submit            # stage App Store version (manual final Submit in ASC)
```

Auto-changelog helper (in Fastfile):
```ruby
def release_notes_from_git
  last_tag = `git describe --tags --abbrev=0 2>/dev/null`.strip
  range = last_tag.empty? ? "HEAD~30..HEAD" : "#{last_tag}..HEAD"
  log = `git log #{range} --no-merges --pretty=format:"- %s"`.strip
  log.empty? ? "- maintenance update" : log
end

def review_release_notes(default_text)
  return default_text if ENV["FASTLANE_NO_PROMPT"]
  Tempfile.create(["release-notes", ".md"]) do |f|
    f.write("# Edit. Lines starting with # ignored.\n\n#{default_text}")
    f.flush
    sh("#{ENV['EDITOR'] || 'vim'} #{f.path}")
    File.read(f.path).lines.reject { |l| l.start_with?("#") }.join.strip
  end
end
```

**Production lanes (Android):**
- `upload_to_play_store(track: "internal", track_promote_to: "production", version_code: <code>, rollout: "1.0", skip_upload_aab: true, skip_upload_apk: true, skip_upload_changelogs: false)` promotes existing internal build, no rebuild needed.
- Source `version_code` from `google_play_track_version_codes(track: "internal").max`.
- Write changelog to `fastlane/metadata/android/<lang>/changelogs/<versionCode>.txt` before calling.

**Staging lanes (iOS):**
- `upload_to_app_store` with `submit_for_review: false` uploads release notes + attaches the latest TestFlight build, but doesn't trigger Apple review. Final visual check + Submit click happens in ASC.
- Build number sourced from `latest_testflight_build_number(app_identifier: bundle_id)`.

When to revisit CI:
- Project gains contributors needing merge gates
- Hands-free deploy on git push becomes valuable (rare for solo)
- Unity fixes the Linux batchmode regression (issue #13)

---

## 16. Firebase App Distribution: legacy artifact, not a goal

If the bootstrap inherits Firebase App Distribution config (FIREBASE_SERVICE_ACCOUNT_JSON, app IDs, tester groups), confirm it's actually wanted. Modern flow uses Play Internal Testing track + TestFlight directly — both have built-in tester management, and you avoid the third-party hop.

If dropping Firebase App Distribution: also remove `firebase_app_distribution` from Fastfile lanes, drop the secrets, and uninstall the gem from Pluginfile.

---

## Quick-reference symptom table

| Symptom | Section |
|---|---|
| `No Accounts: Add a new account` (xcodebuild) | #1 |
| `No profiles for 'com.x.y' were found` | #1 |
| `Cloud signing permission error` (export) | #1 |
| `version X.Y is less than closed version Z` (TestFlight) | #3 |
| `Build input file cannot be found: '/github/workspace/...'` | #5 |
| `iOS.Xcode` namespace not found (Android compile) | #4 |
| `Could not parse service account json` | #9 |
| `Package not found com.X.Y` (Play upload) | #10 |
| Keystore path keeps reverting in Unity | #11 |
| Unity license error in batch mode | #12 |
| `m_PodArrays.empty()` / `ADTM` SIGSEGV (GameCI Linux) | #13 |
| Pod install gem version conflict | #8 |
| Unity Xcode project not at expected path on CI | #6 |
| Build numbers diverging between iOS / Android | #2 |
| Hardcoded Unity version in workflow | #7 |
| Asking "should I keep CI?" as solo dev | #15 |
