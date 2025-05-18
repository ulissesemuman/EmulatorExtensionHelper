using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace EmulatorExtensionHelper
{
    public partial class frmHelper : Form
    {
        // Constantes
        private const int BCM_SETSHIELD = 0x160C;

        private static LanguageManager lang = new LanguageManager("language", "config.json");

        [DllImport("user32")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public frmHelper()
        {
            InitializeComponent();

            cmdRemoveContextMenu.Name = nameof(cmdRemoveContextMenu);
            cmdRemoveContextMenu.Text = lang.T("MainForm.RemoveContextMenu");

            cmdExit.Name = nameof(cmdExit);
            cmdExit.Text = lang.T("Common.Exit");

            groupBox1.Name = nameof(groupBox1);
            groupBox1.Text = lang.T("MainForm.GroupBoxAddContext");

            cmdCurrentUser.Name = nameof(cmdCurrentUser);
            cmdCurrentUser.Text = lang.T("MainForm.CurrentUser");

            cmdAllUsers.Name = nameof(cmdAllUsers);
            cmdAllUsers.Text = lang.T("MainForm.AllUsers");

            cmdRemoveAllAssociations.Name = nameof(cmdRemoveAllAssociations);
            cmdRemoveAllAssociations.Text = lang.T("MainForm.RemoveAllAssociations");

            cmdAllUsers.FlatStyle = FlatStyle.System; // Necessário para que o botão aceite o escudo
            ShowShield(cmdAllUsers, !Library.IsRunningAsAdmin());
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdRemoveContextMenu_Click(object sender, EventArgs e)
        {
            ContextMenuInstaller.RemoveContextMenu();
        }

        private void ShowShield(Button b, bool show)
        {
            SendMessage(b.Handle, BCM_SETSHIELD, IntPtr.Zero, new IntPtr(show ? 1 : 0));
        }

        private void cmdAllUsers_Click(object sender, EventArgs e)
        {
            if (!Library.IsRunningAsAdmin())
            {
                Library.RunAsAdmin();

                return;
            }

            ContextMenuInstaller.CreateContextMenu(true);

            MessageBox.Show("Menu de contexto adicionado com sucesso.");
        }

        private void cmdCurrentUser_Click(object sender, EventArgs e)
        {
            ContextMenuInstaller.CreateContextMenu(false);

            MessageBox.Show("Menu de contexto adicionado com sucesso.");
        }

        private void frmHelper_Load(object sender, EventArgs e)
        {

        }

        private void cmdRemoveAllAssociations_Click(object sender, EventArgs e)
        {
            FileAssociationHelper.RemoveAllEmulatorExtensionHelperiations();
        }
    }
}
