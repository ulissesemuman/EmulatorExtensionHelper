using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EmulatorExtensionHelper.frmEmulatorSelector;

namespace EmulatorExtensionHelper
{
    public partial class frmEmulatorSelector : Form
    {
        private static LanguageManager lang = new LanguageManager();

        private readonly List<string> emulators;
        private readonly string fileName;
        private bool isDialogOpen;

        public enum ActionType
        {
            ExecuteEmulator,
            AssociateFileName,
            AssociateExtension,
            DisassociateFileName,
            DisassociateExtension
        }

        public frmEmulatorSelector(string fileName, ActionType actionType)
        {
            InitializeComponent();

            switch (actionType)
            {
                case ActionType.ExecuteEmulator:
                    this.emulators = ConfigManager.FileExistsInConfig(fileName) ? ConfigManager.ListAssociatedEmulators(fileName, false, false) : ConfigManager.ListAssociatedEmulators(fileName, true, false);
                    break;
                case ActionType.AssociateFileName:
                    this.emulators = ConfigManager.ListUnassociatedEmulatorsForFile(fileName);
                    break;
                case ActionType.AssociateExtension:
                    this.emulators = ConfigManager.ListAssociatedEmulators(fileName, true, true);
                    break;
                case ActionType.DisassociateFileName:
                    this.emulators = ConfigManager.ListAssociatedEmulators(fileName, false, false);
                    break;
                case ActionType.DisassociateExtension:
                    this.emulators = ConfigManager.ListAssociatedEmulators(fileName, true, false);
                    break;
                default: 
                    this.emulators = new List<string>();
                    break;
            }

            this.fileName = fileName;

            // Configurações para comportamento de menu suspenso
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            //this.Deactivate += (s, e) => this.Close(); // Fecha ao perder o foco

            CriarMenu(actionType);
        }

        private void CriarMenu(ActionType actionType)
        {
            int alturaItem = 30;
            int largura = 250;
            int heightSelectNew = 0;

            this.Location = Cursor.Position;

            if (actionType == ActionType.AssociateFileName || actionType == ActionType.AssociateExtension)
            {
                var item = new Label
                {
                    Text = lang.T("EmulatorSelector.NewEmulator"),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10),
                    Size = new Size(largura, alturaItem),
                    Location = new Point(0, 0),
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };

                item.MouseEnter += (s, e) => item.BackColor = Color.LightGray;
                item.MouseLeave += (s, e) => item.BackColor = Color.White;

                item.Click += (s, e) => OpenFileDialog(fileName, actionType);

                this.Controls.Add(item);

                heightSelectNew = alturaItem;
            }

            this.Size = new Size(largura, (alturaItem * emulators.Count) + heightSelectNew);

            for (int i = 0; i < emulators.Count; i++)
            {
                var item = new Label
                {
                    Text = emulators[i],
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10),
                    Size = new Size(largura, alturaItem),
                    Location = new Point(0, (i * alturaItem) + heightSelectNew),
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };

                item.MouseEnter += (s, e) => item.BackColor = Color.LightGray;
                item.MouseLeave += (s, e) => item.BackColor = Color.White;

                switch (actionType)
                {
                    case ActionType.ExecuteEmulator:
                        item.Click += (s, e) => ExecuteEmulator(item.Text); break;
                    case ActionType.AssociateFileName:
                        item.Click += (s, e) => AssociateFileWithEmulator(this.fileName, item.Text, string.Empty); break;
                    case ActionType.AssociateExtension:
                        item.Click += (s, e) => AssociateExtensionWithEmulator(this.fileName, item.Text, string.Empty); break;
                    case ActionType.DisassociateFileName:
                        item.Click += (s, e) => DisassociateFileWithEmulator(this.fileName, item.Text); break;
                    case ActionType.DisassociateExtension:
                        item.Click += (s, e) => DisassociateExtensionWithEmulator(this.fileName, item.Text); break;
                }

                this.Controls.Add(item);
            }
        }

        private void ExecuteEmulator(string nomeEmulador)
        {
            var config = ConfigManager.GetConfig();
            if (config.Emulators != null && config.Emulators.TryGetValue(nomeEmulador, out EmulatorConfig emuladorInfo))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = emuladorInfo.Path,
                        Arguments = $"\"{fileName}\"",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{lang.T("EmulatorSelector.LaunchError")}\n{ex.Message}", lang.T("Common.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.Close();
        }

        private void OpenFileDialog(string fileName, ActionType actionType)
        {
            this.isDialogOpen = true; // Indica que o diálogo está aberto

            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = lang.T("EmulatorSelector.SelectExecutableTitle");

                openFileDialog.Filter = lang.T("EmulatorSelector.ExecutableFilter");
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string exePath = openFileDialog.FileName;
                    string baseName = ExecutableInfoHelper.GetFriendlyNameFromExecutable(exePath);
                    string emulatorName = baseName;
                    var config = ConfigManager.GetConfig();
                    // Verifica se já existe e gera nome alternativo com índice
                    int index = 2;

                    if (!config.Emulators.Values.Any(em => string.Equals(em.Path, exePath, StringComparison.OrdinalIgnoreCase)))
                    {
                        while (config.Emulators.ContainsKey(emulatorName))
                        {
                            emulatorName = $"{baseName} ({index})";
                            index++;
                        }
                    }

                    if (actionType == ActionType.AssociateFileName)
                    {
                        //ConfigManager.AddOrUpdateEmulator(emulatorName, exePath, Path.GetExtension(fileName));
                        ConfigManager.AssociateFileWithEmulator(fileName, emulatorName, exePath);
                    }
                    else if (actionType == ActionType.AssociateExtension)
                    {
                        //ConfigManager.AddOrUpdateEmulator(emulatorName, exePath, Path.GetExtension(fileName));
                        ConfigManager.AssociateExtensionWithEmulator(fileName, emulatorName, exePath);
                    }
                }
            }

            this.Close();
        }

        private void AssociateFileWithEmulator(string fileName, string emulatorName, string exePath)
        {
            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto
            ConfigManager.AssociateFileWithEmulator(fileName, emulatorName, exePath);
            this.Close();
        }

        private void AssociateExtensionWithEmulator(string fileName, string emulatorName, string exePath)
        {
            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto
            ConfigManager.AssociateExtensionWithEmulator(fileName, emulatorName, exePath);
            this.Close();
        }

        private void DisassociateFileWithEmulator(string fileName, string emulatorName)
        {
            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto
            ConfigManager.DisassociateFileWithEmulator(fileName, emulatorName);
            this.Close();
        }

        private void DisassociateExtensionWithEmulator(string fileName, string emulatorName)
        {
            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto
            ConfigManager.DisassociateExtensionWithEmulator(fileName, emulatorName);
            this.Close();
        }

        private void frmEmulatorSelector_Deactivate(object sender, EventArgs e)
        {
            if (!this.isDialogOpen)
                this.Close(); // Fecha o formulário ao perder o foco
        }
    }
}