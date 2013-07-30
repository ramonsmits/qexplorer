using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QXplorer
{
    public partial class frmConnectComputer : Form
    {
       
        GlobalVariables oVar = new GlobalVariables();
        public frmConnectComputer(ref GlobalVariables objArg)
        {
            oVar = objArg;
            InitializeComponent();
        }

        private void frmConnectComputer_Load(object sender, EventArgs e)
        {

            if (oVar.oComputerList_ARRAY.Count >= 1)
            {
                comboComputerList.Text =(string) oVar.oComputerList_ARRAY[0];
                foreach (string sComputer in oVar.oComputerList_ARRAY)
                {
                    comboComputerList.Items.Add(sComputer);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            oVar.sComputerName = comboComputerList.Text.ToString().Trim();
            if(!(string.IsNullOrEmpty(oVar.sComputerName)))
                oVar.bCallRefresh = true;
            this.Close();
            
            
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}