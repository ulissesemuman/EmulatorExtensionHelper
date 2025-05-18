using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmulatorExtensionHelper
{
    internal class ContextMenuInstaller
    {
        private static LanguageManager lang = new LanguageManager();

        public static void CreateContextMenu(bool forAllUsers)
        {
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            // Raiz do menu de contexto
            string baseKey = forAllUsers ? @"*\shell\EmulatorHelper" : @"Software\Classes\*\shell\EmulatorHelper";
            using (var key = CreateSubKey(baseKey, forAllUsers))
            {
                //key.SetValue(null, lang.T("menu.Root")); // Nome exibido (resolvido via dicionário de idioma)
                key.SetValue("MUIVerb", lang.T("menu.Root")); // Nome multilíngue
                key.SetValue("Icon", exePath);
                key.SetValue("SubCommands", ""); // Habilita submenu
            }

            // Subcomando: Associar com emulador (pai)
            string parent1 = baseKey + @"\shell\Associate";
            using (var key = CreateSubKey(parent1, forAllUsers))
            {
                key.SetValue("MUIVerb", lang.T("menu.Associate"));
                key.SetValue("Icon", exePath);
                key.SetValue("SubCommands", ""); // Habilita submenu
            }

            // Subcomando: Associar diretamente
            string cmd1 = parent1 + @"\shell\AssociateThisFile";
            using (var key = CreateSubKey(cmd1, forAllUsers))
            {
                key.SetValue("MUIVerb", lang.T("menu.AssociateThisFile"));
                key.SetValue("Icon", exePath);
                CreateSubKey(cmd1 + @"\command", forAllUsers)
                    .SetValue(null, $"\"{exePath}\" --action=associateFileName --file=\"%1\"");
            }

            // Subcomando: Associar diretamente
            string cmd2 = parent1 + @"\shell\AssociateThisExtension";
            using (var key = CreateSubKey(cmd2, forAllUsers))
            {
                key.SetValue("MUIVerb", lang.T("menu.AssociateThisExtension"));
                key.SetValue("Icon", exePath);
                CreateSubKey(cmd2 + @"\command", forAllUsers)
                    .SetValue(null, $"\"{exePath}\" --action=associateExtension --file=\"%1\"");
            }

            // Subcomando: Associar com emulador (pai)
            string parent2 = baseKey + @"\shell\RemoveAssociation";
            using (var key = CreateSubKey(parent2, forAllUsers))
            {
                key.SetValue("MUIVerb", lang.T("menu.RemoveAssociation"));
                key.SetValue("Icon", exePath);
                key.SetValue("SubCommands", ""); // Habilita submenu
            }

            // Subcomando: Associar diretamente
            string cmd3 = parent2 + @"\shell\DisassociateThisFile";
            using (var key = CreateSubKey(cmd3, forAllUsers))
            {
                key.SetValue("MUIVerb", lang.T("menu.DisassociateThisFile"));
                key.SetValue("Icon", exePath);
                CreateSubKey(cmd3 + @"\command", forAllUsers)
                    .SetValue(null, $"\"{exePath}\" --action=disassociateFileName --file=\"%1\"");
            }

            // Subcomando: Associar diretamente
            string cmd4 = parent2 + @"\shell\DisassociateThisExtension";
            using (var key = CreateSubKey(cmd4, forAllUsers))
            {
                key.SetValue("MUIVerb", lang.T("menu.DisassociateThisExtension"));
                key.SetValue("Icon", exePath);
                CreateSubKey(cmd4 + @"\command", forAllUsers)
                    .SetValue(null, $"\"{exePath}\" --action=disassociateExtension --file=\"%1\"");
            }
        }

        public static void RemoveContextMenu()
        {
            try
            {
                var parentKey = Registry.LocalMachine.OpenSubKey(@"Software\Classes\*\shell\EmulatorHelper\", writable: false);

                if (parentKey != null)
                {
                    if (!Library.IsRunningAsAdmin())
                    {
                        var result = MessageBox.Show(
                                 lang.T("ContextMenuInstaller.RequireElevatedPermissions"),
                                 lang.T("Common.Confirmation"),
                                 MessageBoxButtons.OKCancel,
                                 MessageBoxIcon.Warning
                             );

                        if (result != DialogResult.OK)
                            return;

                        Library.RunAsAdmin();
                    }
                }

                parentKey = Registry.ClassesRoot.OpenSubKey(@"*\shell", writable: true);

                if (parentKey == null) 
                {
                    //MessageBox.Show("Chave pai não encontrada.");
                    return;
                }

                parentKey.DeleteSubKeyTree("EmulatorHelper", throwOnMissingSubKey: false);
  
                MessageBox.Show(
                            lang.T("ContextMenuInstaller.RemovalSuccess"),
                            lang.T("Common.Information"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
);
                foreach (var subKeyName in parentKey.GetSubKeyNames())
                {
                    Console.WriteLine($" - {subKeyName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static RegistryKey CreateSubKey(string subkey, bool forAllUsers)
        {
            return forAllUsers ? Registry.ClassesRoot.CreateSubKey(subkey) : Registry.CurrentUser.CreateSubKey(subkey);
        }
    }
}
