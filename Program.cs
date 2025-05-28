using System;
using System.CodeDom.Compiler;
using System.Diagnostics.Metrics;
using System.Security.Principal;

namespace EmulatorExtensionHelper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // Nenhum par�metro = abre a interface para instalar/remover contexto
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmHelper()); // <-- janela com os dois bot�es
            }
            else
            {
               // MessageBox.Show(string.Join(" ", args), "Par�metros recebidos", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //string[] a = { "--action=associateExtension", "--file=F:\\Jogos\\Emuladores\\ROMs\\Master System\\Aztec Adventure -The Golden Road to Paradise.sms" };

                //string[] a = { "--action=execute", "--file=F:\\Jogos\\Emuladores\\ROMs\\Mega Drive\\Faery Tale Adventure, The (USA, Europe).md" };
                //string[] a = { "--action=associateExtension", "--file=F:\\Jogos\\Emuladores\\ROMs\\Mega Drive\\Faery Tale Adventure, The (USA, Europe).md" };
                //string[] a = { "--action=disassociateExtension", "--file=F:\\Jogos\\Emuladores\\ROMs\\Mega Drive\\Faery Tale Adventure, The (USA, Europe).md" };

                //args = a;

                // Executa l�gica sem interface com base nos par�metros
                ContextActions.Execute(args); 
            }
        }
    }
}