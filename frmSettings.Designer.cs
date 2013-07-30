namespace QXplorer
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.chkEnableQuery = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownMaxMessage = new System.Windows.Forms.NumericUpDown();
            this.rdoPeekMessage = new System.Windows.Forms.RadioButton();
            this.rdoGetAllMessages = new System.Windows.Forms.RadioButton();
            this.chkSortQueue = new System.Windows.Forms.CheckBox();
            this.chkViewByTable = new System.Windows.Forms.CheckBox();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rdoAscending = new System.Windows.Forms.RadioButton();
            this.rdoDescending = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(154, 176);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(254, 176);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // chkEnableQuery
            // 
            this.chkEnableQuery.AutoSize = true;
            this.chkEnableQuery.Location = new System.Drawing.Point(14, 4);
            this.chkEnableQuery.Name = "chkEnableQuery";
            this.chkEnableQuery.Size = new System.Drawing.Size(90, 17);
            this.chkEnableQuery.TabIndex = 4;
            this.chkEnableQuery.Text = "Enable Query";
            this.chkEnableQuery.UseVisualStyleBackColor = true;
            this.chkEnableQuery.CheckedChanged += new System.EventHandler(this.chkEnableQuery_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownMaxMessage);
            this.groupBox1.Controls.Add(this.rdoPeekMessage);
            this.groupBox1.Controls.Add(this.rdoGetAllMessages);
            this.groupBox1.Location = new System.Drawing.Point(12, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 86);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Query Message by";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Max. Number of Message to  query";
            // 
            // numericUpDownMaxMessage
            // 
            this.numericUpDownMaxMessage.Location = new System.Drawing.Point(13, 62);
            this.numericUpDownMaxMessage.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownMaxMessage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxMessage.Name = "numericUpDownMaxMessage";
            this.numericUpDownMaxMessage.Size = new System.Drawing.Size(96, 20);
            this.numericUpDownMaxMessage.TabIndex = 2;
            this.numericUpDownMaxMessage.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // rdoPeekMessage
            // 
            this.rdoPeekMessage.AutoSize = true;
            this.rdoPeekMessage.Checked = true;
            this.rdoPeekMessage.Location = new System.Drawing.Point(13, 19);
            this.rdoPeekMessage.Name = "rdoPeekMessage";
            this.rdoPeekMessage.Size = new System.Drawing.Size(96, 17);
            this.rdoPeekMessage.TabIndex = 1;
            this.rdoPeekMessage.TabStop = true;
            this.rdoPeekMessage.Text = "Peek Message";
            this.rdoPeekMessage.UseVisualStyleBackColor = true;
            this.rdoPeekMessage.CheckedChanged += new System.EventHandler(this.rdoPeekMessage_CheckedChanged);
            // 
            // rdoGetAllMessages
            // 
            this.rdoGetAllMessages.AutoSize = true;
            this.rdoGetAllMessages.Location = new System.Drawing.Point(13, 42);
            this.rdoGetAllMessages.Name = "rdoGetAllMessages";
            this.rdoGetAllMessages.Size = new System.Drawing.Size(107, 17);
            this.rdoGetAllMessages.TabIndex = 0;
            this.rdoGetAllMessages.Text = "Get All Messages";
            this.rdoGetAllMessages.UseVisualStyleBackColor = true;
            this.rdoGetAllMessages.CheckedChanged += new System.EventHandler(this.rdoGetAllMessages_CheckedChanged);
            // 
            // chkSortQueue
            // 
            this.chkSortQueue.AutoSize = true;
            this.chkSortQueue.Location = new System.Drawing.Point(14, 116);
            this.chkSortQueue.Name = "chkSortQueue";
            this.chkSortQueue.Size = new System.Drawing.Size(80, 17);
            this.chkSortQueue.TabIndex = 6;
            this.chkSortQueue.Text = "Sort Queue";
            this.chkSortQueue.UseVisualStyleBackColor = true;
            this.chkSortQueue.CheckedChanged += new System.EventHandler(this.chkSortQueue_CheckedChanged);
            // 
            // chkViewByTable
            // 
            this.chkViewByTable.AutoSize = true;
            this.chkViewByTable.Checked = true;
            this.chkViewByTable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkViewByTable.Location = new System.Drawing.Point(14, 138);
            this.chkViewByTable.Name = "chkViewByTable";
            this.chkViewByTable.Size = new System.Drawing.Size(129, 17);
            this.chkViewByTable.TabIndex = 11;
            this.chkViewByTable.Text = "Enable View by Table";
            this.chkViewByTable.UseVisualStyleBackColor = true;
            this.chkViewByTable.CheckedChanged += new System.EventHandler(this.chkViewByTable_CheckedChanged);
            // 
            // chkAutoRefresh
            // 
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.Location = new System.Drawing.Point(14, 161);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(123, 17);
            this.chkAutoRefresh.TabIndex = 12;
            this.chkAutoRefresh.Text = "Auto Refresh Queue";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            this.chkAutoRefresh.CheckedChanged += new System.EventHandler(this.chkAutoRefresh_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(215, 335);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rdoAscending
            // 
            this.rdoAscending.AutoSize = true;
            this.rdoAscending.Checked = true;
            this.rdoAscending.Location = new System.Drawing.Point(100, 115);
            this.rdoAscending.Name = "rdoAscending";
            this.rdoAscending.Size = new System.Drawing.Size(75, 17);
            this.rdoAscending.TabIndex = 14;
            this.rdoAscending.TabStop = true;
            this.rdoAscending.Text = "Ascending";
            this.rdoAscending.UseVisualStyleBackColor = true;
            this.rdoAscending.CheckedChanged += new System.EventHandler(this.rdoAscending_CheckedChanged);
            // 
            // rdoDescending
            // 
            this.rdoDescending.AutoSize = true;
            this.rdoDescending.Location = new System.Drawing.Point(181, 116);
            this.rdoDescending.Name = "rdoDescending";
            this.rdoDescending.Size = new System.Drawing.Size(82, 17);
            this.rdoDescending.TabIndex = 15;
            this.rdoDescending.Text = "Descending";
            this.rdoDescending.UseVisualStyleBackColor = true;
            this.rdoDescending.CheckedChanged += new System.EventHandler(this.rdoDescending_CheckedChanged);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(341, 206);
            this.Controls.Add(this.rdoDescending);
            this.Controls.Add(this.rdoAscending);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkAutoRefresh);
            this.Controls.Add(this.chkViewByTable);
            this.Controls.Add(this.chkSortQueue);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkEnableQuery);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MSMQ QXplorer Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.CheckBox chkEnableQuery;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoGetAllMessages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxMessage;
        private System.Windows.Forms.RadioButton rdoPeekMessage;
        private System.Windows.Forms.CheckBox chkSortQueue;
        private System.Windows.Forms.CheckBox chkViewByTable;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rdoAscending;
        private System.Windows.Forms.RadioButton rdoDescending;
    }
}