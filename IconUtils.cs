using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{
    internal static class IconUtils
    {
        [DllImport("shell32.dll", EntryPoint = "ExtractIconEx", CharSet = CharSet.Auto)]
        private static extern int ExtractIconEx(string lpszFile, int nIconIndex,
            IntPtr[] phiconLarge, IntPtr[] phiconSmall, int nIcons);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static Icon ExtractIcon(string exePath, int index = 0, bool large = true)
        {
            IntPtr[] largeIcons = new IntPtr[1];
            IntPtr[] smallIcons = new IntPtr[1];

            int result = ExtractIconEx(exePath, index, large ? largeIcons : null, !large ? smallIcons : null, 1);

            if (result > 0)
            {
                IntPtr iconHandle = large ? largeIcons[0] : smallIcons[0];
                if (iconHandle != IntPtr.Zero)
                {
                    Icon icon = (Icon)Icon.FromHandle(iconHandle).Clone();
                    DestroyIcon(iconHandle); // Libera o handle nativo
                    return icon;
                }
            }

            return null;
        }

        public static string SaveEmulatorIcon(Icon icone, string nomeEmulador)
        {
            if (icone == null)
                throw new ArgumentNullException(nameof(icone));

            // Define o caminho da pasta "icon\[nome_do_emulador]"
            string pastaDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon", nomeEmulador);
            Directory.CreateDirectory(pastaDestino); // Garante que a pasta existe

            // Caminho completo do arquivo
            string caminhoArquivo = Path.Combine(pastaDestino, nomeEmulador + ".png");

            // Salva o ícone como PNG
            using (Bitmap bitmap = icone.ToBitmap())
            {
                bitmap.Save(caminhoArquivo, ImageFormat.Png);
            }

            return caminhoArquivo; // Retorna o caminho do arquivo salvo
        }
    }
}
