using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmulatorExtensionHelper
{
    internal static class Library
    {
        private static LanguageManager lang = new LanguageManager();

        public static bool IsRunningAsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RunAsAdmin()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Verb = "runas", // <- Isso faz o UAC aparecer
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                Application.Exit(); // Opcional: fecha a versão atual
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223) // Código quando o usuário clica "Não" no UAC
                {
                    MessageBox.Show(lang.T("Library.ElevationRequiredCanceled"), lang.T("Common.Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
