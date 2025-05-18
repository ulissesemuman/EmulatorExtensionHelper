🕹️ Emulator Helper
Emulator Helper is a lightweight Windows utility designed to simplify the process of associating ROM files with emulators through the Windows Explorer context menu. It allows users to configure which emulator to use for specific file extensions or even for individual ROM files, with just a few clicks.

✨ Features
🔗 File and Extension Associations
Associate individual ROM files or entire file extensions (e.g., .gba, .sfc, .bin, etc.) with one or more emulators.

📂 Context Menu Integration
Adds custom options to the right-click menu in Windows Explorer for quick access to your configured emulators.

👥 Per-User or All Users Installation
Choose whether to install context menu options for the current user only or for all users (requires elevation).

🧼 Uninstallation Tools
Easily remove all context menu entries and emulator associations created by the app.

🔄 Dynamic Emulator Selection
If more than one emulator is associated with a file or extension, you will be prompted to choose one at runtime.

🌐 Localization Support
All user-facing texts are fully localizable using a centralized lang.T("...") system. Default language: Portuguese (pt-BR).

🛠️ How It Works
The app manages a JSON configuration file (config.json) which stores:

Global emulator registrations with file extensions they support.

Per-file associations (identified using MD5 hash).

User preferences and language settings.

Associations are written to the Windows Registry using the Software\Classes path under HKEY_CURRENT_USER, ensuring a non-invasive, user-specific setup. If installing for all users, elevation is requested and registry entries are written under HKEY_LOCAL_MACHINE.

📦 Installation
Download the latest release from the Releases page.

Launch the app and choose:

"Add context menu for current user"

or "Add context menu for all users" (requires administrator rights).

Right-click any supported file type in Explorer and associate it with an emulator!

🧪 Example Use Case
You have multiple .bin and .cue files for PlayStation games. With Emulator Helper, you can:

Register DuckStation.exe or ePSXe.exe as emulators for .cue files.

Associate a specific .cue file with DuckStation.

Right-click any .cue and choose "Open with DuckStation" from the context menu — done!

🔐 Requirements
Windows 10 or later

.NET 6 or newer installed

Administrator privileges (only if installing for all users)

📝 License
This project is licensed under the MIT License.

📁 Project Structure (Developer Info)
pgsql
Copiar
Editar
📦 EmulatorHelper/
├── 🧠 Core logic (association, hashing, language management)
├── 📁 UI (WinForms-based main interface and dialogs)
├── 📄 config.json (auto-generated on first run)
├── 🌐 lang/ (language files, e.g., pt-br.json)
├── 📜 LICENSE
└── 📘 README.md
🤝 Contributions
Contributions, suggestions, and translations are welcome! Feel free to fork the project or open issues to report bugs or request features.
