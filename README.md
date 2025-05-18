
# 🕹️ Emulator Helper

Emulator Helper is a lightweight Windows utility designed to simplify the process of associating ROM files with emulators through the Windows Explorer context menu. It allows users to configure a list of emulators to use for specific file extensions or even for individual ROM files, with just a few clicks.

## 📸 Screenshots

![Context Menu](https://raw.githubusercontent.com/ulissesemuman/EmulatorExtensionHelper/master/assets/ContextMenuAssociate.png)

![Emulator Selection Menu](https://raw.githubusercontent.com/ulissesemuman/EmulatorExtensionHelper/refs/heads/master/assets/SelectEmulator.png)

![Configuration UI](https://raw.githubusercontent.com/ulissesemuman/EmulatorExtensionHelper/master/assets/ConfigurationInterface.png)

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
- 🧠 Smart emulator naming

     Automatically extracts friendly emulator names from metadata, registry, and manifests.
- 🔧 Simple configuration

     All settings are stored in a central config.json file.  

## 🛠️ How It Works

The app manages a JSON configuration file (config.json) which stores:

- Global emulator registrations with file extensions they support.

- Per-file associations (identified using MD5 hash).

- User preferences and language settings.

Associations are written to the Windows Registry using the Software\Classes path under HKEY_CURRENT_USER, ensuring a non-invasive, user-specific setup. If installing for all users, elevation is requested and registry entries are written under HKEY_LOCAL_MACHINE.

The launcher will then show a friendly UI listing all configured emulators, allowing users to:

- Select or modify emulator associations
- Remove existing associations

## ⚙️Emulator Configuration

Emulators are managed via a centralized `config.json` file:

```json
{
  "language": "pt-br",
  "emulators": {
    "RetroArch": {
      "path": "C:\\Emulators\\RetroArch\\retroarch.exe",
      "extensions": ["gba", "sfc"]
    }
  },
  "files": {
    "a1b2c3d4...": {
      "path": "C:\\ROMs\\Metroid.gba",
      "emulators": ["RetroArch"]
    }
  }
}
```
- language: Interface language.
- emulators: Emulator definitions with executable paths and associated extensions.
- files: Per-ROM associations tracked by MD5 hash.

## 💬 Localization

All user-facing messages are fully localized. English and Brazilian Portuguese are supported out-of-the-box.

Interface localization is fully supported. Translations are defined via JSON. Example:
```json
{
  "menu.Associate": "Associar",
  "menu.AssociateThisExtension": "Associar a extensão a um emulador",
  ...
}
```

## 📦 Installation

1. Download the latest release from the [![Releases](https://img.shields.io/github/v/release/ulissesemuman/EmulatorExtensionHelper?label=release)](https://github.com/ulissesemuman/EmulatorExtensionHelper/releases) page.
2. Launch the app and choose:
     - "Add context menu for current user"
     - or "Add context menu for all users" (requires administrator rights).

## Uninstallation

Use the app’s interface to:
- Remove all context menu entries.
- Remove all file and extension associations made by the program.
  
## 🛠️ Usage

1. Right-click any file type in Explorer to show options:
- "Associate extension with emulator"
- "Associate file with emulator"
- "Remove extension association from emulator"
- "Remove file association from emulator"

2. Double-click any file type in Explorer to open menu with associated emulators.
Note: The first time you double-click a supported file type, you’ll be prompted to enable the Emulator Helper. This helper allows the launcher to display a list of associated emulators when opening a file. Without enabling it, direct launching is not possible due to system permission restrictions for file type associations.

## 📸 Screenshots

## 🔐 Requirements

- Windows 10 or later
- .NET 6 or newer installed
- Administrator privileges (only if installing for all users)

## 📝 License

This project is licensed under the **MIT License**, which means:
> You may use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software — must be provided with the original copyright notice and attribution to the developer are included.

## 📁 Project Structure (Developer Info)
```
📦 EmulatorHelper/
├── 🧠 Core logic (association, hashing, language management)
├── 📁 UI (WinForms-based main interface and dialogs)
├── 📄 config.json (auto-generated on first run)
├── 🌐 language/ (language files, e.g., pt-br.json)
├── 📜 LICENSE.txt
└── 📘 README.md
```

## 🤝 Contributions

Contributions, suggestions, and translations are welcome (please ensure all `lang.T(...)` messages are localized using the existing JSON structure)! Feel free to fork the project or open issues to report bugs or request features.

---

© You. Free forever.
