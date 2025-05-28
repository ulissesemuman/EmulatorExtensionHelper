namespace EmulatorExtensionHelper
{
    partial class frmHelper
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHelper));
            cmdRemoveContextMenu = new Button();
            cmdExit = new Button();
            groupBox1 = new GroupBox();
            cmdCurrentUser = new Button();
            cmdAllUsers = new Button();
            cmdRemoveAllAssociations = new Button();
            label1 = new Label();
            cmbSelectLanguage = new ComboBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // cmdRemoveContextMenu
            // 
            cmdRemoveContextMenu.Location = new Point(16, 119);
            cmdRemoveContextMenu.Name = "cmdRemoveContextMenu";
            cmdRemoveContextMenu.Size = new Size(348, 23);
            cmdRemoveContextMenu.TabIndex = 1;
            cmdRemoveContextMenu.Text = "Remover opções do menu de contexto do Windows Explorer";
            cmdRemoveContextMenu.UseVisualStyleBackColor = true;
            cmdRemoveContextMenu.Click += cmdRemoveContextMenu_Click;
            // 
            // cmdExit
            // 
            cmdExit.Location = new Point(150, 199);
            cmdExit.Name = "cmdExit";
            cmdExit.Size = new Size(75, 23);
            cmdExit.TabIndex = 2;
            cmdExit.Text = "Sair";
            cmdExit.UseVisualStyleBackColor = true;
            cmdExit.Click += cmdExit_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cmdCurrentUser);
            groupBox1.Controls.Add(cmdAllUsers);
            groupBox1.Location = new Point(16, 55);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(348, 58);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Adicionar opções ao menu de contexto do Windows Explorer";
            // 
            // cmdCurrentUser
            // 
            cmdCurrentUser.Location = new Point(24, 22);
            cmdCurrentUser.Name = "cmdCurrentUser";
            cmdCurrentUser.Size = new Size(139, 23);
            cmdCurrentUser.TabIndex = 4;
            cmdCurrentUser.Text = "Usuário Atual";
            cmdCurrentUser.UseVisualStyleBackColor = true;
            cmdCurrentUser.Click += cmdCurrentUser_Click;
            // 
            // cmdAllUsers
            // 
            cmdAllUsers.Location = new Point(183, 22);
            cmdAllUsers.Name = "cmdAllUsers";
            cmdAllUsers.Size = new Size(139, 23);
            cmdAllUsers.TabIndex = 0;
            cmdAllUsers.Text = "Todos os Usuários";
            cmdAllUsers.UseVisualStyleBackColor = true;
            cmdAllUsers.Click += cmdAllUsers_Click;
            // 
            // cmdRemoveAllAssociations
            // 
            cmdRemoveAllAssociations.Location = new Point(16, 158);
            cmdRemoveAllAssociations.Name = "cmdRemoveAllAssociations";
            cmdRemoveAllAssociations.Size = new Size(348, 23);
            cmdRemoveAllAssociations.TabIndex = 6;
            cmdRemoveAllAssociations.Text = "Remover todas as associações do Registriodo Sistema";
            cmdRemoveAllAssociations.UseVisualStyleBackColor = true;
            cmdRemoveAllAssociations.Click += cmdRemoveAllAssociations_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 19);
            label1.Name = "label1";
            label1.Size = new Size(101, 15);
            label1.TabIndex = 7;
            label1.Text = "Selecionar Idioma";
            // 
            // cmbSelectLanguage
            // 
            cmbSelectLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSelectLanguage.FormattingEnabled = true;
            cmbSelectLanguage.Location = new Point(123, 16);
            cmbSelectLanguage.Name = "cmbSelectLanguage";
            cmbSelectLanguage.Size = new Size(229, 23);
            cmbSelectLanguage.TabIndex = 9;
            cmbSelectLanguage.SelectedIndexChanged += cmbSelectLanguage_SelectedIndexChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // frmHelper
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cmdExit;
            ClientSize = new Size(373, 240);
            Controls.Add(cmbSelectLanguage);
            Controls.Add(label1);
            Controls.Add(cmdRemoveAllAssociations);
            Controls.Add(groupBox1);
            Controls.Add(cmdExit);
            Controls.Add(cmdRemoveContextMenu);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "frmHelper";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Emulator Extension Helper";
            Load += frmHelper_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button cmdRemoveContextMenu;
        private Button cmdExit;
        private GroupBox groupBox1;
        private Button cmdCurrentUser;
        private Button cmdAllUsers;
        private Button cmdRemoveAllAssociations;
        private Label label1;
        private ComboBox cmbSelectLanguage;
        private ContextMenuStrip contextMenuStrip1;
    }
}
