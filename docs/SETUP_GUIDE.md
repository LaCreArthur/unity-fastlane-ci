# Complete Setup Guide: Unity Game CI with Fastlane

This comprehensive guide walks you through setting up automated CI/CD for your Unity game project from scratch. By the end, you'll have:

- ✅ Automated Unity builds on GitHub Actions
- ✅ Beta distribution via Firebase App Distribution
- ✅ Production releases to Google Play Store and Apple App Store

**Estimated setup time:** 2-3 hours

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Structure Setup](#project-structure-setup)
3. [Unity License Activation](#unity-license-activation)
4. [Firebase Setup](#firebase-setup)
5. [Android Configuration](#android-configuration)
6. [iOS Configuration](#ios-configuration)
7. [GitHub Secrets Configuration](#github-secrets-configuration)
8. [Testing Your Setup](#testing-your-setup)
9. [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before starting, ensure you have:

- [ ] A Unity project (2021.3 LTS or newer recommended)
- [ ] A GitHub account with your project in a repository
- [ ] A Google account (for Firebase and Play Store)
- [ ] An Apple Developer account ($99/year) - for iOS only
- [ ] Ruby installed locally (for testing Fastlane)

### Required Software

```bash
# macOS
brew install ruby
gem install bundler

# Windows (use RubyInstaller from https://rubyinstaller.org/)
gem install bundler

# Verify installation
ruby --version
bundler --version
```

---

## Project Structure Setup

### Step 1: Copy CI Files to Your Project

Copy these files/folders to your Unity project root:

```
your-unity-project/
├── .github/
│   └── workflows/
│       ├── android-build.yml
│       └── ios-build.yml
├── fastlane/
│   ├── Fastfile
│   └── Pluginfile
├── Gemfile
└── ... (your Unity project files)
```

### Step 2: Create the Gemfile

Create `Gemfile` in your project root:

```ruby
source "https://rubygems.org"

gem "fastlane"
plugins_path = File.join(File.dirname(__FILE__), 'fastlane', 'Pluginfile')
eval_gemfile(plugins_path) if File.exist?(plugins_path)
```

### Step 3: Create the Pluginfile

Create `fastlane/Pluginfile`:

```ruby
gem 'fastlane-plugin-firebase_app_distribution'
```

### Step 4: Install Dependencies Locally

```bash
cd your-unity-project
bundle install
```

### Step 5: Update Unity Version

Edit `.github/workflows/android-build.yml` and `.github/workflows/ios-build.yml`:

```yaml
env:
  UNITY_VERSION: 6000.0.62f1  # Change to your Unity version
```

Find your Unity version in Unity Hub or `ProjectSettings/ProjectVersion.txt`.

---

## Unity License Activation

GameCI requires a Unity license to build your project. There are three license types:

| License Type | Cost | CI Support |
|--------------|------|------------|
| Personal | Free | ✅ Requires activation |
| Plus/Pro | Paid | ✅ Requires activation |
| Floating | Enterprise | ✅ Different process |

### Step 1: Request Activation File

1. Create `.github/workflows/activation.yml`:

```yaml
name: Acquire Unity License
on:
  workflow_dispatch: {}

jobs:
  activation:
    name: Request activation file
    runs-on: ubuntu-latest
    steps:
      - name: Request manual activation file
        id: getManualLicenseFile
        uses: game-ci/unity-request-activation-file@v2
        with:
          unityVersion: 6000.0.62f1  # Your Unity version

      - name: Expose as artifact
        uses: actions/upload-artifact@v4
        with:
          name: Unity_v${{ steps.getManualLicenseFile.outputs.unityVersion }}.alf
          path: ${{ steps.getManualLicenseFile.outputs.filePath }}
```

2. Push this file and go to **Actions** tab in GitHub
3. Run the "Acquire Unity License" workflow manually
4. Download the `.alf` artifact file

### Step 2: Activate License on Unity Website

1. Go to [license.unity3d.com/manual](https://license.unity3d.com/manual)
2. Sign in with your Unity account
3. Upload the `.alf` file
4. Select your license type:
   - **Personal**: Choose "Unity Personal Edition"
   - **Pro/Plus**: Choose "Unity Pro/Plus" and enter your serial
5. Download the `.ulf` license file

### Step 3: Add License to GitHub Secrets

1. Open the `.ulf` file in a text editor
2. Copy the **entire contents**
3. Go to your GitHub repo → **Settings** → **Secrets and variables** → **Actions**
4. Create these secrets:

| Secret Name | Value |
|-------------|-------|
| `UNITY_LICENSE` | Entire contents of the `.ulf` file |
| `UNITY_EMAIL` | Your Unity account email |
| `UNITY_PASSWORD` | Your Unity account password |

> ⚠️ **Important**: The `.ulf` file is tied to your Unity version. If you upgrade Unity, you may need to repeat this process.

### Step 4: Clean Up

Delete `.github/workflows/activation.yml` after activation - you don't need it anymore.

---

## Firebase Setup

Firebase App Distribution allows you to distribute beta builds to testers without going through the app stores.

### Step 1: Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Click **"Create a project"** (or use existing)
3. Enter project name (e.g., "My Game CI")
4. Disable Google Analytics (optional, not needed for App Distribution)
5. Click **"Create project"**

### Step 2: Add Android App

1. In Firebase Console, click **"Add app"** → **Android**
2. Enter your Android package name:
   - Find it in Unity: **Edit** → **Project Settings** → **Player** → **Android** → **Other Settings** → **Package Name**
   - Example: `com.yourcompany.yourgame`
3. Enter app nickname (optional): "My Game Android"
4. Skip the SHA-1 fingerprint for now
5. Click **"Register app"**
6. Download `google-services.json` (optional for App Distribution only)
7. Click **"Continue"** through remaining steps

### Step 3: Add iOS App

1. Click **"Add app"** → **iOS**
2. Enter your iOS bundle ID:
   - Find it in Unity: **Edit** → **Project Settings** → **Player** → **iOS** → **Other Settings** → **Bundle Identifier**
   - Should match your Android package name
3. Enter app nickname: "My Game iOS"
4. Skip App Store ID for now
5. Click **"Register app"**
6. Download `GoogleService-Info.plist` (optional for App Distribution only)

### Step 4: Get Firebase App IDs

1. In Firebase Console, go to **Project Settings** (gear icon)
2. Scroll down to **"Your apps"**
3. Find each app and copy the **"App ID"**:
   - Android: `1:123456789012:android:abc123def456`
   - iOS: `1:123456789012:ios:abc123def456`

Save these for later:
- `FIREBASE_APP_ID_ANDROID`: The Android App ID
- `FIREBASE_APP_ID_IOS`: The iOS App ID

### Step 5: Create Service Account for CI

1. In Firebase Console, go to **Project Settings** → **Service accounts**
2. Click **"Generate new private key"**
3. Click **"Generate key"** in the confirmation dialog
4. A JSON file will download - **keep this secure!**

The JSON file looks like:
```json
{
  "type": "service_account",
  "project_id": "your-project-id",
  "private_key_id": "abc123...",
  "private_key": "-----BEGIN PRIVATE KEY-----\n...",
  "client_email": "firebase-adminsdk-xxxxx@your-project.iam.gserviceaccount.com",
  ...
}
```

Save the **entire contents** as `FIREBASE_SERVICE_ACCOUNT_JSON`.

### Step 6: Enable App Distribution

1. In Firebase Console, go to **Release & Monitor** → **App Distribution**
2. Click **"Get started"**
3. Accept the terms

### Step 7: Create Tester Groups

1. In App Distribution, go to **"Testers & Groups"**
2. Click **"Add group"**
3. Create a group named `qa-testers` (matches the Fastfile configuration)
4. Add tester email addresses
5. Testers will receive an email to join

> 💡 **Tip**: Create multiple groups like `qa-testers`, `beta-testers`, `stakeholders` for different release stages.

---

## Android Configuration

### Step 1: Create Signing Keystore

Your app must be signed for Play Store distribution.

#### Generate New Keystore

```bash
keytool -genkey -v \
  -keystore user.keystore \
  -alias your-key-alias \
  -keyalg RSA \
  -keysize 2048 \
  -validity 10000 \
  -storepass your-keystore-password \
  -keypass your-key-password \
  -dname "CN=Your Name, OU=Your Company, O=Your Company, L=City, ST=State, C=US"
```

**Replace:**
- `your-key-alias`: A name for your key (e.g., `upload-key`)
- `your-keystore-password`: Strong password for the keystore
- `your-key-password`: Strong password for the key (can be same as keystore)
- `dname` values: Your actual information

#### Convert to Base64

```bash
# macOS/Linux
base64 -i user.keystore > user.keystore.base64.txt

# Windows PowerShell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("user.keystore")) | Out-File user.keystore.base64.txt -Encoding ASCII
```

Save these values:
- `ANDROID_KEYSTORE_BASE64`: Contents of `user.keystore.base64.txt`
- `ANDROID_KEYSTORE_PASS`: Your keystore password
- `ANDROID_KEY_ALIAS_NAME`: Your key alias (e.g., `upload-key`)
- `ANDROID_KEY_ALIAS_PASS`: Your key password

> ⚠️ **Critical**: Back up `user.keystore` securely! If lost, you cannot update your app on Play Store.

### Step 2: Google Play Console Setup

#### Create Developer Account

1. Go to [Google Play Console](https://play.google.com/console)
2. Pay the one-time $25 registration fee
3. Complete identity verification

#### Create Your App

1. Click **"Create app"**
2. Fill in app details:
   - App name
   - Default language
   - App or game
   - Free or paid
3. Complete the app setup checklist (privacy policy, etc.)

#### Enable API Access

1. Go to **Setup** → **API access**
2. Click **"Link"** to link/create a Google Cloud project
3. Accept the terms

### Step 3: Create Service Account for Play Store

1. In Play Console, go to **Setup** → **API access**
2. Click **"Create new service account"**
3. Click the **"Google Cloud Platform"** link (opens new tab)
4. In Google Cloud Console:
   - Click **"+ CREATE SERVICE ACCOUNT"**
   - Name: `play-store-ci` (or similar)
   - Click **"CREATE AND CONTINUE"**
   - Skip the optional steps
   - Click **"DONE"**
5. Find your new service account in the list
6. Click the **⋮** menu → **"Manage keys"**
7. Click **"ADD KEY"** → **"Create new key"**
8. Choose **"JSON"** format
9. Click **"CREATE"** - downloads the key file

### Step 4: Grant Service Account Permissions

1. Return to Play Console → **Setup** → **API access**
2. Click **"Refresh"** to see your new service account
3. Click **"Grant access"** next to your service account
4. Under **"App permissions"**:
   - Select your app
   - Grant **"Release to production, exclude devices, and use Play App Signing"**
5. Under **"Account permissions"**, leave defaults
6. Click **"Invite user"**

### Step 5: Upload First Build Manually

> ⚠️ **Important**: You must upload the first build manually before CI can upload subsequent builds.

1. In Play Console, go to **Release** → **Testing** → **Internal testing**
2. Click **"Create new release"**
3. Upload your AAB file (build from Unity locally first)
4. Complete the release

Save the service account JSON file contents as `GPLAY_SERVICE_JSON`.

---

## iOS Configuration

### Step 1: Apple Developer Program

1. Enroll at [developer.apple.com/programs](https://developer.apple.com/programs/)
2. Pay the $99/year fee
3. Wait for approval (usually 24-48 hours)

### Step 2: Create App ID

1. Go to [Certificates, Identifiers & Profiles](https://developer.apple.com/account/resources/identifiers/list)
2. Click **"+"** to add new identifier
3. Select **"App IDs"** → **"App"**
4. Enter:
   - Description: Your app name
   - Bundle ID: Explicit, e.g., `com.yourcompany.yourgame`
5. Select capabilities your app needs (Push Notifications, etc.)
6. Click **"Continue"** → **"Register"**

Save `IOS_BUNDLE_ID`: Your bundle ID (e.g., `com.yourcompany.yourgame`)

### Step 3: Create Distribution Certificate

1. Go to [Certificates](https://developer.apple.com/account/resources/certificates/list)
2. Click **"+"** to create new certificate
3. Select **"Apple Distribution"** (for App Store)
4. Click **"Continue"**

#### Generate Certificate Signing Request (CSR)

**On macOS:**
1. Open **Keychain Access**
2. Menu: **Keychain Access** → **Certificate Assistant** → **Request a Certificate From a Certificate Authority**
3. Enter:
   - Email: Your email
   - Common Name: Your name or company
   - CA Email: Leave empty
   - Request is: **Saved to disk**
4. Save the `.certSigningRequest` file

**Back in Apple Developer Portal:**
1. Upload the CSR file
2. Click **"Continue"**
3. Download the certificate (`.cer` file)
4. Double-click to install in Keychain Access

#### Export as P12

1. Open **Keychain Access**
2. Find your certificate under **"My Certificates"** (look for "Apple Distribution: Your Name")
3. Expand it to see the private key
4. Select **both** the certificate and private key
5. Right-click → **"Export 2 items..."**
6. Choose format: **Personal Information Exchange (.p12)**
7. Save as `distribution.p12`
8. Create a strong password when prompted

#### Convert to Base64

```bash
# macOS/Linux
base64 -i distribution.p12 > distribution.p12.base64.txt

# Windows PowerShell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("distribution.p12")) | Out-File distribution.p12.base64.txt -Encoding ASCII
```

Save:
- `IOS_DISTRIBUTION_CERTIFICATE_P12_BASE64`: Contents of the base64 file
- `IOS_DISTRIBUTION_CERTIFICATE_PASSWORD`: The password you created

### Step 4: Create Provisioning Profile

1. Go to [Profiles](https://developer.apple.com/account/resources/profiles/list)
2. Click **"+"** to create new profile
3. Select **"App Store Connect"** (under Distribution)
4. Click **"Continue"**
5. Select your App ID
6. Click **"Continue"**
7. Select your Distribution certificate
8. Click **"Continue"**
9. Enter profile name: `YourGame AppStore Distribution`
10. Click **"Generate"**
11. Download the `.mobileprovision` file

#### Convert to Base64

```bash
# macOS/Linux
base64 -i YourGame_AppStore_Distribution.mobileprovision > profile.base64.txt

# Windows PowerShell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("YourGame_AppStore_Distribution.mobileprovision")) | Out-File profile.base64.txt -Encoding ASCII
```

Save:
- `IOS_PROVISIONING_PROFILE_BASE64`: Contents of the base64 file
- `IOS_PROVISIONING_PROFILE_NAME`: Exact name you entered (e.g., `YourGame AppStore Distribution`)

### Step 5: Get Team ID

1. Go to [Apple Developer Account](https://developer.apple.com/account)
2. Look for **"Team ID"** in the membership section
3. It's a 10-character alphanumeric string (e.g., `ABC123XYZ9`)

Save `APPLE_TEAM_ID`: Your Team ID

### Step 6: Create App Store Connect API Key

1. Go to [App Store Connect](https://appstoreconnect.apple.com/)
2. Go to **Users and Access** → **Integrations** → **App Store Connect API**
3. Click **"+"** to generate a new key
4. Enter:
   - Name: `CI Upload Key`
   - Access: **App Manager**
5. Click **"Generate"**
6. **Important**: Download the `.p8` file immediately - you can only download it once!
7. Note the **Key ID** (shown in the table)
8. Note the **Issuer ID** (shown at the top of the page)

#### Convert to Base64

```bash
# macOS/Linux
base64 -i AuthKey_XXXXXXXXXX.p8 > authkey.base64.txt

# Windows PowerShell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("AuthKey_XXXXXXXXXX.p8")) | Out-File authkey.base64.txt -Encoding ASCII
```

Save:
- `APP_STORE_CONNECT_KEY_ID`: The Key ID (e.g., `ABC123DEFG`)
- `APP_STORE_CONNECT_ISSUER_ID`: The Issuer ID (UUID format)
- `APP_STORE_CONNECT_KEY_CONTENT`: Contents of the base64 file

### Step 7: Create App in App Store Connect

1. Go to [App Store Connect](https://appstoreconnect.apple.com/) → **Apps**
2. Click **"+"** → **"New App"**
3. Fill in:
   - Platform: iOS
   - Name: Your app name
   - Primary Language
   - Bundle ID: Select the one you created
   - SKU: Unique identifier (e.g., `yourgame001`)
   - User Access: Full Access
4. Click **"Create"**

---

## GitHub Secrets Configuration

Now add all the secrets to your GitHub repository.

### Adding Secrets

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **"New repository secret"** for each secret

### Complete Secrets Checklist

#### Unity (Required)
| Secret Name | Description | Example |
|-------------|-------------|---------|
| `UNITY_LICENSE` | Contents of `.ulf` file | `<?xml version="1.0"...` |
| `UNITY_EMAIL` | Unity account email | `you@example.com` |
| `UNITY_PASSWORD` | Unity account password | `your-password` |

#### Firebase (Required)
| Secret Name | Description | Example |
|-------------|-------------|---------|
| `FIREBASE_SERVICE_ACCOUNT_JSON` | Service account JSON contents | `{"type":"service_account"...}` |
| `FIREBASE_APP_ID_ANDROID` | Android Firebase App ID | `1:123456789:android:abc123` |
| `FIREBASE_APP_ID_IOS` | iOS Firebase App ID | `1:123456789:ios:def456` |

#### Android (Required for Android builds)
| Secret Name | Description | Example |
|-------------|-------------|---------|
| `ANDROID_KEYSTORE_BASE64` | Base64-encoded keystore | `MIIKfwIBAzCCC...` |
| `ANDROID_KEYSTORE_PASS` | Keystore password | `your-keystore-pass` |
| `ANDROID_KEY_ALIAS_NAME` | Key alias | `upload-key` |
| `ANDROID_KEY_ALIAS_PASS` | Key password | `your-key-pass` |
| `GPLAY_SERVICE_JSON` | Play Store service account JSON | `{"type":"service_account"...}` |

#### iOS (Required for iOS builds)
| Secret Name | Description | Example |
|-------------|-------------|---------|
| `APPLE_TEAM_ID` | Apple Developer Team ID | `ABC123XYZ9` |
| `IOS_BUNDLE_ID` | iOS Bundle Identifier | `com.company.game` |
| `IOS_DISTRIBUTION_CERTIFICATE_P12_BASE64` | Base64-encoded .p12 | `MIIKfwIBAzCCC...` |
| `IOS_DISTRIBUTION_CERTIFICATE_PASSWORD` | P12 password | `certificate-pass` |
| `IOS_PROVISIONING_PROFILE_BASE64` | Base64-encoded profile | `MIIKfwIBAzCCC...` |
| `IOS_PROVISIONING_PROFILE_NAME` | Profile name (exact) | `MyGame AppStore Distribution` |
| `APP_STORE_CONNECT_KEY_ID` | API Key ID | `ABC123DEFG` |
| `APP_STORE_CONNECT_ISSUER_ID` | API Issuer ID | `12345678-1234-...` |
| `APP_STORE_CONNECT_KEY_CONTENT` | Base64-encoded .p8 | `LS0tLS1CRUdJTi...` |

---

## Testing Your Setup

### Test 1: Verify Secrets

Create a test workflow to verify secrets are set:

```yaml
# .github/workflows/test-secrets.yml
name: Test Secrets
on: workflow_dispatch

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - name: Check Unity Secrets
        run: |
          if [ -n "${{ secrets.UNITY_LICENSE }}" ]; then
            echo "✅ UNITY_LICENSE is set"
          else
            echo "❌ UNITY_LICENSE is missing"
          fi
          if [ -n "${{ secrets.UNITY_EMAIL }}" ]; then
            echo "✅ UNITY_EMAIL is set"
          else
            echo "❌ UNITY_EMAIL is missing"
          fi
          # Add more checks as needed
```

### Test 2: Run Android Build

1. Push your changes to GitHub
2. Go to **Actions** tab
3. Find **"Android CI/CD"** workflow
4. Click **"Run workflow"** (if workflow_dispatch is enabled)
5. Or push to `master` branch to trigger automatically

### Test 3: Run iOS Build

1. Go to **Actions** tab
2. Find **"iOS CI/CD"** workflow
3. Click **"Run workflow"**

### Test 4: Verify Distribution

1. Check Firebase App Distribution for new builds
2. Testers should receive email notifications
3. Check Play Console / App Store Connect for store uploads

---

## Troubleshooting

### Unity License Issues

**Error: "No valid Unity license found"**
- Re-download and re-add `UNITY_LICENSE` secret
- Ensure the license matches your Unity version
- Check that `UNITY_EMAIL` and `UNITY_PASSWORD` are correct

**Error: "License file has expired"**
- Personal licenses need periodic refresh
- Repeat the activation process

### Firebase Issues

**Error: "Failed to authenticate"**
- Verify `FIREBASE_SERVICE_ACCOUNT_JSON` is valid JSON
- Check that the service account has Firebase App Distribution Admin role
- Regenerate the service account key if needed

**Error: "App not found"**
- Verify `FIREBASE_APP_ID_ANDROID` / `FIREBASE_APP_ID_IOS` format
- Ensure apps are added to your Firebase project

### Android Build Issues

**Error: "Keystore was tampered with"**
- Re-encode keystore to base64
- Ensure no whitespace or newlines in secret

**Error: "Could not find matching signature"**
- Verify `ANDROID_KEY_ALIAS_NAME` matches exactly
- Check passwords are correct

**Error: "APK/AAB not found"**
- Check the build output path in workflow
- Verify Unity build settings are configured for AAB

### iOS Build Issues

**Error: "No signing certificate found"**
- Verify certificate is a Distribution certificate (not Development)
- Re-export with private key included
- Check the P12 password is correct

**Error: "Provisioning profile doesn't match"**
- Profile must be for App Store distribution
- Bundle ID must match exactly
- Certificate used must match the one in the profile

**Error: "Code signing is required"**
- Ensure Team ID is correct
- Verify provisioning profile name matches exactly (case-sensitive)

### Play Store Issues

**Error: "Version code already exists"**
- GameCI uses semantic versioning; ensure version is incrementing
- Check build number in Unity Player Settings

**Error: "Package name mismatch"**
- Bundle ID must match the app in Play Console exactly
- Check Unity's Android Player Settings

### App Store Connect Issues

**Error: "Invalid API key"**
- Verify Key ID and Issuer ID are correct
- Check key has App Manager access
- Ensure base64 encoding is correct

**Error: "App not found"**
- Bundle ID must match exactly
- App must exist in App Store Connect

---

## Maintenance

### Renewing Certificates (iOS)

Distribution certificates expire after 1 year. To renew:

1. Create new certificate in Apple Developer Portal
2. Create new provisioning profile with new certificate
3. Update GitHub secrets:
   - `IOS_DISTRIBUTION_CERTIFICATE_P12_BASE64`
   - `IOS_DISTRIBUTION_CERTIFICATE_PASSWORD`
   - `IOS_PROVISIONING_PROFILE_BASE64`

### Rotating Service Account Keys

Periodically rotate keys for security:

1. Generate new key in Google Cloud / Firebase Console
2. Update `FIREBASE_SERVICE_ACCOUNT_JSON` or `GPLAY_SERVICE_JSON`
3. Delete old keys after verifying new ones work

### Unity Version Updates

When upgrading Unity:

1. Update `UNITY_VERSION` in workflow files
2. May need to re-activate Unity license for new version
3. Test builds locally before pushing

---

## Quick Reference

### Workflow Triggers

| Trigger | Android | iOS |
|---------|---------|-----|
| Push to master | ✅ Build + Firebase | ✅ Build + Firebase |
| Pull request | ✅ Build only | ✅ Build only |
| Manual dispatch | ✅ Optional store upload | ✅ Optional store upload |

### Fastlane Commands

```bash
# Android to Firebase only
bundle exec fastlane android distribute notes:"My release notes"

# Android to Firebase + Play Store
bundle exec fastlane android distribute upload_to_store:true

# iOS to Firebase only
bundle exec fastlane ios distribute notes:"My release notes"

# iOS to Firebase + TestFlight
bundle exec fastlane ios distribute upload_to_store:true
```

### File Locations

| File | Purpose |
|------|---------|
| `.github/workflows/android-build.yml` | Android CI workflow |
| `.github/workflows/ios-build.yml` | iOS CI workflow |
| `fastlane/Fastfile` | Fastlane lane definitions |
| `fastlane/Pluginfile` | Fastlane plugins |
| `Gemfile` | Ruby dependencies |

---

## Getting Help

- **GameCI Documentation**: [game.ci/docs](https://game.ci/docs)
- **Fastlane Documentation**: [docs.fastlane.tools](https://docs.fastlane.tools)
- **Firebase App Distribution**: [firebase.google.com/docs/app-distribution](https://firebase.google.com/docs/app-distribution)
- **GitHub Actions**: [docs.github.com/actions](https://docs.github.com/en/actions)

---

*Last updated: November 2025*
