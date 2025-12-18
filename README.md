# Unity Game CI with Fastlane

Automated CI/CD pipeline for Unity games targeting Android and iOS platforms. This project uses [GameCI](https://game.ci/) for Unity builds and [Fastlane](https://fastlane.tools/) for distribution to Firebase App Distribution and app stores.

## Features

- ✅ Automated Unity builds for Android (AAB) and iOS (IPA)
- ✅ Firebase App Distribution for beta testing
- ✅ Google Play Store deployment (Android)
- ✅ App Store Connect / TestFlight deployment (iOS)
- ✅ Semantic versioning with automatic version code management
- ✅ Release notes generation from git commits
- ✅ Manual workflow dispatch with optional store upload

## Quick Start

1. Click **"Use this template"** to create your own repository
2. Follow the [Complete Setup Guide](docs/SETUP_GUIDE.md)
3. Copy `Assets/google-services.json.template` → `Assets/google-services.json` and fill in your Firebase config
4. Configure GitHub secrets
5. Push to trigger builds!

## Documentation

📖 **[docs/](docs/README.md)** - All documentation

| Document | Description |
|----------|-------------|
| [SETUP_GUIDE.md](docs/SETUP_GUIDE.md) | Complete CI/CD setup (Unity license, Firebase, signing, secrets) |
| [Sorolla SDK](Packages/com.sorolla.sdk/Documentation~/) | Mobile publisher SDK documentation |

## Project Structure

```
├── .github/workflows/           # CI/CD workflows (Android, iOS)
├── fastlane/                    # Fastlane lanes and plugins
├── docs/                        # Documentation
│   ├── SETUP_GUIDE.md          # CI/CD setup guide
│   ├── ai-guidelines/          # AI assistant coding rules
│   └── development/            # Dev session logs
├── Packages/
│   ├── com.sorolla.sdk/        # Mobile publisher SDK
│   └── com.lacrearthur.facebook-sdk-for-unity/
├── Assets/                      # Unity assets
├── Gemfile                      # Ruby dependencies
└── CLAUDE.md                    # Claude Code context
```

## GitHub Secrets Summary

See [SETUP_GUIDE.md](docs/SETUP_GUIDE.md) for detailed instructions on obtaining each secret.

### Unity
| Secret | Description |
|--------|-------------|
| `UNITY_LICENSE` | Unity license file content |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password |

### Firebase
| Secret | Description |
|--------|-------------|
| `FIREBASE_SERVICE_ACCOUNT_JSON` | Firebase service account JSON |
| `FIREBASE_APP_ID_ANDROID` | Firebase App ID for Android |
| `FIREBASE_APP_ID_IOS` | Firebase App ID for iOS |

### Android
| Secret | Description |
|--------|-------------|
| `ANDROID_PACKAGE_NAME` | Android package name (e.g., `com.company.game`) |
| `ANDROID_KEYSTORE_BASE64` | Base64-encoded keystore file |
| `ANDROID_KEYSTORE_PASS` | Keystore password |
| `ANDROID_KEY_ALIAS_NAME` | Key alias name |
| `ANDROID_KEY_ALIAS_PASS` | Key alias password |
| `GPLAY_SERVICE_JSON` | Google Play service account JSON |

### iOS
| Secret | Description |
|--------|-------------|
| `APPLE_TEAM_ID` | Apple Developer Team ID |
| `IOS_BUNDLE_ID` | iOS app bundle identifier |
| `IOS_DISTRIBUTION_CERTIFICATE_P12_BASE64` | Distribution certificate |
| `IOS_DISTRIBUTION_CERTIFICATE_PASSWORD` | Certificate password |
| `IOS_PROVISIONING_PROFILE_BASE64` | Provisioning profile |
| `IOS_PROVISIONING_PROFILE_NAME` | Profile name (exact match) |
| `APP_STORE_CONNECT_KEY_ID` | App Store Connect API Key ID |
| `APP_STORE_CONNECT_ISSUER_ID` | App Store Connect Issuer ID |
| `APP_STORE_CONNECT_KEY_CONTENT` | Base64-encoded API key (.p8) |

## Workflows

| Workflow | Trigger | Output | Distribution |
|----------|---------|--------|--------------|
| Android CI/CD | Push/PR to `master`, Manual | AAB | Firebase + Play Store |
| iOS CI/CD | Push/PR to `master`, Manual | IPA | Firebase + TestFlight |

## Local Development

```bash
# Install dependencies
bundle install

# Android distribution
bundle exec fastlane android distribute notes:"Test build"

# iOS distribution  
bundle exec fastlane ios distribute notes:"Test build"

# With store upload
bundle exec fastlane android distribute upload_to_store:true
```

## Customization

### Change Tester Groups

Edit `fastlane/Fastfile`:
```ruby
groups: "qa-testers, beta-testers"  # Comma-separated Firebase group names
```

### Change Play Store Track

```bash
bundle exec fastlane android distribute upload_to_store:true track:beta
# Available: internal, alpha, beta, production
```

## Troubleshooting

See the [Troubleshooting section](docs/SETUP_GUIDE.md#troubleshooting) in the setup guide.

## Resources

- [GameCI Documentation](https://game.ci/docs)
- [Fastlane Documentation](https://docs.fastlane.tools)
- [Firebase App Distribution](https://firebase.google.com/docs/app-distribution)

## License

This project is provided as-is for educational and reference purposes.
