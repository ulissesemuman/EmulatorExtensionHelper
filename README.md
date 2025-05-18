
# 🕹️ Emulator Helper

Emulator Helper is a lightweight Windows utility designed to simplify the process of associating ROM files with emulators through the Windows Explorer context menu. It allows users to configure a list of emulators to use for specific file extensions or even for individual ROM files, with just a few clicks.

![Context Menu](https://raw.githubusercontent.com/your-username/your-repo/main/assets/context-menu.png)
![Selection UI](https://raw.githubusercontent.com/your-username/your-repo/main/assets/selection-ui.png)

## ✨ Features

- 🔗 File and Extension Associations

     Associate individual ROM files or entire file extensions (e.g., .gba, .sfc, .bin, etc.) with one or more emulators.
- 🎮 Associate specific ROM files or entire extensions with one or more emulators
- 🧠 Smart emulator name detection from metadata and registry
- 👤 Support for per-user or all-user installations
- 🗑️ Remove individual associations or clean all registry entries created by the app

## 🛠️ How It Works

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

## 💬 Localization

All user-facing messages are fully localized. English and Brazilian Portuguese are supported out-of-the-box.

## 📦 Installation

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
