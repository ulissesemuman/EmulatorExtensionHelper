using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Principal;
using static EmulatorExtensionHelper.ContextActions;
using static EmulatorExtensionHelper.frmEmulatorSelector;

namespace EmulatorExtensionHelper
{
    public partial class frmHelper : Form
    {
        // Constantes
        private const int BCM_SETSHIELD = 0x160C;
        private string _currentLanguage = string.Empty;

        private static LanguageManager lang = new LanguageManager();

        [DllImport("user32")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public frmHelper()
        {
            InitializeComponent();

            ReloadControlsText();

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

            ContextMenuInstaller.CreateContextMenu(true, false);

            MessageBox.Show("Menu de contexto adicionado com sucesso.");
        }

        private void cmdCurrentUser_Click(object sender, EventArgs e)
        {
            ContextMenuInstaller.CreateContextMenu(false, false);

            MessageBox.Show("Menu de contexto adicionado com sucesso.");
        }

        private void frmHelper_Load(object sender, EventArgs e)
        {
            ConfigManager.EnsureConfigFileExists();
            LanguageManager.EnsureLanguageFolderExists();
            LanguageManager.EnsureDefaultLanguageFileAsync();

            _currentLanguage = ConfigManager.GetConfig().Language.ToLower();

            cmbSelectLanguage.DataSource = new BindingSource(LanguageManager.GetLanguageDisplayNames(), null);
            cmbSelectLanguage.DisplayMember = "Value";
            cmbSelectLanguage.ValueMember = "Key";

            string isoToSelect = System.Globalization.CultureInfo.CurrentUICulture.Name.ToLower();

            FindAndSelectLanguage(isoToSelect);

            if (cmbSelectLanguage.SelectedItem is null)
            {
                FindAndSelectLanguage(LanguageManager.DefaultLanguage);
            }

#if DEBUG
            ComboBox cmb = new ComboBox();
            cmb.Location = new Point(this.Width / 2 - 180, this.Height / 2 + 60);
            cmb.Items.Add("execute");
            cmb.Items.Add("associatefilename");
            cmb.Items.Add("associateextension");
            cmb.Items.Add("disassociatefilename");
            cmb.Items.Add("disassociateextension");
            cmb.SelectedIndex = 0;
            this.Controls.Add(cmb);
            Button btn = new Button();
            btn.Text = "Debug";
            btn.Size = new Size(70, 30);
            btn.Location = new Point(this.Width / 2 + 50, this.Height / 2 + 60);
            btn.Visible = true;
            btn.Click += (s, e) =>
            {
                Debug(cmb);
            };
            this.Controls.Add(btn);
#endif
        }

        private void cmdRemoveAllAssociations_Click(object sender, EventArgs e)
        {
            FileAssociationHelper.RemoveAllEmulatorExtensionHelperiations();
        }

        private void FindAndSelectLanguage(string isoCode)
        {
            foreach (KeyValuePair<string, string> item in cmbSelectLanguage.Items)
            {
                if (item.Key.Equals(isoCode, StringComparison.OrdinalIgnoreCase))
                {
                    cmbSelectLanguage.SelectedItem = item;
                    break;
                }
            }
        }

        private void cmbSelectLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSelectLanguage.SelectedItem is KeyValuePair<string, string> selectedItem)
            {
                string selectedKey = selectedItem.Key;
                string selectedValue = selectedItem.Value;

                if (selectedKey == _currentLanguage)
                    return;

                _currentLanguage = selectedKey;

                ConfigManager.UpdateLanguage(selectedKey);

                lang = new LanguageManager();

                ReloadControlsText();

                if (Registry.ClassesRoot.OpenSubKey(@"*\shell\EmulatorHelper\", writable: false) != null)
                {
                    ContextMenuInstaller.CreateContextMenu(false, true);
                }
            }
        }

        private void ReloadControlsText()
        {
            cmdRemoveContextMenu.Text = lang.T("MainForm.RemoveContextMenu");
            cmdExit.Text = lang.T("Common.Exit");
            groupBox1.Text = lang.T("MainForm.GroupBoxAddContext");
            cmdCurrentUser.Text = lang.T("MainForm.CurrentUser");
            cmdAllUsers.Text = lang.T("MainForm.AllUsers");
            cmdRemoveAllAssociations.Text = lang.T("MainForm.RemoveAllAssociations");
            label1.Text = lang.T("MainForm.SelectLanguage");
        }

        private void Debug(ComboBox cmb) 
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select a file",
                Filter = "All files (*.*)|*.*",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
 
                string[] args = new string[] { "--action=" + cmb.Items[cmb.SelectedIndex].ToString(), "--file=" + filePath };
                
                //this.Close();

                ContextActions.FormExecutionMode = FormExecutionModes.ShowDialog;
                ContextActions.Execute(args);
            }
        }
    }
}
