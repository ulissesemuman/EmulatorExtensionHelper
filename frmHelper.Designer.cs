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
            cmdRemoveContextMenu = new Button();
            cmdExit = new Button();
            groupBox1 = new GroupBox();
            cmdCurrentUser = new Button();
            cmdAllUsers = new Button();
            cmdRemoveAllAssociations = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // cmdRemoveContextMenu
            // 
            cmdRemoveContextMenu.Location = new Point(12, 82);
            cmdRemoveContextMenu.Name = "cmdRemoveContextMenu";
            cmdRemoveContextMenu.Size = new Size(348, 23);
            cmdRemoveContextMenu.TabIndex = 1;
            cmdRemoveContextMenu.Text = "Remover opções do menu de contexto do Windows Explorer";
            cmdRemoveContextMenu.UseVisualStyleBackColor = true;
            cmdRemoveContextMenu.Click += cmdRemoveContextMenu_Click;
            // 
            // cmdExit
            // 
            cmdExit.Location = new Point(146, 162);
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
            groupBox1.Location = new Point(12, 18);
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
            cmdRemoveAllAssociations.Location = new Point(12, 121);
            cmdRemoveAllAssociations.Name = "cmdRemoveAllAssociations";
            cmdRemoveAllAssociations.Size = new Size(348, 23);
            cmdRemoveAllAssociations.TabIndex = 6;
            cmdRemoveAllAssociations.Text = "Remover todas as associações do Registriodo Sistema";
            cmdRemoveAllAssociations.UseVisualStyleBackColor = true;
            cmdRemoveAllAssociations.Click += cmdRemoveAllAssociations_Click;
            // 
            // frmHelper
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(373, 197);
            Controls.Add(cmdRemoveAllAssociations);
            Controls.Add(groupBox1);
            Controls.Add(cmdExit);
            Controls.Add(cmdRemoveContextMenu);
            Name = "frmHelper";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Emulator Extension Helper";
            Load += frmHelper_Load;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button cmdRemoveContextMenu;
        private Button cmdExit;
        private GroupBox groupBox1;
        private Button cmdCurrentUser;
        private Button cmdAllUsers;
        private Button cmdRemoveAllAssociations;
    }
}
