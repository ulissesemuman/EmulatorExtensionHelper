using System;
using System.CodeDom;
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

        private readonly List<EmulatorInfo> emulators;
        private readonly string fileName;
        private readonly ExecutionMode executionMode;
        private bool isDialogOpen;

        public enum ExecutionMode
        {
            ExecuteEmulator,
            AssociateFileName,
            AssociateExtension,
            DisassociateFileName,
            DisassociateExtension,
            Unknown
        }

        public frmEmulatorSelector(string fileName, ExecutionMode actionType)
        {
            InitializeComponent();

            switch (actionType)
            {
                case ExecutionMode.ExecuteEmulator:
                    this.emulators = ConfigManager.FileExistsInConfig(fileName) ? ConfigManager.GetEmulatorsByRom(fileName) : ConfigManager.GetEmulatorsByExtension(fileName);
                    break;
                case ExecutionMode.AssociateFileName:
                    this.emulators = ConfigManager.GetEmulatorsWithoutRom(fileName);
                    break;
                case ExecutionMode.AssociateExtension:
                    this.emulators = ConfigManager.GetEmulatorsWithoutExtension(fileName);
                    break;
                case ExecutionMode.DisassociateFileName:
                    this.emulators = ConfigManager.GetEmulatorsByRom(fileName);
                    break;
                case ExecutionMode.DisassociateExtension:
                    this.emulators = ConfigManager.GetEmulatorsByExtension(fileName);
                    break;
                default:
                    this.emulators = new List<EmulatorInfo>();
                    break;
            }

            this.fileName = fileName;
            this.executionMode = actionType;

            // Faz o form ficar completamente invisível
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Opacity = 0;
            this.Width = 0;
            this.Height = 0;
            this.BringToFront();
            this.Activate(); // força a ativação do form
        }

        private void CreateMenu(ExecutionMode actionType)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            contextMenu.Visible = true;

            this.Location = Cursor.Position;

            if (actionType == ExecutionMode.AssociateFileName || actionType == ExecutionMode.AssociateExtension)
            {
                var item = new ToolStripMenuItem(lang.T("EmulatorSelector.NewEmulator"));
                item.Click += (s, e) => OpenFileDialog(fileName, actionType);
                contextMenu.Items.Add(item);
            }

            foreach (var emulator in emulators)
            {
                var item = new ToolStripMenuItem(emulator.Name);

                // Se for RetroArch, criar submenu com cores
                if (emulator.Name.Contains("RetroArch", StringComparison.OrdinalIgnoreCase) && actionType == ExecutionMode.ExecuteEmulator)
                {
                    SetClickEventForRetroArchMenuItem(item, actionType);
                }
                else
                {
                    SetClickEventForMenuItem(item, actionType);
                }

                if (!string.IsNullOrEmpty(emulator.Config.Icon) && File.Exists(emulator.Config.Icon))
                {
                    using var fs = new FileStream(emulator.Config.Icon, FileMode.Open, FileAccess.Read);
                    using var ms = new MemoryStream();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    item.Image = Image.FromStream(ms);
                }

                contextMenu.Items.Add(item);
            }

            contextMenu.Show(Cursor.Position);
        }

        private void SetClickEventForMenuItem(ToolStripMenuItem item, ExecutionMode actionType)
        {
            item.Click += (s, e) =>
            {
                switch (actionType)
                {
                    case ExecutionMode.ExecuteEmulator:
                        ExecuteEmulator(item.Text); break;
                    case ExecutionMode.AssociateFileName:
                        AssociateFileWithEmulator(this.fileName, item.Text); break;
                    case ExecutionMode.AssociateExtension:
                        AssociateExtensionWithEmulator(this.fileName, item.Text); break;
                    case ExecutionMode.DisassociateFileName:
                        DisassociateFileWithEmulator(this.fileName, item.Text); break;
                    case ExecutionMode.DisassociateExtension:
                        DisassociateExtensionWithEmulator(this.fileName, item.Text); break;
                }
            };
        }

        private void SetClickEventForRetroArchMenuItem(ToolStripMenuItem item, ExecutionMode actionType)
        {
            string? retroarchPath = ConfigManager.GetEmulatorPathByName(item.Text);

            if (!string.IsNullOrEmpty(retroarchPath))
            {
                var ext = Path.GetExtension(fileName)?.TrimStart('.').ToLowerInvariant();
                var cores = RetroArchHelper.GetRetroArchCoresForExtension(item.Text, ext ?? "");

                if (actionType == ExecutionMode.ExecuteEmulator)
                {
                    foreach (var core in cores)
                    {
                        var coreItem = new ToolStripMenuItem(core.DisplayName);
                        coreItem.Tag = core.Name;

                        // TODO: Chamar SetClickEventForMenuItem quando cada corde for traatado como um emulador standalone
                        coreItem.Click += (s, e) =>
                        {
                            ExecuteEmulator(item.Text, core.Path);
                        };

                        item.DropDownItems.Add(coreItem);
                    }
                }

                //if (actionType != ExecutionMode.ExecuteEmulator)
                //{
                //    // Adiciona um item que lista todos os núcleos disponíveis para a extesnão atual
                //    var allCoresItem = new ToolStripMenuItem(lang.T("EmulatorSelector.AllCores"));

                //    cores = RetroArchHelper.GetMissingRetroArchCores(item.Text, ext ?? "");

                //    foreach (var core in cores)
                //    {
                //        var coreItem = new ToolStripMenuItem(core.DisplayName);
                //        coreItem.Tag = core.Name;

                //        // TODO: Chamar SetClickEventForMenuItem quando cada corde for traatado como um emulador standalone
                //        coreItem.Click += (s, e) =>
                //        {
                //            InstallCore(item.Text, core.Name);
                //        };

                //        allCoresItem.DropDownItems.Add(coreItem);
                //    }

                //    item.DropDownItems.Add(allCoresItem);
                //}
            }
        }

        private void CreateMenuLabel(ExecutionMode actionType)
        {
            int alturaItem = 30;
            int largura = 250;
            int heightSelectNew = 0;

            this.Location = Cursor.Position;

            if (actionType == ExecutionMode.AssociateFileName || actionType == ExecutionMode.AssociateExtension)
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
                    Text = emulators[i].ToString(),
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
                    case ExecutionMode.ExecuteEmulator:
                        item.Click += (s, e) => ExecuteEmulator(item.Text); break;
                    case ExecutionMode.AssociateFileName:
                        item.Click += (s, e) => AssociateFileWithEmulator(this.fileName, item.Text); break;
                    case ExecutionMode.AssociateExtension:
                        item.Click += (s, e) => AssociateExtensionWithEmulator(this.fileName, item.Text); break;
                    case ExecutionMode.DisassociateFileName:
                        item.Click += (s, e) => DisassociateFileWithEmulator(this.fileName, item.Text); break;
                    case ExecutionMode.DisassociateExtension:
                        item.Click += (s, e) => DisassociateExtensionWithEmulator(this.fileName, item.Text); break;
                }

                this.Controls.Add(item);
            }
        }

        private void ExecuteEmulator(string nomeEmulador, string corePath = "")
        {
            var config = ConfigManager.GetConfig();
            if (config.Emulators != null && config.Emulators.TryGetValue(nomeEmulador, out EmulatorConfig emuladorInfo))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = emuladorInfo.Path,
                        Arguments = (corePath != string.Empty ? $"-L \"{corePath}\"" : string.Empty) + $" \"{fileName}\"",
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

        private void OpenFileDialog(string fileName, ExecutionMode actionType)
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
                    var icon = IconUtils.ExtractIcon(exePath, 0, false);

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

                    string iconFileName = IconUtils.SaveEmulatorIcon(icon, emulatorName);

                    if (actionType == ExecutionMode.AssociateFileName)
                    {
                        //ConfigManager.AddOrUpdateEmulator(emulatorName, exePath, Path.GetExtension(fileName));
                        ConfigManager.AssociateFileWithEmulator(fileName, emulatorName, exePath, iconFileName);
                    }
                    else if (actionType == ExecutionMode.AssociateExtension)
                    {
                        //ConfigManager.AddOrUpdateEmulator(emulatorName, exePath, Path.GetExtension(fileName));
                        ConfigManager.AssociateExtensionWithEmulator(fileName, emulatorName, exePath, iconFileName);
                    }
                }
            }

            this.Close();
        }

        private void AssociateFileWithEmulator(string fileName, string emulatorName)
        {
            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto

            string exePath = ConfigManager.GetEmulatorPathByName(emulatorName) ?? string.Empty;

            var icon = IconUtils.ExtractIcon(exePath, 0, false);

            string iconFileName = IconUtils.SaveEmulatorIcon(icon, emulatorName);

            ConfigManager.AssociateFileWithEmulator(fileName, emulatorName, exePath, iconFileName);

            this.Close();
        }

        private void AssociateExtensionWithEmulator(string fileName, string emulatorName)
        {
            this.Visible = false; // Esconde o formulário para evitar que ele fique visível enquanto o diálogo está aberto

            string exePath = ConfigManager.GetEmulatorPathByName(emulatorName) ?? string.Empty;

            var icon = IconUtils.ExtractIcon(exePath, 0, false);

            string iconFileName = IconUtils.SaveEmulatorIcon(icon, emulatorName);

            ConfigManager.AssociateExtensionWithEmulator(fileName, emulatorName, exePath, iconFileName);

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

        private void frmEmulatorSelector_Shown(object sender, EventArgs e)
        {
            CreateMenu(this.executionMode);
        }

        private void InstallCore(string retroArchName, string coreName)
        {
            string? retroarchPath = ConfigManager.GetEmulatorPathByName(retroArchName);
            if (!string.IsNullOrEmpty(retroarchPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = retroarchPath,
                        //Arguments = $"--install-core \"{coreName}\"",
                        Arguments = $"--download_core \"{coreName}\"",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{lang.T("EmulatorSelector.InstallCoreError")}\n{ex.Message}", lang.T("Common.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.Close();
        }
    }
}