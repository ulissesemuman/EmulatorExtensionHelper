
# 🕹️ Emulator Helper

Emulator Helper is a lightweight Windows utility designed to simplify the process of associating ROM files with emulators through the Windows Explorer context menu. It allows users to configure a list of emulators to use for specific file extensions or even for individual ROM files, with just a few clicks.

![Context Menu](https://raw.githubusercontent.com/your-username/your-repo/main/assets/context-menu.png)
![Selection UI](https://raw.githubusercontent.com/your-username/your-repo/main/assets/selection-ui.png)

## ✨ Features

- 🔗 File and Extension Associations

     Associate individual ROM files or entire file extensions (e.g., .gba, .sfc, .bin, etc.) with one or more emulators.
- 📂 Context Menu Integration

     Adds custom options to the right-click menu in Windows Explorer for quick access to your configured emulators.

- 🔄 Dynamic Emulator Selection

     If more than one emulator is associated with a file or extension, you will be prompted to choose one at runtime.
- 👤 Support for per-user or all-user installations

     Choose whether to install context menu options for the current user only or for all users (requires elevation).
- 🗑️ Uninstallation Tools

     Remove individual associations or clean all registry entries created by the app
- 🌐 Localization Support

     All user-facing texts are fully localizable using a centralized lang.T("...") system. Default language: Portuguese (en-US).
     The Emulator Helper interface currently supports the following languages:
     - 🇧🇷 Portuguese (Brazil)
     - 🇺🇸 English (United States)
     
     The application is fully localizable. Language files are stored in a simple JSON format, allowing easy customization or translation into additional languages. If you'd like to contribute a translation, feel free to submit a pull request or open an issue!  
- 🧠 Smart emulator name detection

     Automatically extracts a user-friendly name from the emulator executable or system registry.

## 🛠️ How It Works

The app manages a JSON configuration file (config.json) which stores:

- Global emulator registrations with file extensions they support.

- Per-file associations (identified using MD5 hash).

- User preferences and language settings.

Associations are written to the Windows Registry using the Software\Classes path under HKEY_CURRENT_USER, ensuring a non-invasive, user-specific setup. If installing for all users, elevation is requested and registry entries are written under HKEY_LOCAL_MACHINE.

The launcher will then show a friendly UI listing all configured emulators, allowing users to:

- Select or modify emulator associations
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

1. Download the latest release from the Releases page.
2. Launch the app and choose:
- "Add context menu for current user"
- or "Add context menu for all users" (requires administrator rights).
3. Right-click any supported file type in Explorer and associate it with an emulator!

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
