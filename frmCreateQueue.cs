using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
//Author : JuLius Lucero Ramos, ECE, QA
//Email  : juliusLramos@hotmail.com
//
namespace QXplorer
{
    public partial class frmCreateQueue : Form
    {
        GlobalVariables oVar = new GlobalVariables();
        MSMQXplorer oMSMQ = new MSMQXplorer();       
        public frmCreateQueue(ref GlobalVariables objArg)
        {
            oVar = objArg;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNewQueue_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNewQueue.Text.ToString().Trim()))
            {
                MessageBox.Show("Information is missing");
                txtNewQueue.BorderStyle = BorderStyle.FixedSingle;
                return;
            }
            txtNewQueue.BorderStyle = BorderStyle.Fixed3D;
            oVar.sStatusMessage = "Create Queue " + txtNewQueue.Text.ToString().Trim();
            oMSMQ.CreateQueue(ref oVar, "." + oVar.sQueueType, txtNewQueue.Text.ToString().Trim(), chkTransactional.Checked);
            oVar.sStatusMessage = "Creating Queue "+txtNewQueue.Text.ToString()+"... Done , Please click refresh ";
        }

        private void frmCreateQueue_Load(object sender, EventArgs e)
        {
    
            if (oVar.IsRemote)
            {
                MessageBox.Show("Create Queue in remote computer not supported \n",
                           "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            oVar.GetQueueFormatType(oVar.sComputerName);
            if (!(oVar.IsHostname))
            {
                MessageBox.Show("IP Address in not supported, Please use hostname to create queue",
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            if (string.IsNullOrEmpty(oVar.sQueueType))
                label1.Text = "public$\\";
            else
                label1.Text = "private$\\";
        }
    }
}