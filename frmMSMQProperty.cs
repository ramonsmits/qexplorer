using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QXplorer
{
    public partial class frmMSMQProperty : Form
    {
        public frmMSMQProperty()
        {
            InitializeComponent();
        }

        private void chkLimitJournal_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLimitJournal.Checked)
                txtlimitjournal.Enabled = true;
            else
                txtlimitjournal.Enabled = false;
        }

        private void txtLimitmessage_TextChanged(object sender, EventArgs e)
        {
            if (chkLimitmessage.Checked)
                txtLimitmessage.Enabled = true;
            else
                txtLimitmessage.Enabled = false;
        }

        private void frmMSMQProperty_Load(object sender, EventArgs e)
        {
            
        }
    }
}