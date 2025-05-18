using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{
    internal static class FileAssociationHelper
    {
        private static LanguageManager lang = new LanguageManager("language", "config.json");

        public static void AssociateExtensionToLauncher(string extension)
        {
            if (!extension.StartsWith("."))
                throw new ArgumentException(lang.T("FileAssociationHelper.InvalidExtension"));

            string progId = $"EmulatorExtensionHelper{extension}";
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName
                             ?? throw new InvalidOperationException(lang.T("FileAssociationHelper.MissingExecutablePath"));

            // Associa a extensão ao progId
            using (RegistryKey extKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{extension}"))
            {
                extKey.SetValue("", progId);
            }

            // Cria o progId apontando para este launcher
            using (RegistryKey progIdKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{progId}"))
            {
                progIdKey.SetValue("", lang.T("FileAssociationHelper.FriendlyAppName"));

                using (RegistryKey commandKey = progIdKey.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", $"\"{exePath}\" --action=execute --file=\"%1\"");
                }
            }
        }

        public static void DisassociateExtensionFromLauncher(string extension)
        {
            if (!extension.StartsWith("."))
                throw new ArgumentException(lang.T("FileAssociationHelper.InvalidExtension"));

            string progId = $"EmulatorExtensionHelper{extension}";

            try
            {
                // Remove a associação da extensão, se estiver associada ao nosso progId
                using (RegistryKey extKey = Registry.CurrentUser.OpenSubKey($@"Software\Classes\{extension}", writable: true))
                {
                    if (extKey != null)
                    {
                        var current = extKey.GetValue("") as string;
                        if (string.Equals(current, progId, StringComparison.OrdinalIgnoreCase))
                        {
                            extKey.DeleteValue("", false);
                        }
                    }
                }

                // Remove o progId (toda a chave)
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\{progId}", throwOnMissingSubKey: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{lang.T("FileAssociationHelper.DisassociateError")}\n{ex.Message}", lang.T("Common.Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void RemoveAllEmulatorExtensionHelperiations()
        {
            try
            {
                using (RegistryKey classesKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes", writable: true))
                {
                    if (classesKey == null)
                        throw new InvalidOperationException(lang.T("FileAssociationHelper.RegistryClassesNotFound"));

                    List<string> keysToDelete = new List<string>();

                    foreach (string subKeyName in classesKey.GetSubKeyNames())
                    {
                        // Remover os progIds do tipo EmulatorExtensionHelper.*
                        if (subKeyName.StartsWith("EmulatorExtensionHelper", StringComparison.OrdinalIgnoreCase))
                        {
                            keysToDelete.Add(subKeyName);
                            continue;
                        }

                        // Verificar se uma extensão está associada ao nosso progId e limpar
                        using (RegistryKey extKey = classesKey.OpenSubKey(subKeyName, writable: true))
                        {
                            if (extKey != null)
                            {
                                var defaultValue = extKey.GetValue("") as string;
                                if (!string.IsNullOrEmpty(defaultValue) && defaultValue.StartsWith("EmulatorExtensionHelper"))
                                {
                                    extKey.DeleteValue("", false);
                                }
                            }
                        }
                    }

                    // Agora removemos as chaves dos progIds
                    foreach (string key in keysToDelete)
                    {
                        classesKey.DeleteSubKeyTree(key, throwOnMissingSubKey: false);
                    }

                    MessageBox.Show(lang.T("FileAssociationHelper.AllAssociationsRemoved"), lang.T("Common.Success"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{lang.T("FileAssociationHelper.DisassociateError")}\n{ex.Message}", lang.T("Common.Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
