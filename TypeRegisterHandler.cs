using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorExtensionHelper
{
    internal static class TypeRegisterHandler
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHOpenWithDialog(IntPtr hwndParent, ref OPENASINFO info);

        private enum OAIF
        {
            OAIF_ALLOW_REGISTRATION = 0x00000001,
            OAIF_REGISTER_EXT = 0x00000002,
            OAIF_EXEC = 0x00000004,
            OAIF_FORCE_REGISTRATION = 0x00000008,
            OAIF_HIDE_REGISTRATION = 0x00000020,
            OAIF_URL_PROTOCOL = 0x00000040,
            OAIF_FILE_IS_URI = 0x00000080,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OPENASINFO
        {
            public string pcszFile;
            public string pcszClass;
            public OAIF oaifInFlags;
        }

        public static bool RegisterType(string fileName)
        {
            var info = new OPENASINFO
            {
                pcszFile = fileName,
                pcszClass = null,
                oaifInFlags = OAIF.OAIF_ALLOW_REGISTRATION | OAIF.OAIF_EXEC | OAIF.OAIF_REGISTER_EXT
            };

            int result = SHOpenWithDialog(IntPtr.Zero, ref info);

            return result == 0;
        }
    }
}
