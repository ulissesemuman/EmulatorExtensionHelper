using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{
    internal static class ExecutableInfoHelper
    {
        private static LanguageManager lang = new LanguageManager("language", "config.json");

        public static string GetFriendlyNameFromExecutable(string exePath)
        {
            if (!File.Exists(exePath))
                throw new FileNotFoundException(lang.T("ExecutableInfo.FileNotFound"), exePath);

            // 1. FileVersionInfo - FileDescription ou ProductName
            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(exePath);
                if (!string.IsNullOrWhiteSpace(versionInfo.FileDescription))
                    return versionInfo.FileDescription.Trim();
                if (!string.IsNullOrWhiteSpace(versionInfo.ProductName))
                    return versionInfo.ProductName.Trim();
            }
            catch { }

            // 2. RegisteredApplications (HKEY_LOCAL_MACHINE)
            try
            {
                using (var regApps = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\RegisteredApplications"))
                {
                    if (regApps != null)
                    {
                        foreach (var appName in regApps.GetValueNames())
                        {
                            var relPath = regApps.GetValue(appName)?.ToString();
                            if (!string.IsNullOrEmpty(relPath))
                            {
                                using (var capabilities = Registry.LocalMachine.OpenSubKey(relPath))
                                {
                                    var exe = capabilities?.GetValue("ApplicationIcon")?.ToString()?.Split(',')[0]?.Trim('"');
                                    if (!string.IsNullOrEmpty(exe) && File.Exists(exe) && Path.GetFullPath(exe).Equals(Path.GetFullPath(exePath), StringComparison.OrdinalIgnoreCase))
                                        return appName;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // 3. StartMenuInternet Clients (HKEY_LOCAL_MACHINE)
            try
            {
                using (var startMenuClients = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet"))
                {
                    if (startMenuClients != null)
                    {
                        foreach (var subKeyName in startMenuClients.GetSubKeyNames())
                        {
                            using (var clientKey = startMenuClients.OpenSubKey(subKeyName))
                            {
                                var friendlyName = clientKey?.GetValue(null)?.ToString();
                                var exe = clientKey?.OpenSubKey(@"shell\open\command")?.GetValue(null)?.ToString();

                                if (!string.IsNullOrEmpty(exe))
                                {
                                    string exeInCommand = ExtractExePath(exe);
                                    if (File.Exists(exeInCommand) && Path.GetFullPath(exeInCommand).Equals(Path.GetFullPath(exePath), StringComparison.OrdinalIgnoreCase))
                                        return friendlyName ?? subKeyName;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // 4. Fallback: nome do executável sem extensão
            return Path.GetFileNameWithoutExtension(exePath);
        }

        private static string ExtractExePath(string command)
        {
            if (string.IsNullOrEmpty(command)) return "";
            // Exemplo: "C:\Program Files\App\app.exe" "%1"
            if (command.StartsWith("\""))
            {
                int endQuote = command.IndexOf('"', 1);
                if (endQuote > 1)
                    return command.Substring(1, endQuote - 1);
            }
            else
            {
                int firstSpace = command.IndexOf(' ');
                if (firstSpace > 0)
                    return command.Substring(0, firstSpace);
                return command;
            }
            return command;
        }
    }
}
