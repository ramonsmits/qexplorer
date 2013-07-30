namespace QXplorer
{
    partial class frmCreateQueue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateQueue));
            this.label1 = new System.Windows.Forms.Label();
            this.txtNewQueue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkTransactional = new System.Windows.Forms.CheckBox();
            this.btnNewQueue = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 17);
            this.label1.TabIndex = 0;
            // 
            // txtNewQueue
            // 
            this.txtNewQueue.Location = new System.Drawing.Point(100, 42);
            this.txtNewQueue.Name = "txtNewQueue";
            this.txtNewQueue.Size = new System.Drawing.Size(180, 20);
            this.txtNewQueue.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Queue Name:";
            // 
            // chkTransactional
            // 
            this.chkTransactional.AutoSize = true;
            this.chkTransactional.Location = new System.Drawing.Point(24, 82);
            this.chkTransactional.Name = "chkTransactional";
            this.chkTransactional.Size = new System.Drawing.Size(90, 17);
            this.chkTransactional.TabIndex = 3;
            this.chkTransactional.Text = "Transactional";
            this.chkTransactional.UseVisualStyleBackColor = true;
            // 
            // btnNewQueue
            // 
            this.btnNewQueue.Location = new System.Drawing.Point(154, 117);
            this.btnNewQueue.Name = "btnNewQueue";
            this.btnNewQueue.Size = new System.Drawing.Size(75, 23);
            this.btnNewQueue.TabIndex = 4;
            this.btnNewQueue.Text = "OK";
            this.btnNewQueue.UseVisualStyleBackColor = true;
            this.btnNewQueue.Click += new System.EventHandler(this.btnNewQueue_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(249, 117);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // frmCreateQueue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(342, 152);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnNewQueue);
            this.Controls.Add(this.chkTransactional);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNewQueue);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCreateQueue";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Queue";
            this.Load += new System.EventHandler(this.frmCreateQueue_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNewQueue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkTransactional;
        private System.Windows.Forms.Button btnNewQueue;
        private System.Windows.Forms.Button button2;
    }
}