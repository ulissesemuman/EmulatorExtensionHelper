using System;
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
                // Nenhum parâmetro = abre a interface para instalar/remover contexto
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmHelper()); // <-- janela com os dois botões
            }
            else
            {
                //MessageBox.Show(string.Join(" ", args), "Parâmetros recebidos", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //string[] a = { "--action=associateExtension", "--file=F:\\Jogos\\Emuladores\\ROMs\\Master System\\Aztec Adventure -The Golden Road to Paradise.sms" };

                //string[] a = { "--action=associateExtension", "--file=F:\\Jogos\\Emuladores\\ROMs\\Mega Drive\\Faery Tale Adventure, The (USA, Europe).md" };

                //args = a;

                // Executa lógica sem interface com base nos parâmetros
                ContextActions.Execute(args); // <-- seu código para lidar com ações do menu
            }
        }
    }
}