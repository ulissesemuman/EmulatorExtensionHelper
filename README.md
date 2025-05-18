
# 🚀 Emulator Helper

A lightweight Windows utility for easily associating ROM files with your favorite emulators via the Explorer context menu.

![Context Menu](https://raw.githubusercontent.com/your-username/your-repo/main/assets/context-menu.png)
![Selection UI](https://raw.githubusercontent.com/your-username/your-repo/main/assets/selection-ui.png)

## Features

- ✅ Add or remove context menu options for specific file types
- 🎮 Associate specific ROM files or entire extensions with one or more emulators
- 🔄 Support for multi-disc games and automatic renaming patterns
- 🧠 Smart emulator name detection from metadata and registry
- 👤 Support for per-user or all-user installations
- 🗑️ Remove individual associations or clean all registry entries created by the app

## How It Works

The application registers context menu entries under:
- `HKEY_CURRENT_USER\Software\Classes`
- (Optional) `HKEY_LOCAL_MACHINE\Software\Classes` for all-users mode

When a user right-clicks a file, the launcher is invoked with parameters like:

```sh
EmulatorHelper.exe --action=associate --file="C:\Games\Chrono Trigger (Disc 1).bin"
```

The launcher will then show a friendly UI listing all configured emulators, allowing users to:

- Select or modify emulator associations
- Launch the ROM immediately
- Remove existing associations

## Emulator Configuration

Emulators are managed via a centralized `config.json` file:

```json
{
  "language": "pt-br",
  "emulators": {
    "RetroArch": {
      "path": "C:\Emulators\RetroArch\retroarch.exe",
      "extensions": [".sfc", ".gba", ".bin"]
    }
  }
}
```

## Localization

All user-facing messages are fully localized. English and Brazilian Portuguese are supported out-of-the-box.

## Installation

- Simply run the executable to register context menu options.
- Choose between **Current User** or **All Users** mode.
- Administrator privileges required for system-wide installations.

## Requirements

- Windows 10 or 11
- .NET 6.0 Runtime

## License

This project is licensed under the **Unlicense**, which means:
> You may use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software — no restrictions.

## Contribution

PRs are welcome! Please ensure all `lang.T(...)` messages are localized using the existing JSON structure.

---

© You. Free forever.
