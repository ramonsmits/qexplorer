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
    public partial class frmSettings : Form
    {
        GlobalVariables oVar = new GlobalVariables();
        public frmSettings(ref GlobalVariables objArg)
        {
            oVar = objArg;
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
          
            decimal dmax;
            Decimal.TryParse(oVar.iMaxMessageToQuery.ToString(), out dmax);
            numericUpDownMaxMessage.Value = dmax;
            chkEnableQuery.Checked = oVar.bEnableQuery;
            if (oVar.bSortQueue)
            {
                chkSortQueue.Checked = true;
                rdoAscending.Enabled = true;
                rdoDescending.Enabled = true;
            }
            else
            {
                chkSortQueue.Checked = false;
                rdoAscending.Enabled = false;
                rdoDescending.Enabled = false;
            }


        
            
            if (oVar.bQueryMessage)
            {
                rdoPeekMessage.Checked = true;
                rdoGetAllMessages.Checked = false;
            }
            else
            {
                rdoPeekMessage.Checked = false;
                rdoGetAllMessages.Checked = true;
            }

           
            if(oVar.bEnableQuery)
            {
                numericUpDownMaxMessage.Enabled = true;
                groupBox1.Enabled = true;
            }
            else
            {
                numericUpDownMaxMessage.Enabled = false;
                groupBox1.Enabled = false;
            }
            if (oVar.bViewByTable)
                chkViewByTable.Checked = true;
            else
                chkViewByTable.Checked = false;
            if (oVar.bAutoRefreshQueue)
                chkAutoRefresh.Checked = true;
            else
                chkAutoRefresh.Checked = false;

            if (oVar.bMessageQueueSorting)
            {
                rdoAscending.Checked = true;
                rdoDescending.Checked = false;
            }
            else
            {
                rdoAscending.Checked = false;
                rdoDescending.Checked = true;
            }
            


        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            int.TryParse(numericUpDownMaxMessage.Value.ToString(), out oVar.iMaxMessageToQuery);
             
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int.TryParse(numericUpDownMaxMessage.Value.ToString(), out oVar.iMaxMessageToQuery);
            oVar.bEnableQuery = chkEnableQuery.Checked;
            Close();
        }

        private void chkEnableQuery_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableQuery.Checked)
            {
                numericUpDownMaxMessage.Enabled = true;
                groupBox1.Enabled = true;
            }
            else
            {
                numericUpDownMaxMessage.Enabled = false;
                groupBox1.Enabled = false;
            }
        }

        private void rdoPeekMessage_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoPeekMessage.Checked)
                oVar.bQueryMessage = true;
        }

        private void rdoGetAllMessages_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoGetAllMessages.Checked)
                oVar.bQueryMessage = false;
        }

        private void chkSortQueue_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSortQueue.Checked)
            {
                oVar.bSortQueue = true;
                rdoAscending.Enabled = true;
                rdoDescending.Enabled = true;
            }
            else
            {
                oVar.bSortQueue = false;
                rdoAscending.Enabled = false;
                rdoDescending.Enabled = false;
            }
            
        }

       private void chkViewByTable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkViewByTable.Checked)
                oVar.bViewByTable = true;
            else
                oVar.bViewByTable = false;
        }

        private void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAutoRefresh.Checked)
                oVar.bAutoRefreshQueue = true;
            else
                oVar.bAutoRefreshQueue = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rdoAscending_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoAscending.Checked)
            {
                oVar.bMessageQueueSorting = true;
                rdoDescending.Checked = false;
            }

        }

        private void rdoDescending_CheckedChanged(object sender, EventArgs e)
        {
            if(rdoDescending.Checked)
            {
                oVar.bMessageQueueSorting = false;
                rdoAscending.Checked = false;
            }
        }

    }
}