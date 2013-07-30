namespace QXplorer
{
    partial class frmMSMQProperty
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.txttypeID = new System.Windows.Forms.TextBox();
            this.txtLimitmessage = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtlimitjournal = new System.Windows.Forms.TextBox();
            this.chkLimitJournal = new System.Windows.Forms.CheckBox();
            this.chkEnableJournal = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labeltransaction = new System.Windows.Forms.Label();
            this.cnhAuthenticated = new System.Windows.Forms.CheckBox();
            this.chkLimitmessage = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelPathName = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(8, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(383, 382);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Menu;
            this.tabPage1.Controls.Add(this.txtLabel);
            this.tabPage1.Controls.Add(this.txttypeID);
            this.tabPage1.Controls.Add(this.txtLimitmessage);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.labeltransaction);
            this.tabPage1.Controls.Add(this.cnhAuthenticated);
            this.tabPage1.Controls.Add(this.chkLimitmessage);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.labelPathName);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(375, 356);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            // 
            // txtLabel
            // 
            this.txtLabel.Location = new System.Drawing.Point(92, 54);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(256, 20);
            this.txtLabel.TabIndex = 11;
            // 
            // txttypeID
            // 
            this.txttypeID.Location = new System.Drawing.Point(92, 82);
            this.txttypeID.Name = "txttypeID";
            this.txttypeID.Size = new System.Drawing.Size(256, 20);
            this.txttypeID.TabIndex = 10;
            // 
            // txtLimitmessage
            // 
            this.txtLimitmessage.Location = new System.Drawing.Point(257, 125);
            this.txtLimitmessage.Name = "txtLimitmessage";
            this.txtLimitmessage.Size = new System.Drawing.Size(91, 20);
            this.txtLimitmessage.TabIndex = 9;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "None",
            "Optional",
            "Body"});
            this.comboBox1.Location = new System.Drawing.Point(92, 234);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 8;
            this.comboBox1.Text = "Optional";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtlimitjournal);
            this.groupBox1.Controls.Add(this.chkLimitJournal);
            this.groupBox1.Controls.Add(this.chkEnableJournal);
            this.groupBox1.Location = new System.Drawing.Point(16, 267);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 71);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Journal";
            // 
            // txtlimitjournal
            // 
            this.txtlimitjournal.Location = new System.Drawing.Point(241, 41);
            this.txtlimitjournal.Name = "txtlimitjournal";
            this.txtlimitjournal.Size = new System.Drawing.Size(91, 20);
            this.txtlimitjournal.TabIndex = 10;
            // 
            // chkLimitJournal
            // 
            this.chkLimitJournal.AutoSize = true;
            this.chkLimitJournal.Location = new System.Drawing.Point(24, 44);
            this.chkLimitJournal.Name = "chkLimitJournal";
            this.chkLimitJournal.Size = new System.Drawing.Size(154, 17);
            this.chkLimitJournal.TabIndex = 1;
            this.chkLimitJournal.Text = "Limit journal storage to (KB)";
            this.chkLimitJournal.UseVisualStyleBackColor = true;
            // 
            // chkEnableJournal
            // 
            this.chkEnableJournal.AutoSize = true;
            this.chkEnableJournal.Location = new System.Drawing.Point(24, 19);
            this.chkEnableJournal.Name = "chkEnableJournal";
            this.chkEnableJournal.Size = new System.Drawing.Size(59, 17);
            this.chkEnableJournal.TabIndex = 0;
            this.chkEnableJournal.Text = "Enable";
            this.chkEnableJournal.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Privacy level :";
            // 
            // labeltransaction
            // 
            this.labeltransaction.Location = new System.Drawing.Point(89, 170);
            this.labeltransaction.Name = "labeltransaction";
            this.labeltransaction.Size = new System.Drawing.Size(239, 22);
            this.labeltransaction.TabIndex = 5;
            // 
            // cnhAuthenticated
            // 
            this.cnhAuthenticated.AutoSize = true;
            this.cnhAuthenticated.Location = new System.Drawing.Point(92, 150);
            this.cnhAuthenticated.Name = "cnhAuthenticated";
            this.cnhAuthenticated.Size = new System.Drawing.Size(92, 17);
            this.cnhAuthenticated.TabIndex = 4;
            this.cnhAuthenticated.Text = "Authenticated";
            this.cnhAuthenticated.UseVisualStyleBackColor = true;
            // 
            // chkLimitmessage
            // 
            this.chkLimitmessage.AutoSize = true;
            this.chkLimitmessage.Location = new System.Drawing.Point(92, 127);
            this.chkLimitmessage.Name = "chkLimitmessage";
            this.chkLimitmessage.Size = new System.Drawing.Size(165, 17);
            this.chkLimitmessage.TabIndex = 3;
            this.chkLimitmessage.Text = "Limit message storage to (KB)";
            this.chkLimitmessage.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Label :";
            // 
            // labelPathName
            // 
            this.labelPathName.Location = new System.Drawing.Point(54, 19);
            this.labelPathName.Name = "labelPathName";
            this.labelPathName.Size = new System.Drawing.Size(315, 22);
            this.labelPathName.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Menu;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(375, 356);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Security";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(154, 390);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(235, 390);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(316, 390);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // frmMSMQProperty
            // 
            this.ClientSize = new System.Drawing.Size(290, 273);
            this.Name = "frmMSMQProperty";
            this.Load += new System.EventHandler(this.frmMSMQProperty_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label labelPathName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labeltransaction;
        private System.Windows.Forms.CheckBox cnhAuthenticated;
        private System.Windows.Forms.CheckBox chkLimitmessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.TextBox txttypeID;
        private System.Windows.Forms.TextBox txtLimitmessage;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtlimitjournal;
        private System.Windows.Forms.CheckBox chkLimitJournal;
        private System.Windows.Forms.CheckBox chkEnableJournal;
    }
}