using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
//
//Author : JuLius Lucero Ramos, ECE, QA
//Email  : juliusLramos@hotmail.com
//
namespace QXplorer
{

    public delegate void UpdateComputerDelegate(string UpdateComputer);



    public partial class frmQXplorer : Form
    {
        GlobalVariables oVariables = new GlobalVariables();
        MSMQXplorer oMSMQ = new MSMQXplorer();
        QueueInfos QueueInfo = new QueueInfos();
        Thread ThreadUpdate;
        DGVColumnHeader dgvColumnHeader;

        public frmQXplorer(GlobalVariables oVariable)
        {
            oVariables = oVariable;
            InitializeComponent();
        }

        private void frmQXplorer_Load(object sender, EventArgs e)
        {
            this.Text = "MSMQ QXplorer " + String.Format("v. {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            MainTimer.Start();
            msmqgroupbox.Visible = true;
            filehashgroupbox.Visible = false;
            MyWebGroupBox.Visible = false;

            txtBody.BringToFront();
            txtBodyWeb.SendToBack();
            DgViewMessage.SendToBack();
            txtBody.Visible = true;

            ProgressBarTimer.Start();

            LoadApplicationSettings();
            ComboFilterList.Items.Clear();
            foreach (string sQueueName in oVariables.oFilterList_ARRAY)
            {
                ComboFilterList.Items.Add(sQueueName);
                ComboFilterList.Text = sQueueName;
            }
            if (oVariables.bEnableFilter)
            {
                lblDel.Enabled = true;
                lblAdd.Enabled = true;
                chkEnableFilter.Checked = true;
                ComboFilterList.Enabled = true;
            }
            else
            {
                lblDel.Enabled = false;
                lblAdd.Enabled = false;
                chkEnableFilter.Checked = false;
                ComboFilterList.Enabled = false;
            }
            if (!(string.IsNullOrEmpty(Clipboard.GetText())))
            {
                pasteToolStripMenuItem.Enabled = true;
                ClearClipboardtoolStripMenuItem.Enabled = true;
                PastetoolStripButton.Enabled = true;
            }
            else
            {
                pasteToolStripMenuItem.Enabled = false;
                ClearClipboardtoolStripMenuItem.Enabled = false;
                PastetoolStripButton.Enabled = false;
            }

            // ThreadUpdate = new Thread(new ThreadStart(ThreadUpdateComputer));
           //  ThreadUpdate.Start();
            if (string.IsNullOrEmpty(oVariables.sComputerName))
                ComBoxComputer.Text = Environment.MachineName;
            else
                ComBoxComputer.Text = oVariables.sComputerName;
            comboPriority.Text = "Normal";

            oVariables.bbtnRefreshQueue = true;
            oVariables.brefreshQueueToolStripMenuItem = true;
            oVariables.bRefreshQueuetoolStripButton = true;
            oVariables.bFlagProgressBar = false;
            toolTip1.SetToolTip(this.refreshqueues, "Refresh Queues");


            //dgSearchresult.Columns.Insert(0, new DataGridViewCheckBoxColumn());

            dgvColumnHeader = new DGVColumnHeader();
            DataGridCheckBox.HeaderCell = dgvColumnHeader;
            DataGridCheckBox.HeaderText = "";

        }
        private void frmQXplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApplicationSaveSettings();
        }
        private void refreshqueues_Click(object sender, EventArgs e)
        {
            ThreadUpdate = new Thread(new ThreadStart(ThreadUpdateComputer));
            ThreadUpdate.Start();
        }
        #region MSMQ
        #region Update Computer List
        private void ThreadUpdateComputer()
        {
            try
            {
                oVariables.bbtnRefreshQueue = false;
                oVariables.brefreshQueueToolStripMenuItem = false;
                oVariables.bRefreshQueuetoolStripButton = false;
                oVariables.oComputerList_ARRAY.Clear();
                oVariables.sStatusMessage = "Getting list of computer name  ... Start";
                NetworkBrowser nb = new NetworkBrowser();
                oVariables.oComputerList_ARRAY = nb.getNetworkComputers();
                oVariables.iProgressMax = oVariables.oComputerList_ARRAY.Count;
                oVariables.iProgressValue = 0;
                oVariables.bFlagProgressBar = true;
                oVariables.bAddComputerUpdate = true;
                foreach (string pc in oVariables.oComputerList_ARRAY)
                {
                    oVariables.sStatusMessage = pc.ToString();
                    UpdateComputerDelegate(pc.ToString());
                    if (oVariables.iProgressMax > oVariables.iProgressValue)
                        oVariables.iProgressValue += 1;
                    // oVariables.oComputerList_ARRAY.Add(pc.ToString());
                }
                oVariables.bAddComputerUpdate = false;
                //UpdateComputerDelegate(Environment.MachineName);
                oVariables.sStatusMessage = "Getting list of computer name  ... Done";
                oVariables.oComputerList_ARRAY.Sort();
                oVariables.bbtnRefreshQueue = true;
                oVariables.brefreshQueueToolStripMenuItem = true;
                oVariables.bRefreshQueuetoolStripButton = true;
                oVariables.bFlagProgressBar = false;
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred trying to access the network computers", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                //ThreadUpdate.Abort();
                //Application.Exit();
            }
            oVariables.bRefreshOnly = true;
            ThreadUpdate.Abort();

        }

        private void UpdateComputerDelegate(string UpdateComputer)
        {
            if (ComBoxComputer.InvokeRequired)
            {
                UpdateComputerDelegate UpdateComputerVariable = new UpdateComputerDelegate(UpdateComputerDelegate);
                this.Invoke(UpdateComputerVariable, UpdateComputer);
            }
            else
            {
                if (oVariables.bAddComputerUpdate)
                    ComBoxComputer.Items.Add(UpdateComputer);
                else
                {
                    // ComBoxComputer.Text = UpdateComputer;
                    oVariables.sComputerName = ComBoxComputer.Text.ToString().Trim();
                }
            }
        }

        private void ComBoxComputer_SelectedIndexChanged(object sender, EventArgs e)
        {
            oVariables.sComputerName = ComBoxComputer.Text.ToString().Trim();
            oVariables.IsRemote = oVariables.CheckIsRemote(oVariables.sComputerName);
            oVariables.GetQueueFormatType(oVariables.sComputerName);
        }

        private void ComBoxComputer_TextChanged(object sender, EventArgs e)
        {
            oVariables.sComputerName = ComBoxComputer.Text.ToString().Trim();
            oVariables.IsRemote = oVariables.CheckIsRemote(oVariables.sComputerName);
            oVariables.GetQueueFormatType(oVariables.sComputerName);
        }
        #endregion

        #region MSMQ Qxplorer Calls
        //private void CreateQueue()
        //{

        //    if (String.IsNullOrEmpty(txtQueueName.Text.ToString().Trim()))
        //    {
        //        MessageBox.Show("Information is missing");
        //        txtQueueName.BorderStyle = BorderStyle.FixedSingle;
        //        return;
        //    }
        //    txtQueueName.BorderStyle = BorderStyle.Fixed3D;
        //    oVariables.sStatusMessage = "Create Queue " + txtQueueName.Text.ToString().Trim();

        //    oVariables.GetQueueFormatType(oVariables.sComputerName);
        //    oMSMQ.CreateQueue(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType, txtQueueName.Text.ToString().Trim(), oVariables.bIsTransactional);

        //    oVariables.sStatusMessage = "Creating Queue ... Done , Please click refresh ";
        //}

        private void DeleteQueue()
        {
            oVariables.GetQueueFormatType(oVariables.sComputerName);
            oVariables.sStatusMessage = "Deleting " + txtQueueName.Text.ToString();

            try
            {
                if (oVariables.IsRemote)
                {
                    MessageBox.Show("Delete Queue in remote computer not supported ",
                        "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                if (!(oVariables.IsHostname))
                {
                    MessageBox.Show("IP Address in not supported, Please use hostname to delete queue",
                        "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                oMSMQ.DeleteQueue(ref oVariables, oVariables.sComputerName + oVariables.sQueueType, txtQueueName.Text.ToString().Trim(), false);
                oVariables.sStatusMessage = "Deleting " + txtQueueName.Text.ToString() + " Done...";
            }
            catch (Exception Ex)
            {
                oVariables.sStatusMessage = "Encountered an Error " + Ex.Message.ToString();
            }

            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();
        }

        private void PurgeQueue()
        {
            oVariables.GetQueueFormatType(oVariables.sComputerName);
            oVariables.sStatusMessage = "Purging " + txtQueueName.Text.ToString();

            try
            {
                if ((!(oVariables.IsHostname)) && (oVariables.IsRemote))
                {
                    MessageBox.Show("Purge Queue in remote using IP address in not supported, Please use hostname to purge queue",
                        "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                oMSMQ.PurgeQueue(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType, txtQueueName.Text.ToString().Trim(), false);
                oVariables.sStatusMessage = "Message in " + txtQueueName.Text.ToString() + " succesfully purge";
            }
            catch (Exception Ex)
            {
                oVariables.sStatusMessage = "Encountered an Error " + Ex.Message.ToString();
            }


            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();
        }

        private void SendMessage()
        {
            oVariables.sComputerName = ComBoxComputer.Text.ToString().Trim();
            oVariables.IsRemote = oVariables.CheckIsRemote(oVariables.sComputerName);
            oVariables.GetQueueFormatType(oVariables.sComputerName);

            oVariables.bsendmessageStripMenuItem = false;
            oVariables.bSendMessagetoolStripButton = false;
            oVariables.bbtnSendMessage = false;
            oVariables.bCancelAllProcess = true;
            try
            {
                if (String.IsNullOrEmpty(txtQueueName.Text.Trim()))
                {
                    MessageBox.Show("Information is missing");
                    if (String.IsNullOrEmpty(txtQueuelabel.Text.Trim()))
                        txtQueuelabel.BorderStyle = BorderStyle.FixedSingle;
                    else
                        txtQueuelabel.BorderStyle = BorderStyle.Fixed3D;
                    if (String.IsNullOrEmpty(txtQueueName.Text.Trim()))
                        txtQueueName.BorderStyle = BorderStyle.FixedSingle;
                    else
                        txtQueueName.BorderStyle = BorderStyle.Fixed3D;
                    if (String.IsNullOrEmpty(txtBody.Text.Trim()))
                        txtBody.BorderStyle = BorderStyle.FixedSingle;
                    else
                        txtBody.BorderStyle = BorderStyle.Fixed3D;
                    oVariables.bsendmessageStripMenuItem = true;
                    oVariables.bSendMessagetoolStripButton = true;
                    oVariables.bbtnSendMessage = true;

                    oVariables.sStatusMessage = "Error sending message to " + txtQueueName.Text.Trim();
                    return;

                }
                txtQueueName.BorderStyle = BorderStyle.Fixed3D;
                txtQueuelabel.BorderStyle = BorderStyle.Fixed3D;
                txtBody.BorderStyle = BorderStyle.Fixed3D;

                if ((oVariables.IsRemote) && (oVariables.sQueueType.Equals("\\private$")))
                {
                    DialogResult result = MessageBox.Show("These tool cannot be determined if the queue is transactional or Non-transactional" + "\r\nPlease provide additional information of these queue\r\n\r\n" + "Select Yes if Transactional and No if Non-trasactional",
                                        "Message Queuing Admin", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                        oVariables.IsTransactional = true;
                    else if (result == DialogResult.No)
                        oVariables.IsTransactional = false;
                    else if (result == DialogResult.Cancel)
                    {
                        oVariables.bsendmessageStripMenuItem = true;
                        oVariables.bSendMessagetoolStripButton = true;
                        oVariables.bbtnSendMessage = true;
                        return;
                    }
                }
                if (numericUpDownCopy.Value > 1)
                {
                    GlobalVariables MultiMessage = new GlobalVariables(); ;
                    MultiMessage.IsJournal = oVariables.IsJournal;
                    MultiMessage.IsRemote = oVariables.IsRemote;
                    MultiMessage.IsTransactional = oVariables.IsTransactional;
                    MultiMessage.sComputerName = oVariables.sComputerName;
                    MultiMessage.sQueueFormatType = oVariables.sQueueFormatType;
                    MultiMessage.sQueueType = oVariables.sQueueType;

                    MultiMessage.sMessageBody = txtBody.Text.Trim();
                    MultiMessage.sMessageLabel = txtQueuelabel.Text.Trim().ToString();
                    MultiMessage.sMessagePriority = comboPriority.Text.ToString();
                    MultiMessage.sMessageQueuename = txtQueueName.Text.Trim().ToString();
                    MultiMessage.iNumberofCopy = Convert.ToInt32(numericUpDownCopy.Value);
                    MultiMessage.iMessageDelay = Convert.ToInt32(numericUpDownDelay.Value);

                    Thread newThread = new Thread(delegate()
                    {
                        Application.DoEvents();
                        SendMultiMessage(MultiMessage.sQueueFormatType, MultiMessage.sComputerName, MultiMessage.sQueueType, MultiMessage.sMessageQueuename,
                            MultiMessage.sMessageLabel, MultiMessage.sMessageBody, MultiMessage.IsTransactional, MultiMessage.sMessagePriority,
                            MultiMessage.IsJournal, MultiMessage.IsRemote, MultiMessage.iNumberofCopy, MultiMessage.iMessageDelay);
                    });
                    newThread.Start();
                    while (newThread.IsAlive)
                    {
                        Application.DoEvents();
                        if (oVariables.bCancelAllProcessFlag)
                        {
                            oVariables.bCancelAllProcess = false;
                            oVariables.sStatusMessage = "Sending message cancelled";
                            newThread.Abort();
                        }
                    }
                    oVariables.sStatusMessage = "Sending message done";
                }
                else
                {
                    oVariables.sStatusMessage = "Start Sending message to " + txtQueueName.Text.Trim();
                    oMSMQ.SendMessage(oVariables.sQueueType, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                        txtQueueName.Text.Trim(), txtQueuelabel.Text.Trim(), txtBody.Text.Trim(), oVariables.IsTransactional, comboPriority.Text, oVariables.IsJournal, oVariables.IsRemote);
                    //MSMQ.SendMessageQ(MQ_String,
                    //        cboNetworkComputers.Text.ToString().Trim() + GetQueueType(), txtQueueName.Text,
                    //        TransactionType(), txtLabel.Text, txtBody.Text, cboPriority.Text, 0, IsJournal(), false);
                    oVariables.sStatusMessage = "Sending message done";
                }
            }
            catch (Exception Ex)
            {
                oVariables.sStatusMessage = "Encountered an Error " + Ex.Message.ToString();
            }
            oVariables.bsendmessageStripMenuItem = true;
            oVariables.bSendMessagetoolStripButton = true;
            oVariables.bbtnSendMessage = true;
            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();
        }

        private void SendMultiMessage(string sQueueFormatType, string sComputerName, string sQueueType, string sMessageQueuename,
            string sMessageLabel, string sMessageBody, bool IsTransactional, string sMessagePriority, bool IsJournal, bool IsRemote, int iNumberofCopy, int iMessageDelay)
        {
            int counter = 0;
            for (int i = 1; i < iNumberofCopy + 1; i++)
            {
                Application.DoEvents();

                oVariables.sStatusMessage = i + " send message";
                //txtBody.Text += IsTransactional.ToString() + "\r\n"; 
                oMSMQ.SendMessage(sQueueType, sQueueFormatType, sComputerName + sQueueType,
                    sMessageQueuename, sMessageLabel, sMessageBody, IsTransactional, sMessagePriority, IsJournal, IsRemote);


                oVariables.sStatusMessage = "Sending message done";

                for (int ii = 0; ii < iMessageDelay; ii++)
                {
                    Decimal delay = iMessageDelay - ii;
                    oVariables.sStatusMessage = "Will now delay " + delay + " second\\s " + iMessageDelay + " - " + i + " Message sent";
                    Thread.Sleep(2000);
                }
                oVariables.sStatusMessage = i + " send message";
                counter = i;
                oVariables.bCancelAllProcess = true;
                if (oVariables.bCancelAllProcessFlag)
                {
                    oVariables.bCancelAllProcess = false;
                    oVariables.sStatusMessage = "Sending message cancelled";
                    break;
                }
            }
            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();

        }
        private void DeleteMessage()
        {
            oVariables.sQueueFormatType = oVariables.GetQueueFormatType(oVariables.sComputerName);
            oVariables.sStatusMessage = "Deleting message";
            oMSMQ.DeleteMessage(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                  txtQueueName.Text.Trim(), txtQueueName.Tag.ToString(), txtQueuelabel.Text.Trim());
            oVariables.sStatusMessage = "Succesfully delete message";
            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();
        }

        private void ResendMessage()
        {

            oVariables.sComputerName = ComBoxComputer.Text.ToString().Trim();
            oVariables.IsRemote = oVariables.CheckIsRemote(oVariables.sComputerName);
            oVariables.GetQueueFormatType(oVariables.sComputerName);

            oVariables.bsendmessageStripMenuItem = false;
            oVariables.bSendMessagetoolStripButton = false;
            oVariables.bbtnSendMessage = false;
            oVariables.bCancelAllProcess = true;
            try
            {
                if (String.IsNullOrEmpty(txtQueueName.Text.Trim()))
                {
                    MessageBox.Show("Information is missing");
                    if (String.IsNullOrEmpty(txtQueuelabel.Text.Trim()))
                        txtQueuelabel.BorderStyle = BorderStyle.FixedSingle;
                    else
                        txtQueuelabel.BorderStyle = BorderStyle.Fixed3D;
                    if (String.IsNullOrEmpty(txtQueueName.Text.Trim()))
                        txtQueueName.BorderStyle = BorderStyle.FixedSingle;
                    else
                        txtQueueName.BorderStyle = BorderStyle.Fixed3D;
                    if (String.IsNullOrEmpty(txtBody.Text.Trim()))
                        txtBody.BorderStyle = BorderStyle.FixedSingle;
                    else
                        txtBody.BorderStyle = BorderStyle.Fixed3D;
                    oVariables.bsendmessageStripMenuItem = true;
                    oVariables.bSendMessagetoolStripButton = true;
                    oVariables.bbtnSendMessage = true;
                    oVariables.sStatusMessage = "Error sending message to " + txtQueueName.Text.Trim();
                    return;
                }
                txtQueueName.BorderStyle = BorderStyle.Fixed3D;
                txtQueuelabel.BorderStyle = BorderStyle.Fixed3D;
                txtBody.BorderStyle = BorderStyle.Fixed3D;

                if ((oVariables.IsRemote) && (oVariables.sQueueType.Equals("\\private$")))
                {
                    DialogResult result = MessageBox.Show("These tool cannot be determined if the queue is transactional or Non-transactional" + "\r\nPlease provide additional information of these queue\r\n\r\n" + "Select Yes if Transactional and No if Non-trasactional",
                                        "Message Queuing Admin", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                        oVariables.IsTransactional = true;
                    else if (result == DialogResult.No)
                        oVariables.IsTransactional = false;
                    else if (result == DialogResult.Cancel)
                        return;
                }
                oVariables.sStatusMessage = "Deleting message";
                oMSMQ.DeleteMessage(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                      txtQueueName.Text.Trim(), txtQueueName.Tag.ToString(), txtQueuelabel.Text.Trim());
                oVariables.sStatusMessage = "Succesfully delete message";

                oVariables.sStatusMessage = "Start Sending message to " + txtQueueName.Text.Trim();
                oMSMQ.SendMessage(oVariables.sQueueType, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                    txtQueueName.Text.Trim(), txtQueuelabel.Text.Trim(), txtBody.Text.Trim(), oVariables.IsTransactional, comboPriority.Text, oVariables.IsJournal, oVariables.IsRemote);
                //MSMQ.SendMessageQ(MQ_String,
                //        cboNetworkComputers.Text.ToString().Trim() + GetQueueType(), txtQueueName.Text,
                //        TransactionType(), txtLabel.Text, txtBody.Text, cboPriority.Text, 0, IsJournal(), false);
                oVariables.sStatusMessage = "Sending message done";
            }
            catch { }
            oVariables.bsendmessageStripMenuItem = true;
            oVariables.bSendMessagetoolStripButton = true;
            oVariables.bbtnSendMessage = true;
        }


        private void PurgeAllQueue()
        {
            DialogResult result = MessageBox.Show("This will purge all the queues in a current machine ->" + oVariables.sComputerName + " \n Would you like to continue?",
          "Message Queuing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            oVariables.sStatusMessage = "Purge all message queue";
            if (result == DialogResult.Yes)
            {
                try
                {
                    MSMQTView.Cursor = Cursors.WaitCursor;
                    oVariables.bPurgeAllQRunning = true;
                    oVariables.bbtnRefreshQueue = false;
                    oVariables.brefreshQueueToolStripMenuItem = false;
                    oVariables.bpurgeAllQueueToolStripMenuItem = false;
                    oVariables.bPurgeQueuetoolStripButton = false;
                    oVariables.bbtnPurgeQueue = false;
                    oVariables.bpurgeQueueToolStripMenuItem = false;
                    oVariables.bdeleteAllQueueToolStripMenuItem = false;

                    if (string.IsNullOrEmpty(oVariables.sQueueType))
                    {

                        foreach (StructuredQueue oQueues in oVariables.oPublicQueue_ARRAY)
                        {
                            oVariables.sStatusMessage = "Purging " + oQueues.sPath;
                            Application.DoEvents();
                            if (oQueues.Message != null)
                            {
                                if (oQueues.Message.Count >= 1)
                                    oMSMQ.PurgeQueue(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType, oQueues.sQueueName, true);
                            }
                        }
                    }
                    else if (oVariables.sQueueType.Equals("\\private$"))
                    {

                        foreach (StructuredQueue oQueues in oVariables.oPrivateQueue_ARRAY)
                        {
                            oVariables.sStatusMessage = "Purging " + oQueues.sPath;
                            Application.DoEvents();
                            if (oQueues.Message != null)
                            {
                                if (oQueues.Message.Count >= 1)
                                    oMSMQ.PurgeQueue(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType, oQueues.sQueueName, true);
                            }
                        }
                    }

                    MessageBox.Show("All queues in current machine are succesfully purge",
                                       "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    oVariables.sStatusMessage = "All queues in current machine are succesfully purge";
                }
                catch (Exception Ex)
                {
                    oVariables.sStatusMessage = "Encountered an Error " + Ex.Message.ToString();
                }
            }

            oVariables.bbtnRefreshQueue = true;
            oVariables.brefreshQueueToolStripMenuItem = true;
            oVariables.bpurgeAllQueueToolStripMenuItem = true;
            oVariables.bPurgeQueuetoolStripButton = true;
            oVariables.bbtnPurgeQueue = true;
            oVariables.bpurgeQueueToolStripMenuItem = true;
            oVariables.bdeleteAllQueueToolStripMenuItem = true;
            oVariables.bPurgeAllQRunning = false;
            MSMQTView.Cursor = Cursors.Default;
            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();

        }

        private void DeleteAllQueue()
        {
            DialogResult result = MessageBox.Show("This will delete all the queues in a current machine ->" + oVariables.sComputerName + " \n Would you like to continue?",
           "Message Queuing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            oVariables.sStatusMessage = "Deleting all queue";
            if (result == DialogResult.Yes)
            {
                try
                {
                    oVariables.GetQueueFormatType(oVariables.sComputerName);
                    if (oVariables.IsRemote)
                    {
                        MessageBox.Show("Delete Queue in remote computer not supported ",
                            "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }

                    if (!(oVariables.IsHostname))
                    {
                        MessageBox.Show("IP Address in not supported, Please use hostname to delete queue",
                            "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    MSMQTView.Cursor = Cursors.WaitCursor;
                    oVariables.bPurgeAllQRunning = true;
                    oVariables.bbtnRefreshQueue = false;
                    oVariables.brefreshQueueToolStripMenuItem = false;
                    oVariables.bpurgeAllQueueToolStripMenuItem = false;
                    oVariables.bPurgeQueuetoolStripButton = false;
                    oVariables.bbtnPurgeQueue = false;
                    oVariables.bpurgeQueueToolStripMenuItem = false;
                    oVariables.bdeleteAllQueueToolStripMenuItem = false;

                    if (string.IsNullOrEmpty(oVariables.sQueueType))
                    {

                        foreach (StructuredQueue oQueues in oVariables.oPublicQueue_ARRAY)
                        {
                            oVariables.sStatusMessage = "Deleting " + oQueues.sPath;
                            Application.DoEvents();
                            oMSMQ.DeleteQueue(ref oVariables, oVariables.sComputerName + oVariables.sQueueType, oQueues.sQueueName, true);
                        }
                    }
                    else if (oVariables.sQueueType.Equals("\\private$"))
                    {

                        foreach (StructuredQueue oQueues in oVariables.oPrivateQueue_ARRAY)
                        {
                            oVariables.sStatusMessage = "Deleting " + oQueues.sPath;
                            Application.DoEvents();
                            oMSMQ.DeleteQueue(ref oVariables, oVariables.sComputerName + oVariables.sQueueType, oQueues.sQueueName, true);
                        }
                    }

                    MessageBox.Show("All queues in current machine succesfully delete",
                                       "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    oVariables.sStatusMessage = "All queues in current machine succesfully delete";

                }
                catch (Exception Ex)
                {
                    oVariables.sStatusMessage = "Encountered an Error " + Ex.Message.ToString();
                }

            }
            oVariables.bbtnRefreshQueue = true;
            oVariables.brefreshQueueToolStripMenuItem = true;
            oVariables.bpurgeAllQueueToolStripMenuItem = true;
            oVariables.bPurgeQueuetoolStripButton = true;
            oVariables.bbtnPurgeQueue = true;
            oVariables.bpurgeQueueToolStripMenuItem = true;
            oVariables.bdeleteAllQueueToolStripMenuItem = true;
            oVariables.bPurgeAllQRunning = false;
            MSMQTView.Cursor = Cursors.Default;
            if (oVariables.bAutoRefreshQueue)
                Refresh_MSMQ();
        }

        private void Refresh_MSMQ()
        {

            ResendtoolStripMenuItem.Enabled = false;
            oVariables.bbtnRefreshQueue = false;
            oVariables.brefreshQueueToolStripMenuItem = false;
            oVariables.bCancelAllProcess = true;
            cboSpecificQueue.Items.Clear();
            oVariables.GetQueueFormatType(oVariables.sComputerName);

            try
            {
                MSMQTView.Nodes.Clear();
                if (!(oVariables.bCallRefresh))
                    oVariables.sComputerName = ComBoxComputer.Text.ToString();
                else
                    ComBoxComputer.Text = oVariables.sComputerName;
                oVariables.IsRemote = oVariables.CheckIsRemote(oVariables.sComputerName);
                oVariables.bCallRefresh = false;
                if (chkOutgoingQueues.Checked)
                {
                    // oOutgoingQueue.Clear();
                }
                if (chkPublicQueues.Checked)
                {
                    MSMQTView.Nodes.RemoveByKey("Public Queue");
                    //if (oVariables.bRefreshOnly == false)
                    Application.DoEvents();
                    {
                        oVariables.oPublicQueue_ARRAY.Clear();
                        oMSMQ.GetPublicQueues(ref oVariables, oVariables.sComputerName);
                        UpdateTreeView(oVariables.oPublicQueue_ARRAY, "Public Queue", true);
                        if (oVariables.bEnableQuery)
                        {
                            UpdateTreeViewByQueueName(oVariables.oPublicQueue_ARRAY, "Public Queue");
                            //oVariables.oPublicQueue_ARRAY = oMSMQ.GetAllMessageInMSMQ(ref oVariables, oVariables.oPublicQueue_ARRAY);
                            //UpdateTreeView(oVariables.oPublicQueue_ARRAY, "Public Queue", false);
                        }
                    }
                    Application.DoEvents();
                }
                if (chkPrivateQueues.Checked)
                {
                    //GetQueus GetTest = new GetQueus();
                    MSMQTView.Nodes.RemoveByKey("Private Queue");
                    oVariables.oPrivateQueue_ARRAY.Clear();
                    //if (oVariables.bRefreshOnly == false)
                    Application.DoEvents();
                    {

                        oVariables.oPrivateQueue_ARRAY.Clear();
                        oMSMQ.GetPrivateQueues(ref oVariables, oVariables.sComputerName);
                        UpdateTreeView(oVariables.oPrivateQueue_ARRAY, "Private Queue", true);
                        if (oVariables.bEnableQuery)
                        {
                            UpdateTreeViewByQueueName(oVariables.oPrivateQueue_ARRAY, "Private Queue");
                            // oVariables.oPrivateQueue_ARRAY = oMSMQ.GetAllMessageInMSMQ(ref oVariables, oVariables.oPrivateQueue_ARRAY);

                            //UpdateTreeView(oVariables.oPrivateQueue_ARRAY, "Private Queue", false);
                        }
                    }
                    Application.DoEvents();

                }
                if (chkSystemQueues.Checked)
                {
                    // oSystemQueue.Clear();
                }
            }
            catch (Exception Ex)
            {
                oVariables.sStatusMessage = "Encountered an Error " + Ex.Message.ToString();
            }
            oVariables.bbtnRefreshQueue = true;
            oVariables.brefreshQueueToolStripMenuItem = true;
            oVariables.bCancelAllProcess = false;
            oVariables.bCancelAllProcessFlag = false;
            oVariables.bCallRefresh = false;
        }

        #endregion

        #region Button Calls
        private void btnRefreshQueue_Click(object sender, EventArgs e)
        {

            Refresh_MSMQ();
        }

        private void btnPurgeQueue_Click(object sender, EventArgs e)
        {
            PurgeQueue();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            SendMessage();
        }
        #endregion

        #region Treeview Calls
        private void MSMQTView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            oVariables.bCreateQueueStripButton = false;
            oVariables.bDeleteMessagetoolStripButton = false;
            oVariables.bDeleteQueuetoolStripButton = false;
            oVariables.bPurgeQueuetoolStripButton = false;
            oVariables.bSendMessagetoolStripButton = false;
            oVariables.bbtnPurgeQueue = false;
            oVariables.bbtnSendMessage = false;
            oVariables.bdeleteAllQueueToolStripMenuItem = false;
            oVariables.bdeleteQueueToolStripMenuItem = false;
            oVariables.bdeletemessageStripMenuItem = false;
            oVariables.bnewQueueToolStripMenuItem = false;
            oVariables.bpurgeAllQueueToolStripMenuItem = false;
            oVariables.bpurgeQueueToolStripMenuItem = false;
            oVariables.bsendmessageStripMenuItem = false;
            oVariables.bsavemessageStripMenuItem = false;

            //DgViewMessage.Visible = false;
            // txtBody.Visible = false;
            // txtBodyWeb.Visible = false;

            try
            {
                if (MSMQTView.SelectedNode.Tag.ToString() == "Queuetype")
                {
                    txtBody.BringToFront();
                    oVariables.bnewQueueToolStripMenuItem = true;
                    oVariables.bCreateQueueStripButton = true;
                    oVariables.bpurgeAllQueueToolStripMenuItem = true;
                    oVariables.bdeleteAllQueueToolStripMenuItem = true;
                    if (MSMQTView.SelectedNode.FullPath.Contains("Public Queue"))
                    {
                        oVariables.sQueueType = null;
                    }
                    else if (MSMQTView.SelectedNode.FullPath.Contains("Private Queue"))
                    {
                        oVariables.sQueueType = "\\private$";
                    }
                    else
                    {
                        oVariables.bnewQueueToolStripMenuItem = false;
                        oVariables.bCreateQueueStripButton = false;
                    }
                    MSMQTView.SelectedNode.ImageIndex = 4;
                    return;
                }
                if (MSMQTView.SelectedNode.Tag.ToString().Contains("Queues|||||"))
                {
                    txtQueueName.Text = MSMQTView.SelectedNode.Name;
                    ComboFilterList.Text = MSMQTView.SelectedNode.Name;
                    string[] MySelectedMessage = oVariables.SplitByString(MSMQTView.SelectedNode.Tag.ToString(), "|||||");
                    if (!(oVariables.bPurgeAllQRunning))
                    {
                        oVariables.bdeleteQueueToolStripMenuItem = true;
                        oVariables.bDeleteQueuetoolStripButton = true;
                        if (Convert.ToInt32(MySelectedMessage[3]) > 0)
                        {
                            oVariables.bPurgeQueuetoolStripButton = true;
                            oVariables.bpurgeQueueToolStripMenuItem = true;
                            oVariables.bbtnPurgeQueue = true;
                        }
                        else
                        {
                            oVariables.bPurgeQueuetoolStripButton = false;
                            oVariables.bpurgeQueueToolStripMenuItem = false;
                            oVariables.bbtnPurgeQueue = false;
                        }

                        oVariables.bbtnSendMessage = true;
                        oVariables.bpurgeAllQueueToolStripMenuItem = true;
                        oVariables.bdeleteAllQueueToolStripMenuItem = true;
                    }
                    oVariables.bsendmessageStripMenuItem = true;
                    oVariables.bSendMessagetoolStripButton = true;

                    oVariables.bnewQueueToolStripMenuItem = true;
                    oVariables.bCreateQueueStripButton = true;
                    oVariables.bsavemessageStripMenuItem = true;


                    //if (oVar.IsRemote)
                    //    { oQueue.bTransactional = 1; }
                    //    else if (PrivateQueues[i].Transactional)
                    //    { oQueue.bTransactional = 2; }
                    //    else
                    //    { oQueue.bTransactional = 3;}

                    if (MySelectedMessage[2] == "2")
                        oVariables.IsTransactional = true;
                    else if (MySelectedMessage[2] == "3")
                        oVariables.IsTransactional = false;
                    if (rdoBytext.Checked)
                        oVariables.bLastRdo = true;
                    else if (rdoByXML.Checked)
                        oVariables.bLastRdo = false;
                    //DgViewMessage.Visible = true;

                    MSMQTView.SelectedNode.Parent.ImageIndex = 4;
                    if (MSMQTView.SelectedNode.FullPath.Contains("Public Queue"))
                    {
                        oVariables.sQueueType = null;
                        if ((Convert.ToInt32(MySelectedMessage[3]) > 0) && oVariables.bViewByTable)
                        {
                            StructuredQueue Queues = (StructuredQueue)oVariables.oPublicQueue_ARRAY.GetRange(Convert.ToInt32(MySelectedMessage[1]), 1)[0];
                            Update_DataGridMessage(Queues);
                            return;
                        }
                    }
                    else if (MSMQTView.SelectedNode.FullPath.Contains("Private Queue"))
                    {
                        oVariables.sQueueType = "\\private$";
                        if ((Convert.ToInt32(MySelectedMessage[3]) > 0) && oVariables.bViewByTable)
                        {
                            StructuredQueue Queues = (StructuredQueue)oVariables.oPrivateQueue_ARRAY.GetRange(Convert.ToInt32(MySelectedMessage[1]), 1)[0];
                            Update_DataGridMessage(Queues);
                            return;
                        }
                    }
                    //else
                    //{
                    //    oVariables.bnewQueueToolStripMenuItem = false;
                    //    oVariables.bCreateQueueStripButton = false;
                    //    oVariables.bsendmessageStripMenuItem = false;
                    //    oVariables.bSendMessagetoolStripButton = false;
                    //    oVariables.bdeleteQueueToolStripMenuItem = false;
                    //    oVariables.bDeleteQueuetoolStripButton = false;
                    //    oVariables.bdeleteAllQueueToolStripMenuItem = false;
                    //    oVariables.bbtnSendMessage = false;

                    //}
                    DgViewMessage.SendToBack();
                    if (oVariables.bLastRdo)
                    {
                        txtBodyWeb.SendToBack();
                        txtBody.BringToFront();
                        rdoBytext.Checked = true;
                    }
                    else
                    {
                        txtBody.SendToBack();
                        txtBodyWeb.BringToFront();
                        rdoByXML.Checked = true;
                    }
                    return;

                }
                if (MSMQTView.SelectedNode.Tag.ToString().Contains("Message|||||"))
                {

                    // txtBody.Visible = true;
                    // txtBodyWeb.Visible = true;
                    oVariables.sStatusMessage = "Current queue has " + MSMQTView.SelectedNode.Parent.Name + " message";
                    txtQueueName.Text = MSMQTView.SelectedNode.Name;

                    if (!(oVariables.bPurgeAllQRunning))
                    {
                        oVariables.bdeleteQueueToolStripMenuItem = true;
                        oVariables.bDeleteQueuetoolStripButton = true;
                        oVariables.bPurgeQueuetoolStripButton = true;
                        oVariables.bpurgeQueueToolStripMenuItem = true;
                        oVariables.bbtnPurgeQueue = true;
                        oVariables.bbtnSendMessage = true;
                        oVariables.bpurgeAllQueueToolStripMenuItem = true;
                        oVariables.bdeleteAllQueueToolStripMenuItem = true;
                        oVariables.bDeleteMessagetoolStripButton = true;
                        oVariables.bdeletemessageStripMenuItem = true;
                    }

                    oVariables.bsendmessageStripMenuItem = true;
                    oVariables.bSendMessagetoolStripButton = true;
                    oVariables.bbtnSendMessage = true;
                    oVariables.bnewQueueToolStripMenuItem = true;
                    oVariables.bCreateQueueStripButton = true;
                    oVariables.bsavemessageStripMenuItem = true;


                    ArrayList TVQueue = new ArrayList();
                    if (MSMQTView.SelectedNode.FullPath.Contains("Public Queue"))
                    {
                        oVariables.sQueueType = null;
                        TVQueue = oVariables.oPublicQueue_ARRAY;
                    }
                    else if (MSMQTView.SelectedNode.FullPath.Contains("Private Queue"))
                    {
                        oVariables.sQueueType = "\\private$";
                        TVQueue = oVariables.oPrivateQueue_ARRAY;
                    }
                    else
                    {
                        oVariables.bnewQueueToolStripMenuItem = false;
                        oVariables.bCreateQueueStripButton = false;
                        oVariables.bsendmessageStripMenuItem = false;
                        oVariables.bSendMessagetoolStripButton = false;
                        oVariables.bdeleteQueueToolStripMenuItem = false;
                        oVariables.bDeleteQueuetoolStripButton = false;
                        oVariables.bdeleteAllQueueToolStripMenuItem = false;
                        oVariables.bbtnSendMessage = false;
                        oVariables.bDeleteMessagetoolStripButton = false;
                        oVariables.bdeletemessageStripMenuItem = false;
                    }
                    MSMQTView.SelectedNode.Parent.Parent.ImageIndex = 4;


                    string[] MySelectedMessage = oVariables.SplitByString(MSMQTView.SelectedNode.Tag.ToString(), "|||||");
                    if (MySelectedMessage[3] == "2")
                        oVariables.IsTransactional = true;
                    else if (MySelectedMessage[3] == "3")
                        oVariables.IsTransactional = false;
                    StructuredQueue Queues = (StructuredQueue)TVQueue.GetRange(Convert.ToInt32(MySelectedMessage[2]), 1)[0];
                    QueueInfos Messages = (QueueInfos)Queues.Message[Convert.ToInt32(MySelectedMessage[1])];
                    SetQueueInfos(Messages);
                    //foreach (StructuredQueue Queues in TVQueue)
                    //{
                    //    foreach (QueueInfos Messages in Queues.Message)
                    //    {
                    //        if (MSMQTView.SelectedNode.Name == Messages.ID)
                    //        {
                    //            SetQueueInfos(Messages);
                    //            break;
                    //        }

                    //    }
                    //}
                    return;
                }
            }
            catch
            {
            }
        }

          private void SetQueueInfos(QueueInfos Messages)
        {

            if (oVariables.bLastRdo)
            {
                txtBodyWeb.SendToBack();
                txtBody.BringToFront();
                rdoBytext.Checked = true;
            }
            else
            {
                txtBody.SendToBack();
                txtBodyWeb.BringToFront();
                rdoByXML.Checked = true;
            }
            //txtQueueName.Text = "";
            //txtBody.Text = "";
            //txtLabel.Text = "";
            //cboPriority.Text = "";
            //txtMessageID.Text = "";
            //txtSentTime.Text = "";
            //txtPathName.Text = "";
            txtQueueName.Text = Messages.Queue;

            txtQueueName.Tag = Messages.ID;
            // txtBody.Text = Messages.Body;
            //objVar.TempMessageBody = Messages.Body;
            txtQueuelabel.Text = Messages.Label;
            comboPriority.Text = Messages.Priority;
            // .Text = Messages.Priority;
            //txtMessageID.Text = Messages.ID;
            //txtSentTime.Text = Messages.SentTime;
            //txtPathName.Text = Messages.Path;
            oVariables.sCurrentMessageBody = Messages.Body;
            if (rdoBytext.Checked)
            {
                txtBody.BringToFront();
                txtBody.Text = Messages.Body;
            }
            else if (rdoByXML.Checked)
            {
                txtBodyWeb.BringToFront();
                using (StreamWriter myStream = new StreamWriter(oVariables.sTempFolder + "\\messagebody.tmp"))
                {
                    if (!(String.IsNullOrEmpty(Messages.Body)))
                        myStream.Write(Messages.Body);
                    else if (!(String.IsNullOrEmpty(txtBody.Text.ToString())))
                        myStream.Write(txtBody.Text.ToString());
                    else
                        myStream.Write("Message Body is Empty");
                    myStream.Close();

                }
                oVariables.sWebUrl = oVariables.sTempFolder + "\\messagebody.tmp";
                txtBodyWeb.Navigate(oVariables.sWebUrl);
            }
            else
            {
            }


            //if (MSMQTView.SelectedNode.ImageIndex == 2)
            //    oVariables.IsTransactional = true;
            //else if (MSMQTView.SelectedNode.ImageIndex == 3)
            //    oVariables.IsTransactional = false;



        }

        private void UpdateTreeView(ArrayList ArrayQueue, string QueueType, bool flag)
        {
            MSMQTView.BeginUpdate();
            MSMQTView.ImageList = imageList1;
            if (flag)
            {
                TreeNode pNode = new TreeNode();
                pNode.Tag = "Queuetype";
                pNode.Text = QueueType + "(" + ArrayQueue.Count + ")";
                pNode.Name = QueueType;
                pNode.ImageIndex = 4;

                bool NodeNotFound = true;
                foreach (TreeNode ParentNode in MSMQTView.Nodes)
                {
                    if (ParentNode.Name == QueueType)
                        NodeNotFound = false;
                }
                if (NodeNotFound)
                    MSMQTView.Nodes.Add(pNode);

                foreach (StructuredQueue oQueues in ArrayQueue)
                {

                    Application.DoEvents();
                    string[] test = oQueues.sPath.Split(new Char[] { ':' });
                    if (test.Length == 3)
                        cboSpecificQueue.Items.Add(test[2]);
                    else
                        cboSpecificQueue.Items.Add(oQueues.sPath);

                    if (oVariables.bEnableFilter)
                    {
                        foreach (string sQueueNamecompare in oVariables.oFilterList_ARRAY)
                        {
                            string sStringToMatch = sQueueNamecompare.Replace("+", "");
                            if (sQueueNamecompare.StartsWith("+"))
                            {
                                if (!(Regex.IsMatch(oQueues.sQueueName, sStringToMatch)))
                                //if (oQueues.sQueueName.Contains(sQueueNamecompare))
                                {
                                    TreeNode qNode = new TreeNode();
                                    oVariables.sStatusMessage = "Updating " + oQueues.sQueueName + "Queue";
                                    qNode.Tag = "Queues|||||" + oQueues.iQueueId + "|||||" + oQueues.bTransactional.ToString() + "|||||" + oQueues.CountMessage;
                                    qNode.Name = oQueues.sQueueName.ToString();
                                    qNode.Text = oQueues.sQueueName + "(" + oQueues.CountMessage + ")";

                                    qNode.ImageIndex = 0;
                                    if (oQueues.bTransactional == 2)
                                        qNode.ImageIndex = 1;
                                    else if (oQueues.bTransactional == 3)
                                        qNode.ImageIndex = 3;
                                    else if (oQueues.bTransactional == 1)
                                        qNode.ImageIndex = 5;

                                    pNode.Nodes.Add(qNode);
                                    break;
                                }
                            }
                            else
                            {
                                if (Regex.IsMatch(oQueues.sQueueName, sStringToMatch))
                                //if (oQueues.sQueueName.Contains(sQueueNamecompare))
                                {
                                    TreeNode qNode = new TreeNode();
                                    oVariables.sStatusMessage = "Updating " + oQueues.sQueueName + "Queue";
                                    qNode.Tag = "Queues|||||" + oQueues.iQueueId + "|||||" + oQueues.bTransactional.ToString() + "|||||" + oQueues.CountMessage;
                                    qNode.Name = oQueues.sQueueName.ToString();
                                    qNode.Text = oQueues.sQueueName + "(" + oQueues.CountMessage + ")";

                                    qNode.ImageIndex = 0;
                                    if (oQueues.bTransactional == 2)
                                        qNode.ImageIndex = 1;
                                    else if (oQueues.bTransactional == 3)
                                        qNode.ImageIndex = 3;
                                    else if (oQueues.bTransactional == 1)
                                        qNode.ImageIndex = 5;

                                    pNode.Nodes.Add(qNode);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        TreeNode qNode = new TreeNode();
                        oVariables.sStatusMessage = "Updating " + oQueues.sQueueName + "Queue";
                        qNode.Tag = "Queues|||||" + oQueues.iQueueId + "|||||" + oQueues.bTransactional.ToString() + "|||||" + oQueues.CountMessage;
                        qNode.Name = oQueues.sQueueName.ToString();
                        qNode.Text = oQueues.sQueueName + "(" + oQueues.CountMessage + ")";

                        qNode.ImageIndex = 0;
                        if (oQueues.bTransactional == 2)
                            qNode.ImageIndex = 1;
                        else if (oQueues.bTransactional == 3)
                            qNode.ImageIndex = 3;
                        else if (oQueues.bTransactional == 1)
                            qNode.ImageIndex = 5;

                        pNode.Nodes.Add(qNode);
                    }
                }
                pNode.Expand();

                if (oVariables.bSortQueue)
                {
                    if (oVariables.bMessageQueueSorting)
                    {
                        MSMQTView.TreeViewNodeSorter = new NodeSorter(pNode, "Ascending");
                        MSMQTView.Sort();
                    }
                    else
                    {
                        MSMQTView.TreeViewNodeSorter = new NodeSorter(pNode, "Descending");
                        MSMQTView.Sort();
                    }

                }

                MSMQTView.EndUpdate();
            }
            else
            {
                TreeNodeCollection nodes = MSMQTView.Nodes;
                foreach (TreeNode n in nodes)
                {
                    if (n.Name == QueueType)
                    {
                        foreach (TreeNode node2 in n.Nodes)
                        {
                            foreach (StructuredQueue oQueues in ArrayQueue)
                            {
                                if (oQueues.sQueueName != null)
                                {
                                    if (node2.Name == oQueues.sQueueName.ToString())
                                    {

                                        if (oQueues.Message != null)
                                        {
                                            progressBar1.Maximum = oQueues.Message.Count + 1;
                                            if (oVariables.bCancelAllProcessFlag)
                                            {
                                                progressBar1.Value = oQueues.Message.Count;
                                                oVariables.bCancelAllProcessFlag = false;
                                                break;
                                            }

                                            MSMQTView.Cursor = Cursors.WaitCursor;
                                            node2.Text = oQueues.sQueueName + "(" + oQueues.CountMessage + ")";
                                            // MSMQTView.BeginUpdate();
                                            //TreeNode myNode = new TreeNode();
                                            foreach (QueueInfos Messages in oQueues.Message)
                                            {
                                                Application.DoEvents();
                                                oVariables.sStatusMessage = "Loading " + Messages.ID + " to " + oQueues.sQueueName + " Please wait...";
                                                if (progressBar1.Maximum > 0)
                                                {
                                                    if (progressBar1.Maximum > progressBar1.Value)
                                                        progressBar1.Value += 1;
                                                    //node2.Text = oQueues.sQueueName + "(" + progressBar1.Value + ")";
                                                }
                                                if (oVariables.bCancelAllProcessFlag)
                                                {
                                                    progressBar1.Value = oQueues.Message.Count;
                                                    oVariables.bCancelAllProcessFlag = false;
                                                    break;
                                                }
                                                TreeNode mNode = new TreeNode();
                                                if (Messages.Transact == 2)
                                                    mNode.ImageIndex = 1;
                                                else if (Messages.Transact == 3)
                                                    mNode.ImageIndex = 3;
                                                else if (Messages.Transact == 1)
                                                    mNode.ImageIndex = 5;
                                                mNode.Tag = "Message|||||" + Messages.MessageID + "|||||" + oQueues.iQueueId + "|||||" + oQueues.bTransactional.ToString();
                                                mNode.Name = Messages.ID;
                                                mNode.Text = Messages.Label;

                                                //myNode.Nodes.Add(mNode);
                                                node2.Nodes.Add(mNode);
                                                Application.DoEvents();
                                                //MSMQTView.EndUpdate();
                                            }

                                            //// Create an array of 'TreeNodes'.
                                            //TreeNode[] myTreeNodeArray = new TreeNode[myNode.Nodes.Count];
                                            //// Copy the tree nodes to the 'myTreeNodeArray' array.
                                            //myNode.Nodes.CopyTo(myTreeNodeArray, 0);
                                            //// Remove all the tree nodes from the 'myTreeViewBase' TreeView.
                                            //myNode.Nodes.Clear();
                                            //// Add the 'myTreeNodeArray' to the 'myTreeViewCustom' TreeView.
                                            //Application.DoEvents();
                                            //node2.Nodes.AddRange(myTreeNodeArray);
                                            MSMQTView.Cursor = Cursors.Default;
                                            MSMQTView.EndUpdate();
                                            oVariables.sStatusMessage = "Queue " + oQueues.sQueueName + " has already updated";
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                MSMQTView.EndUpdate();
            }
            MSMQTView.EndUpdate();
        }

        private void UpdateTreeViewByQueueName(ArrayList ArrayQueue, string QueueType)
        {
            MessageInfo MyInfo = new MessageInfo();
            int MaxQuery = oVariables.iMaxMessageToQuery;
            bool bQueryMessage = oVariables.bQueryMessage;


            MSMQTView.ImageList = imageList1;
            TreeNodeCollection nodes = MSMQTView.Nodes;
            foreach (TreeNode n in nodes)
            {
                if (n.Name == QueueType)
                {
                    foreach (TreeNode node2 in n.Nodes)
                    {
                        string[] MySelectedMessage = oVariables.SplitByString(node2.Tag.ToString(), "|||||");
                        StructuredQueue Queues = (StructuredQueue)ArrayQueue.GetRange(Convert.ToInt32(MySelectedMessage[1]), 1)[0];

                        //StructuredQueue oQueue = new StructuredQueue();
                        //oQueue = Queues;
                        oVariables.iProgressValue = Queues.iQueueId;
                        Application.DoEvents();
                        oVariables.sStatusMessage = "Quering " + Queues.sPath + " Please wait...";
                        Thread newThread = new Thread(delegate()
                        {
                            Application.DoEvents();
                            if (bQueryMessage)
                                MyInfo = oMSMQ.PeekAllMessage(Queues, Queues.iQueueId, MaxQuery);
                            else
                                MyInfo = oMSMQ.GetAllMessage(Queues, Queues.iQueueId, MaxQuery);
                        });
                        newThread.Start();
                        while (newThread.IsAlive)
                        {
                            Application.DoEvents();
                            if (oVariables.bCancelAllProcessFlag)
                            {
                                oVariables.bCancelAllProcessFlag = false;
                                newThread.Abort();
                            }
                        }
                        Queues.CountMessage = MyInfo.ID;

                        Queues.Message = MyInfo.Message;
                        if (node2.FullPath.Contains("Public Queue"))
                        {
                            oVariables.oPublicQueue_ARRAY.RemoveAt(Convert.ToInt32(MySelectedMessage[1]));
                            oVariables.oPublicQueue_ARRAY.Insert(Convert.ToInt32(MySelectedMessage[1]), Queues);
                        }
                        else if (node2.FullPath.Contains("Private Queue"))
                        {
                            oVariables.oPrivateQueue_ARRAY.RemoveAt(Convert.ToInt32(MySelectedMessage[1]));
                            oVariables.oPrivateQueue_ARRAY.Insert(Convert.ToInt32(MySelectedMessage[1]), Queues);
                        }


                        MSMQTView.Cursor = Cursors.WaitCursor;
                        node2.Text = Queues.sQueueName + "(" + MyInfo.ID + ")";
                        node2.Tag = "Queues|||||" + Queues.iQueueId + "|||||" + Queues.bTransactional.ToString() + "|||||" + Queues.CountMessage;
                        // MSMQTView.BeginUpdate();
                        //TreeNode myNode = new TreeNode();
                        node2.Nodes.Clear();
                        if (MyInfo.Message != null)
                        {
                            progressBar1.Maximum = MyInfo.Message.Count + 1;
                            MSMQTView.BeginUpdate();
                            foreach (QueueInfos Messages in MyInfo.Message)
                            {
                                Application.DoEvents();
                                oVariables.sStatusMessage = "Loading " + Messages.ID + " to " + Queues.sQueueName + " Please wait...";
                                if (progressBar1.Maximum > 0)
                                {
                                    if (progressBar1.Maximum > progressBar1.Value)
                                        progressBar1.Value += 1;
                                    //node2.Text = oQueues.sQueueName + "(" + progressBar1.Value + ")";
                                }
                                if (oVariables.bCancelAllProcessFlag)
                                {
                                    progressBar1.Value = Queues.Message.Count;
                                    MSMQTView.EndUpdate();
                                    MSMQTView.Cursor = Cursors.Default;
                                    oVariables.bCancelAllProcessFlag = false;
                                    break;
                                }
                                TreeNode mNode = new TreeNode();
                                if (Messages.Transact == 2)
                                    mNode.ImageIndex = 1;
                                else if (Messages.Transact == 3)
                                    mNode.ImageIndex = 3;
                                else if (Messages.Transact == 1)
                                    mNode.ImageIndex = 5;
                                mNode.Tag = "Message|||||" + Messages.MessageID + "|||||" + Queues.iQueueId + "|||||" + Queues.bTransactional.ToString();
                                mNode.Name = Messages.ID;
                                mNode.Text = Messages.Label;

                                //myNode.Nodes.Add(mNode);
                                node2.Nodes.Add(mNode);
                                Application.DoEvents();
                                //MSMQTView.EndUpdate();
                            }
                            oVariables.sStatusMessage = "Queue " + Queues.sQueueName + " has already updated.. There are " + MyInfo.Message.Count + " message queried";
                            MSMQTView.EndUpdate();
                        }
                    }

                }

            }
            MSMQTView.EndUpdate();
            MSMQTView.Cursor = Cursors.Default;
        }

        private void UpdateTreeViewbySpecificQueuename()
        {
            MessageInfo MyInfo = new MessageInfo();
            int MaxQuery = oVariables.iMaxMessageToQuery;
            bool bQueryMessage = oVariables.bQueryMessage;

            MSMQTView.ImageList = imageList1;
            oVariables.bbtnRefreshQueue = false;
            oVariables.brefreshQueueToolStripMenuItem = false;
            oVariables.bCancelAllProcess = true;
            TreeNode node2 = MSMQTView.SelectedNode;
            if (node2 != null)
            {
                MSMQTView.Cursor = Cursors.WaitCursor;
                string[] MySelectedMessage = oVariables.SplitByString(MSMQTView.SelectedNode.Tag.ToString(), "|||||");
                StructuredQueue Queues = new StructuredQueue();
                if (node2.FullPath.Contains("Public Queue"))
                {
                    Queues = (StructuredQueue)oVariables.oPublicQueue_ARRAY.GetRange(Convert.ToInt32(MySelectedMessage[1]), 1)[0];
                }
                else if (node2.FullPath.Contains("Private Queue"))
                {
                    Queues = (StructuredQueue)oVariables.oPrivateQueue_ARRAY.GetRange(Convert.ToInt32(MySelectedMessage[1]), 1)[0];
                }

                //StructuredQueue oQueue = new StructuredQueue();
                //oQueue = Queues;
                oVariables.iProgressValue = Queues.iQueueId;
                Application.DoEvents();
                oVariables.sStatusMessage = "Quering " + Queues.sPath + " Please wait...";
                Thread newThread = new Thread(delegate()
                {
                    Application.DoEvents();
                    if (bQueryMessage)
                        MyInfo = oMSMQ.PeekAllMessage(Queues, Queues.iQueueId, MaxQuery);
                    else
                        MyInfo = oMSMQ.GetAllMessage(Queues, Queues.iQueueId, MaxQuery);
                });
                newThread.Start();
                while (newThread.IsAlive)
                {
                    Application.DoEvents();
                    if (oVariables.bCancelAllProcessFlag)
                    {
                        oVariables.bCancelAllProcessFlag = false;
                        newThread.Abort();
                    }
                }
                Queues.CountMessage = MyInfo.ID;

                Queues.Message = MyInfo.Message;

                if (node2.FullPath.Contains("Public Queue"))
                {
                    oVariables.oPublicQueue_ARRAY.RemoveAt(Convert.ToInt32(MySelectedMessage[1]));
                    oVariables.oPublicQueue_ARRAY.Insert(Convert.ToInt32(MySelectedMessage[1]), Queues);
                }
                else if (node2.FullPath.Contains("Private Queue"))
                {
                    oVariables.oPrivateQueue_ARRAY.RemoveAt(Convert.ToInt32(MySelectedMessage[1]));
                    oVariables.oPrivateQueue_ARRAY.Insert(Convert.ToInt32(MySelectedMessage[1]), Queues);
                }



                node2.Text = Queues.sQueueName + "(" + MyInfo.ID + ")";
                node2.Tag = "Queues|||||" + Queues.iQueueId + "|||||" + Queues.bTransactional.ToString() + "|||||" + Queues.CountMessage;
                // MSMQTView.BeginUpdate();
                //TreeNode myNode = new TreeNode();
                node2.Nodes.Clear();
                if (MyInfo.Message != null)
                {
                    progressBar1.Maximum = MyInfo.Message.Count + 1;
                    MSMQTView.BeginUpdate();
                    foreach (QueueInfos Messages in MyInfo.Message)
                    {
                        Application.DoEvents();
                        oVariables.sStatusMessage = "Loading " + Messages.ID + " to " + Queues.sQueueName + " Please wait...";
                        if (progressBar1.Maximum > 0)
                        {
                            if (progressBar1.Maximum > progressBar1.Value)
                                progressBar1.Value += 1;
                            //node2.Text = oQueues.sQueueName + "(" + progressBar1.Value + ")";
                        }
                        if (oVariables.bCancelAllProcessFlag)
                        {
                            progressBar1.Value = Queues.Message.Count;
                            MSMQTView.EndUpdate();
                            MSMQTView.Cursor = Cursors.Default;
                            oVariables.bCancelAllProcessFlag = false;
                            break;
                        }
                        TreeNode mNode = new TreeNode();
                        if (Messages.Transact == 2)
                            mNode.ImageIndex = 1;
                        else if (Messages.Transact == 3)
                            mNode.ImageIndex = 3;
                        else if (Messages.Transact == 1)
                            mNode.ImageIndex = 5;
                        mNode.Tag = "Message|||||" + Messages.MessageID + "|||||" + Queues.iQueueId + "|||||" + Queues.bTransactional.ToString();
                        mNode.Name = Messages.ID;
                        mNode.Text = Messages.Label;

                        //myNode.Nodes.Add(mNode);
                        node2.Nodes.Add(mNode);
                        Application.DoEvents();
                        //MSMQTView.EndUpdate();
                    }
                    oVariables.sStatusMessage = "Queue " + Queues.sQueueName + " has already updated.. There are " + MyInfo.Message.Count + " message queried";
                    MSMQTView.EndUpdate();
                }
                MSMQTView.Cursor = Cursors.Default;


            }
            oVariables.bbtnRefreshQueue = true;
            oVariables.brefreshQueueToolStripMenuItem = true;
            oVariables.bCancelAllProcess = false;

        }

        private void Update_DataGridMessage(StructuredQueue ArrayMessage)
        {

            DgViewMessage.BringToFront();
            rdoBytable.Checked = true;
            Object[] cellValues = null;
            DgViewMessage.Rows.Clear();
            foreach (QueueInfos Messages in ArrayMessage.Message)
            {
                Application.DoEvents();
                cellValues = new Object[]
                {
                    
                    (Convert.ToInt32(Messages.MessageID) + 1) ,
                    Messages.Label,
                    Messages.Body,
                    Messages.Priority,
                    Messages.ID,
                    Messages.SentTime
                };
                DgViewMessage.Rows.Add(cellValues);


            }
        }

        private void DgViewMessage_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DgViewMessage.SendToBack();
                // txtBodyWeb.Visible = true;
                //txtBody.Visible = true;
                //  rdoBytext.Checked = true;
                QueueInfos MyMessage = new QueueInfos();
                MyMessage.Label = DgViewMessage.Rows[e.RowIndex].Cells["Label"].Value.ToString();
                MyMessage.Body = DgViewMessage.Rows[e.RowIndex].Cells["Body"].Value.ToString();
                MyMessage.Priority = DgViewMessage.Rows[e.RowIndex].Cells["Priority"].Value.ToString();
                MyMessage.Queue = txtQueueName.Text;
                SetQueueInfos(MyMessage);
            }
            catch { }
        }
        private void DgViewMessage_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    DgViewMessage.SendToBack();
            //    // txtBodyWeb.Visible = true;
            //    //txtBody.Visible = true;
            //    //  rdoBytext.Checked = true;
            //    QueueInfos MyMessage = new QueueInfos();
            //    MyMessage.Label = DgViewMessage.Rows[e.RowIndex].Cells["Label"].Value.ToString();
            //    MyMessage.Body = DgViewMessage.Rows[e.RowIndex].Cells["Body"].Value.ToString();
            //    MyMessage.Priority = DgViewMessage.Rows[e.RowIndex].Cells["Priority"].Value.ToString();
            //    MyMessage.Queue = txtQueueName.Text;
            //    SetQueueInfos(MyMessage);
            //}
            //catch { }
        }
     
        private TreeNode m_OldSelectNode;
        private void MSMQTView_MouseUp(object sender, MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = MSMQTView.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    m_OldSelectNode = MSMQTView.SelectedNode;
                    MSMQTView.SelectedNode = node;

                    // Find the appropriate ContextMenu depending on the selected node.
                    contextMenuStripTreeview.Items.Clear();
                    if (node.Tag.ToString() == "Queuetype")
                    {
                        contextMenuStripTreeview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                        this.newQueueToolStripMenuItem,
                        this.purgeAllQueueToolStripMenuItem,
                         this.deleteAllQueueToolStripMenuItem});
                        contextMenuStripTreeview.Show(MSMQTView, p);
                        return;
                    } if (node.Tag.ToString().Contains("Queues|||||"))
                    {
                        string[] MySelectedMessage = oVariables.SplitByString(node.Tag.ToString(), "|||||");
                        if (Convert.ToInt32(MySelectedMessage[3]) > 0)
                            oVariables.bpurgeQueueToolStripMenuItem = true;
                        else
                            oVariables.bpurgeQueueToolStripMenuItem = false;

                        contextMenuStripTreeview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                        this.toolStripMenuItemRefresqueue,
                        this.deleteQueueToolStripMenuItem,
                        this.purgeQueueToolStripMenuItem});


                        contextMenuStripTreeview.Show(MSMQTView, p);
                        return;
                    }
                    if (node.Tag.ToString().Contains("Message|||||"))
                    {
                        contextMenuStripTreeview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                        this.deletemessageStripMenuItem,
                        this.purgeQueueToolStripMenuItem});
                        contextMenuStripTreeview.Show(MSMQTView, p);
                        return;
                    }

                }
                else
                {

                    // Highlight the selected node.
                    MSMQTView.SelectedNode = m_OldSelectNode;
                    m_OldSelectNode = null;
                }

            }
        }

        private void MSMQTView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.F5))
            {
                UpdateTreeViewbySpecificQueuename();
            }
        }
        #endregion

        #region Timers
        private void timer1_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (oVariables.bFlagProgressBar)
            {
                if (oVariables.iProgressMax > 0)
                {

                    progressBar1.Maximum = oVariables.iProgressMax + 1;
                    progressBar1.Value = oVariables.iProgressValue;
                }
                else
                {
                    oVariables.iProgressValue = 0;
                }
            }
            //LabelStatusBar.Text = oVariables.sStatusMessage.ToString();
            txtStatus.Text = oVariables.sStatusMessage.ToString();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            try
            {
               

                if ((oVariables.bAutoRefreshQueue) && oVariables.bRefreshOnly)
                {
                    oVariables.bRefreshOnly = false;
                    Refresh_MSMQ();
                }

                LabelDateTime.Text = DateTime.Now.ToString();

                if (oVariables.bCallRefresh)
                {
                    Refresh_MSMQ();
                }
                if (oVariables.IsTransactional)
                {
                    comboPriority.BackColor = SystemColors.Menu;

                    comboPriority.Enabled = false;
                }
                else
                {
                    comboPriority.Enabled = true;
                    comboPriority.BackColor = SystemColors.Window;



                }

                if (oVariables.bconnecToAnotherComputerToolStripMenuItem)
                    connecToAnotherComputerToolStripMenuItem.Enabled = true;
                else
                    connecToAnotherComputerToolStripMenuItem.Enabled = false;

                if (oVariables.brecentComputersToolStripMenuItem)
                    recentComputersToolStripMenuItem.Enabled = true;
                else
                    recentComputersToolStripMenuItem.Enabled = false;
                if (oVariables.bexitToolStripMenuItem)
                    exitToolStripMenuItem.Enabled = true;
                else
                    exitToolStripMenuItem.Enabled = false;
                if (oVariables.bloadmessageStripMenuItem)
                    loadmessageStripMenuItem.Enabled = true;
                else
                    loadmessageStripMenuItem.Enabled = false;
                if (oVariables.bsavemessageStripMenuItem)
                    savemessageStripMenuItem.Enabled = true;
                else
                    savemessageStripMenuItem.Enabled = false;
                if (oVariables.bqueueToolStripMenuItem)
                    queueToolStripMenuItem.Enabled = true;
                else
                    queueToolStripMenuItem.Enabled = false;
                if ((oVariables.bnewQueueToolStripMenuItem) && (!(oVariables.IsRemote)))
                    newQueueToolStripMenuItem.Enabled = true;
                else
                    newQueueToolStripMenuItem.Enabled = false;
                if ((oVariables.bdeleteQueueToolStripMenuItem) && (!(oVariables.IsRemote)))
                    deleteQueueToolStripMenuItem.Enabled = true;
                else
                    deleteQueueToolStripMenuItem.Enabled = false;
                if (oVariables.bmessageToolStripMenuItem)
                    EditToolStripMenuItem.Enabled = true;
                else
                    EditToolStripMenuItem.Enabled = false;
                if (oVariables.bpurgeQueueToolStripMenuItem)
                    purgeQueueToolStripMenuItem.Enabled = true;
                else
                    purgeQueueToolStripMenuItem.Enabled = false;
                if (oVariables.brefreshQueueToolStripMenuItem)
                    refreshQueueToolStripMenuItem.Enabled = true;
                else
                    refreshQueueToolStripMenuItem.Enabled = false;
                if (oVariables.brefreshQueueToolStripMenuItem)
                {
                    refreshQueueToolStripMenuItem.Enabled = true;
                    RefreshQueuetoolStripButton.Enabled = true;
                }
                else
                {
                    refreshQueueToolStripMenuItem.Enabled = false;
                    RefreshQueuetoolStripButton.Enabled = false;
                }



                if (oVariables.bpurgeAllQueueToolStripMenuItem)
                    purgeAllQueueToolStripMenuItem.Enabled = true;
                else
                    purgeAllQueueToolStripMenuItem.Enabled = false;
                if ((oVariables.bdeleteAllQueueToolStripMenuItem) && (!(oVariables.IsRemote)))
                    deleteAllQueueToolStripMenuItem.Enabled = true;
                else
                    deleteAllQueueToolStripMenuItem.Enabled = false;

                if (oVariables.bcutToolStripMenuItem)
                    cutToolStripMenuItem.Enabled = true;
                else
                    cutToolStripMenuItem.Enabled = false;
                if (oVariables.bcopyToolStripMenuItem)
                    copyToolStripMenuItem.Enabled = true;
                else
                    copyToolStripMenuItem.Enabled = false;
                if (oVariables.bcutToolStripMenuItem)
                    cutToolStripMenuItem.Enabled = true;
                else
                    cutToolStripMenuItem.Enabled = false;
                if (oVariables.bselectAllToolStripMenuItem)
                    selectAllToolStripMenuItem.Enabled = true;
                else
                    selectAllToolStripMenuItem.Enabled = false;
                if (oVariables.bclearUndoRedoToolStripMenuItem)
                    clearUndoRedoToolStripMenuItem.Enabled = true;
                else
                    clearUndoRedoToolStripMenuItem.Enabled = false;
                //if (oVariables.bsettingsToolStripMenuItem)
                //    settingsToolStripMenuItem.Enabled = true;
                //else
                //    settingsToolStripMenuItem.Enabled = false;
                //if (oVariables.bhelpToolStripMenuItem)
                //    helpToolStripMenuItem.Enabled = true;
                //else
                //    helpToolStripMenuItem.Enabled = false;
                //if (oVariables.bvisitMSMQQXplorerWebsiteToolStripMenuItem)
                //    visitMSMQQXplorerWebsiteToolStripMenuItem.Enabled = true;
                //else
                //    visitMSMQQXplorerWebsiteToolStripMenuItem.Enabled = false;
                //if (oVariables.baboutToolStripMenuItem)
                //    aboutToolStripMenuItem.Enabled = true;
                //else
                //    aboutToolStripMenuItem.Enabled = false;


                if ((oVariables.bCreateQueueStripButton) && (!(oVariables.IsRemote)))
                    CreateQueueStripButton.Enabled = true;
                else
                    CreateQueueStripButton.Enabled = false;

                if (oVariables.bPastetoolStripButton)
                {
                    pasteToolStripMenuItem.Enabled = true;
                    PastetoolStripButton.Enabled = true;
                }
                else
                {
                    pasteToolStripMenuItem.Enabled = false;
                    PastetoolStripButton.Enabled = false;
                }
                if ((oVariables.bDeleteQueuetoolStripButton) && (!(oVariables.IsRemote)))
                    DeleteQueuetoolStripButton.Enabled = true;
                else
                    DeleteQueuetoolStripButton.Enabled = false;
                if (oVariables.bPurgeQueuetoolStripButton)
                    PurgeQueuetoolStripButton.Enabled = true;
                else
                    PurgeQueuetoolStripButton.Enabled = false;
                if (oVariables.bSendMessagetoolStripButton)
                    SendMessagetoolStripButton.Enabled = true;
                else
                    SendMessagetoolStripButton.Enabled = false;
                if (oVariables.bDeleteMessagetoolStripButton)
                    DeleteMessagetoolStripButton.Enabled = true;
                else
                    DeleteMessagetoolStripButton.Enabled = false;

                if (oVariables.oUndoList_ARRAY.Count > 0)
                {
                    UndotoolStripButton.Enabled = true;
                    undoToolStripMenuItem.Enabled = true;
                }
                else
                {
                    UndotoolStripButton.Enabled = false;
                    undoToolStripMenuItem.Enabled = false;
                }

                if (oVariables.oRedoList_ARRAY.Count > 0)
                {
                    RedotoolStripButton.Enabled = true;
                    redoToolStripMenuItem.Enabled = true;
                }
                else
                {
                    RedotoolStripButton.Enabled = false;
                    redoToolStripMenuItem.Enabled = false;
                }
                if ((oVariables.oRedoList_ARRAY.Count > 0) || (oVariables.oUndoList_ARRAY.Count > 0))
                    clearUndoRedoToolStripMenuItem.Enabled = true;
                else
                    clearUndoRedoToolStripMenuItem.Enabled = false;


                if (oVariables.bCuttoolStripButton)
                    CuttoolStripButton.Enabled = true;
                else
                    CuttoolStripButton.Enabled = false;
                if (oVariables.bCopytoolStripButton)
                    CopytoolStripButton.Enabled = true;
                else
                    CopytoolStripButton.Enabled = false;
                if (oVariables.bsendmessageStripMenuItem)
                    sendmessageStripMenuItem.Enabled = true;
                else
                    sendmessageStripMenuItem.Enabled = false;
                if (oVariables.bdeletemessageStripMenuItem)
                    deletemessageStripMenuItem.Enabled = true;
                else
                    deletemessageStripMenuItem.Enabled = false;
                if (oVariables.bbtnRefreshQueue)
                    btnRefreshQueue.Enabled = true;
                else
                    btnRefreshQueue.Enabled = false;
                if (oVariables.bbtnSendMessage)
                    btnSendMessage.Enabled = true;
                else
                    btnSendMessage.Enabled = false;
                if (oVariables.bbtnPurgeQueue)
                    btnPurgeQueue.Enabled = true;
                else
                    btnPurgeQueue.Enabled = false;
                if (oVariables.bCancelAllProcess)
                    toolStripMenuItemCancel.Enabled = true;
                else
                    toolStripMenuItemCancel.Enabled = false;

                Control ctrl = this.ActiveControl;
                if (ctrl != null)
                {
                    if (ctrl is TextBox)
                    {
                        TextBox tx = (TextBox)ctrl;

                        if (tx.SelectedText.Length == 0)
                        {
                            oVariables.bCopytoolStripButton = false;
                            oVariables.bcopyToolStripMenuItem = false;
                            oVariables.bCuttoolStripButton = false;
                            oVariables.bcutToolStripMenuItem = false;
                        }
                        else
                        {

                            oVariables.bCopytoolStripButton = true;
                            oVariables.bcopyToolStripMenuItem = true;
                            oVariables.bCuttoolStripButton = true;
                            oVariables.bcutToolStripMenuItem = true;
                        }
                        if (tx.SelectedText.Length != tx.Text.Length)
                        {
                            oVariables.bselectAllToolStripMenuItem = true;

                        }
                        else
                        {
                            oVariables.bselectAllToolStripMenuItem = false;
                        }

                    }
                    else if (ctrl is ComboBox)
                    {
                        ComboBox tx = (ComboBox)ctrl;
                        if (tx.SelectionLength == 0)
                        {
                            oVariables.bCopytoolStripButton = false;
                            oVariables.bcopyToolStripMenuItem = false;
                            oVariables.bCuttoolStripButton = false;
                            oVariables.bcutToolStripMenuItem = false;
                        }
                        else
                        {
                            oVariables.bCopytoolStripButton = true;
                            oVariables.bcopyToolStripMenuItem = true;
                            oVariables.bCuttoolStripButton = true;
                            oVariables.bcutToolStripMenuItem = true;
                        }
                        if (tx.SelectionLength != tx.Text.Length)
                        {
                            oVariables.bselectAllToolStripMenuItem = true;

                        }
                        else
                        {
                            oVariables.bselectAllToolStripMenuItem = false;
                        }


                    }
                    else
                    {
                        oVariables.bCopytoolStripButton = false;
                        oVariables.bcopyToolStripMenuItem = false;
                        oVariables.bCuttoolStripButton = false;
                        oVariables.bcutToolStripMenuItem = false;
                        if (Clipboard.GetText().Length==0)
                        {
                            oVariables.bpasteToolStripMenuItem = false;
                            oVariables.bPastetoolStripButton = false;
                        }
                    }



                }

            }
            catch { }

        }
        #endregion

        #region Tool Strip button
        private void CreateQueueStripButton_Click(object sender, EventArgs e)
        {
            Form frmCreateQueue = new frmCreateQueue(ref oVariables);
            frmCreateQueue.ShowDialog();
        }

        private void DeleteQueuetoolStripButton_Click(object sender, EventArgs e)
        {
            DeleteQueue();
        }

        private void PurgeQueuetoolStripButton_Click(object sender, EventArgs e)
        {
            PurgeQueue();
        }

        private void SendMessagetoolStripButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void DeleteMessagetoolStripButton_Click(object sender, EventArgs e)
        {
            DeleteMessage();
        }

        private void UndotoolStripButton_Click(object sender, EventArgs e)
        {
            cmdUndo();
        }

        private void RedotoolStripButton_Click(object sender, EventArgs e)
        {
            cmdRedo();
        }

        private void CuttoolStripButton_Click(object sender, EventArgs e)
        {
            CallCut();
        }

        private void CopytoolStripButton_Click(object sender, EventArgs e)
        {
            CallCopy();
        }

        private void PastetoolStripButton_Click(object sender, EventArgs e)
        {
            CallPaste();
        }


        #endregion

        #region File Menu
        private void connecToAnotherComputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmConnectCpmputer = new frmConnectComputer(ref oVariables);
            frmConnectCpmputer.ShowDialog();

        }

        private void recentComputersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void savemessageStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadmessageStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Edit Menu
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdRedo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            CallCut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallCopy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallPaste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CallSelectAll();
        }

        private void clearUndoRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            oVariables.oUndoList_ARRAY.Clear();
            oVariables.oRedoList_ARRAY.Clear();
        }

        private void CallCopy()
        {
            Control ctrl = this.ActiveControl;
            if (ctrl != null)
            {
                if (ctrl is TextBox)
                {
                    TextBox tx = (TextBox)ctrl;
                    tx.Copy();
                }
                if (ctrl is ComboBox)
                {
                    ComboBox cb = (ComboBox)ctrl;
                    if (cb.SelectedText != string.Empty)
                        Clipboard.SetText(cb.SelectedText);
                }
            }
            oVariables.bPastetoolStripButton = true;
            ClearClipboardtoolStripMenuItem.Enabled = true;

        }
        private void CallPaste()
        {
            Control ctrl = this.ActiveControl;
            if (ctrl != null)
            {
                if (ctrl is TextBox)
                {
                    TextBox tx = (TextBox)ctrl;
                    tx.Paste();
                }
                if (ctrl is ComboBox)
                {
                    ComboBox cb = (ComboBox)ctrl;
                    string txtInClip = Clipboard.GetText();
                    int sPos = cb.SelectionStart;
                    if (cb.SelectedText != string.Empty)
                    {
                        cb.SelectedText = cb.SelectedText.Replace(cb.SelectedText, txtInClip);
                    }
                    else
                    {
                        cb.Text = cb.Text.Insert(cb.SelectionStart, txtInClip);
                        cb.SelectionStart = sPos + txtInClip.Length;

                    }
                }
            }
        }
        private void CallCut()
        {
            Control ctrl = this.ActiveControl;
            if (ctrl != null)
            {
                if (ctrl is TextBox)
                {
                    TextBox tx = (TextBox)ctrl;
                    tx.Cut();
                }
                if (ctrl is ComboBox)
                {
                    ComboBox cb = (ComboBox)ctrl;
                    string txtInClip = Clipboard.GetText();
                    string copied = cb.SelectedText;
                    int sPos = cb.SelectionStart;
                    cb.SelectedText = cb.SelectedText.Replace(copied, "");
                    cb.SelectionStart = sPos;
                    Clipboard.SetText(copied);
                }
            }
            oVariables.bPastetoolStripButton = true;
            ClearClipboardtoolStripMenuItem.Enabled = true;

        }
        private void CallSelectAll()
        {
            Control ctrl = this.ActiveControl;
            if (ctrl != null)
            {
                if (ctrl is TextBox)
                {
                    TextBox tx = (TextBox)ctrl;
                    tx.SelectAll();
                }
                if (ctrl is ComboBox)
                {
                    ComboBox cb = (ComboBox)ctrl;
                    cb.SelectAll();
                }

            }
        }
        private void cmdUndo()
        {
            //oVariables.oRedoList_ARRAY.Add(txtBody.Text.ToString());
            //txtBody.Text = oVariables.oUndoList_ARRAY[oVariables.oUndoList_ARRAY.Count - 1].ToString();
            //oVariables.oUndoList_ARRAY.RemoveAt(oVariables.oUndoList_ARRAY.Count - 1);
            try
            {
                if (oVariables.oUndoPressed == false)
                {

                    oVariables.oUndoPressed = true;
                    if (oVariables.oUndoList_ARRAY.Count > 1)
                    {
                        int Selection = 0;
                        int SelectedText = 0;
                        Selection = txtBody.SelectionStart;
                        SelectedText = txtBody.SelectionLength;

                        //if (!(string.IsNullOrEmpty(txtBody.Text)))
                        oVariables.oRedoList_ARRAY.Add(txtBody.Text);
                        txtBody.Text = oVariables.oUndoList_ARRAY[oVariables.oUndoList_ARRAY.Count - 1].ToString();
                        if (txtBody.Text.Length >= Selection)
                        {
                            txtBody.SelectionStart = Selection;
                            txtBody.Select(Selection, SelectedText);
                        }
                        else
                        {
                            txtBody.SelectionStart = Selection;
                        }
                        txtBody.ScrollToCaret();
                        oVariables.oUndoList_ARRAY.RemoveAt(oVariables.oUndoList_ARRAY.Count - 1);
                    }
                    else if (oVariables.oUndoList_ARRAY.Count == 1)
                    {
                        //if (!(string.IsNullOrEmpty(txtBody.Text)))
                        oVariables.oRedoList_ARRAY.Add(txtBody.Text);
                        txtBody.Text = oVariables.oUndoList_ARRAY[0].ToString();
                        oVariables.oUndoList_ARRAY.Clear();
                        if (!(string.IsNullOrEmpty(txtBody.Text)))
                            oVariables.oRedoList_ARRAY.Add(txtBody.Text);


                    }
                    else
                    {
                        oVariables.oUndoList_ARRAY.Clear();
                    }
                }

            }
            catch { }
            if (oVariables.oUndoPressed == true)
            {
                oVariables.oUndoPressed = false;
            }
        }
        private void cmdRedo()
        {
            //oVariables.oUndoList_ARRAY.Add(txtBody.Text.ToString());
            //txtBody.Text = oVariables.oRedoList_ARRAY[oVariables.oUndoList_ARRAY.Count - 1].ToString();
            //oVariables.oRedoList_ARRAY.RemoveAt(oVariables.oUndoList_ARRAY.Count - 1);
            try
            {
                if (oVariables.oRedoPressed == false)
                {
                    oVariables.oRedoPressed = true;
                    if (oVariables.oRedoList_ARRAY.Count > 1)
                    {
                        int Selection = 0;
                        int SelectedText = 0;
                        Selection = txtBody.SelectionStart;
                        SelectedText = txtBody.SelectionLength;
                        Selection = txtBody.SelectionStart;
                        // if (!(string.IsNullOrEmpty(txtBody.Text)))
                        oVariables.oUndoList_ARRAY.Add(txtBody.Text);

                        txtBody.Text = oVariables.oRedoList_ARRAY[oVariables.oRedoList_ARRAY.Count - 1].ToString();
                        if (txtBody.Text.Length >= Selection)
                        {
                            txtBody.SelectionStart = Selection;
                            txtBody.Select(Selection, SelectedText);
                        }
                        else
                        {
                            txtBody.SelectionStart = Selection;
                        }
                        txtBody.ScrollToCaret();
                        oVariables.oRedoList_ARRAY.RemoveAt(oVariables.oRedoList_ARRAY.Count - 1);
                    }
                    else if (oVariables.oRedoList_ARRAY.Count == 1)
                    {
                        //if (!(string.IsNullOrEmpty(txtBody.Text)))
                        oVariables.oUndoList_ARRAY.Add(txtBody.Text);
                        txtBody.Text = oVariables.oRedoList_ARRAY[0].ToString();


                        oVariables.oRedoList_ARRAY.Clear();
                        if (!(string.IsNullOrEmpty(txtBody.Text)))
                            oVariables.oUndoList_ARRAY.Add(txtBody.Text);

                    }
                    else
                    {
                        oVariables.oRedoList_ARRAY.Clear();
                    }
                }

            }
            catch { }
            if (oVariables.oRedoPressed)
            {
                oVariables.oRedoPressed = false;
            }
        }

        private void ClearClipboardtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();

            ClearClipboardtoolStripMenuItem.Enabled = false;
            oVariables.bPastetoolStripButton = false;
        }

        #endregion

        #region Message Menu
        private void queueToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.queueToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newQueueToolStripMenuItem,
            this.deleteQueueToolStripMenuItem,
            this.purgeQueueToolStripMenuItem,
            this.refreshQueueToolStripMenuItem,
            this.toolStripSeparator14,
            this.sendmessageStripMenuItem,
            this.deletemessageStripMenuItem,
            this.ResendtoolStripMenuItem,
            this.toolStripSeparator2,
            this.purgeAllQueueToolStripMenuItem,
            this.deleteAllQueueToolStripMenuItem,
            this.toolStripSeparator15,
            this.toolStripMenuItemCancel});
        }

        private void newQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmCreateQueue = new frmCreateQueue(ref oVariables);
            frmCreateQueue.ShowDialog();
        }

        private void deleteQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteQueue();
        }

        private void purgeQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurgeQueue();
        }

        private void refreshQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh_MSMQ();
        }

        private void RefreshQueuetoolStripButton_Click(object sender, EventArgs e)
        {
            Refresh_MSMQ();
        }

        private void sendmessageStripMenuItem_Click(object sender, EventArgs e)
        {

            SendMessage();
        }

        private void deletemessageStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMessage();
        }

        private void ResendtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResendtoolStripMenuItem.Enabled = false;
            ResendMessage();

        }
        private void purgeAllQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurgeAllQueue();
        }

        private void deleteAllQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAllQueue();
        }

        private void toolStripMenuItemCancel_Click(object sender, EventArgs e)
        {
            oVariables.bCancelAllProcessFlag = true;
        }
        #endregion

        #region Select Tools Menu
        private void msmqqxplorerStripMenuItem_Click(object sender, EventArgs e)
        {
            filehashgroupbox.Visible = false;
            MyWebGroupBox.Visible = false;
            groupboxSearch_Text.Visible = false;
            msmqgroupbox.Visible = true;
            MyBrowser.Stop();
            //msmqgroupbox.BringToFront();
            //filehashgroupbox.SendToBack();
            //MyWebGroupBox.SendToBack();

        }

        private void filehashStripMenuItem_Click(object sender, EventArgs e)
        {
            msmqgroupbox.Visible = false;
            MyWebGroupBox.Visible = false;
            groupboxSearch_Text.Visible = false;
            filehashgroupbox.Visible = true;

            MyBrowser.Stop();
            //filehashgroupbox.BringToFront();
            //msmqgroupbox.SendToBack();
            //MyWebGroupBox.SendToBack();

        }
        private void SearchtextStripMenuItem_Click(object sender, EventArgs e)
        {
            msmqgroupbox.Visible = false;
            MyWebGroupBox.Visible = false;
            filehashgroupbox.Visible = false;
            groupboxSearch_Text.Visible = true;


        }
        private void settingeStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmSettings = new frmSettings(ref oVariables);
            frmSettings.ShowDialog();

        }
        #endregion

        #region Help
        private void visitMSMQQXplorerWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyWebGroupBox.Visible = true;
            filehashgroupbox.Visible = false;
            msmqgroupbox.Visible = false;
            groupboxSearch_Text.Visible = false;



            //            MyWebGroupBox.BringToFront();
            //msmqgroupbox.SendToBack();
            //filehashgroupbox.SendToBack();

            MyBrowser.Navigate("http://msmqqxplorer.blogspot.com");
            if (!(ComboNavigator.Items.Contains("http://msmqqxplorer.blogspot.com")))
                ComboNavigator.Items.Add("http://msmqqxplorer.blogspot.com");
            ComboNavigator.Text = "http://msmqqxplorer.blogspot.com";
            oVariables.sStatusMessage = MyBrowser.StatusText.ToString();
            ComboNavigator.Enabled = false;

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmAbout = new frmAboutBox(ref oVariables);
            frmAbout.ShowDialog();
            if (oVariables.bClickDonate)
            {
                oVariables.bClickDonate = false;
                MyWebGroupBox.Visible = true;
                filehashgroupbox.Visible = false;
                msmqgroupbox.Visible = false;
                groupboxSearch_Text.Visible = false;
                MyBrowser.Navigate("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=juliusLramos%40yahoo%2ecom%2eph&item_name=MSMQ%20QXplorer&no_shipping=0&no_note=1&tax=0&currency_code=USD&lc=PH&bn=PP%2dDonationsBF&charset=UTF%2d8");
                if (!(ComboNavigator.Items.Contains("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=juliusLramos%40yahoo%2ecom%2eph&item_name=MSMQ%20QXplorer&no_shipping=0&no_note=1&tax=0&currency_code=USD&lc=PH&bn=PP%2dDonationsBF&charset=UTF%2d8")))
                    ComboNavigator.Items.Add("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=juliusLramos%40yahoo%2ecom%2eph&item_name=MSMQ%20QXplorer&no_shipping=0&no_note=1&tax=0&currency_code=USD&lc=PH&bn=PP%2dDonationsBF&charset=UTF%2d8");
                ComboNavigator.Text = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=juliusLramos%40yahoo%2ecom%2eph&item_name=MSMQ%20QXplorer&no_shipping=0&no_note=1&tax=0&currency_code=USD&lc=PH&bn=PP%2dDonationsBF&charset=UTF%2d8";
                oVariables.sStatusMessage = MyBrowser.StatusText.ToString();
                ComboNavigator.Enabled = false;
            }
        }
        #endregion

        #region Other Options

        private void chkPrivateQueues_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPrivateQueues.Checked)
                oVariables.bIsPrivate = true;
            else
                oVariables.bIsPrivate = false;
        }

        private void chkOutgoingQueues_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPrivateQueues.Checked)
                oVariables.bIsOutgoing = true;
            else
                oVariables.bIsOutgoing = false;
        }

        private void chkPublicQueues_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPublicQueues.Checked)
                oVariables.bIsPublic = true;
            else
                oVariables.bIsPublic = false;
        }

        private void chkSystemQueues_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSystemQueues.Checked)
                oVariables.bIsSystem = true;
            else
                oVariables.bIsSystem = false;
        }

        private void rdoByXML_CheckedChanged(object sender, EventArgs e)
        {

            if (rdoByXML.Checked)
            {
                txtBodyWeb.BringToFront();
                oVariables.bLastRdo = false;
                txtBody.SendToBack();
                using (StreamWriter myStream = new StreamWriter(oVariables.sTempFolder + "\\messagebody.tmp"))
                {
                    if (!(String.IsNullOrEmpty(oVariables.sCurrentMessageBody)))
                        myStream.Write(oVariables.sCurrentMessageBody);
                    else if (!(String.IsNullOrEmpty(txtBody.Text.ToString())))
                        myStream.Write(txtBody.Text.ToString());
                    else
                        myStream.Write("Message Body is Empty");
                    myStream.Close();
                }
                oVariables.sWebUrl = oVariables.sTempFolder + "\\messagebody.tmp";
                txtBodyWeb.Navigate(oVariables.sWebUrl);

            }

        }

        private void rdoBytext_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoBytext.Checked)
            {
                txtBody.BringToFront();
                txtBodyWeb.SendToBack();
                oVariables.bLastRdo = true;
                try
                {

                    FileStream file = new FileStream(oVariables.sWebUrl, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(file);
                    txtBody.Text = sr.ReadToEnd();
                    sr.Close();
                    file.Close();
                }
                catch
                { }
            }

            //txtBody.Visible = true;
        }

        private void rdoBytable_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoBytable.Checked)
                DgViewMessage.BringToFront();
            else
            {
                DgViewMessage.SendToBack();
                if (rdoBytext.Checked)
                    txtBody.BringToFront();
                else if (rdoByXML.Checked)
                    txtBodyWeb.BringToFront();
            }
        }

        private void ExitApplicationSaveSettings()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Write down the XML declaration
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            // Create the root element
            XmlElement rootNode = xmlDoc.CreateElement("QXplorer");
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            xmlDoc.AppendChild(rootNode);

            // Create a new <Category> element and add it to the root node
            XmlElement CompNode = xmlDoc.CreateElement("ComputerName");
            xmlDoc.DocumentElement.AppendChild(CompNode);
            XmlText CompNodeText = xmlDoc.CreateTextNode(oVariables.sComputerName);
            CompNode.AppendChild(CompNodeText);

            XmlElement PrivateQueueNode = xmlDoc.CreateElement("PrivateQueue");
            xmlDoc.DocumentElement.AppendChild(PrivateQueueNode);
            XmlText PrivateQueueText = xmlDoc.CreateTextNode(chkPrivateQueues.Checked.ToString());
            PrivateQueueNode.AppendChild(PrivateQueueText);

            XmlElement PublicQueueNode = xmlDoc.CreateElement("PublicQueue");
            xmlDoc.DocumentElement.AppendChild(PublicQueueNode);
            XmlText PublicQueueText = xmlDoc.CreateTextNode(chkPublicQueues.Checked.ToString());
            PublicQueueNode.AppendChild(PublicQueueText);

            XmlElement QueryTypeNode = xmlDoc.CreateElement("QueryType");
            xmlDoc.DocumentElement.AppendChild(QueryTypeNode);
            XmlText QueryTypeText = xmlDoc.CreateTextNode(oVariables.bQueryMessage.ToString());
            QueryTypeNode.AppendChild(QueryTypeText);

            XmlElement MaxMessageToQueryNode = xmlDoc.CreateElement("MaxMessageToQuery");
            xmlDoc.DocumentElement.AppendChild(MaxMessageToQueryNode);
            XmlText MaxMessageToQueryText = xmlDoc.CreateTextNode(oVariables.iMaxMessageToQuery.ToString());
            MaxMessageToQueryNode.AppendChild(MaxMessageToQueryText);

            XmlElement EnableQueryNode = xmlDoc.CreateElement("EnableQuery");
            xmlDoc.DocumentElement.AppendChild(EnableQueryNode);
            XmlText EnableQueryText = xmlDoc.CreateTextNode(oVariables.bEnableQuery.ToString());
            EnableQueryNode.AppendChild(EnableQueryText);

            XmlElement SortQueueNode = xmlDoc.CreateElement("SortQueue");
            xmlDoc.DocumentElement.AppendChild(SortQueueNode);
            XmlText SortQueueText = xmlDoc.CreateTextNode(oVariables.bSortQueue.ToString());
            SortQueueNode.AppendChild(SortQueueText);

            XmlElement EnableFilterListNode = xmlDoc.CreateElement("EnableFilterList");
            xmlDoc.DocumentElement.AppendChild(EnableFilterListNode);
            XmlText EnableFilterListText = xmlDoc.CreateTextNode(oVariables.bEnableFilter.ToString());
            EnableFilterListNode.AppendChild(EnableFilterListText);

            XmlElement EnableViewbyTableNode = xmlDoc.CreateElement("EnableViewbyTable");
            xmlDoc.DocumentElement.AppendChild(EnableViewbyTableNode);
            XmlText EnableViewbyTableText = xmlDoc.CreateTextNode(oVariables.bViewByTable.ToString());
            EnableViewbyTableNode.AppendChild(EnableViewbyTableText);

            XmlElement AutoRefreshQueueNode = xmlDoc.CreateElement("AutoRefreshQueue");
            xmlDoc.DocumentElement.AppendChild(AutoRefreshQueueNode);
            XmlText AutoRefreshQueueText = xmlDoc.CreateTextNode(oVariables.bAutoRefreshQueue.ToString());
            AutoRefreshQueueNode.AppendChild(AutoRefreshQueueText);

            XmlElement FilterListNode = xmlDoc.CreateElement("FilterList");
            xmlDoc.DocumentElement.AppendChild(FilterListNode);
            // XmlText FilterListText = xmlDoc.CreateTextNode(oVariables.bEnableFilter.ToString());
            //FilterListNode.AppendChild(FilterListText);

            foreach (string sQueueName in oVariables.oFilterList_ARRAY)
            {
                XmlElement Sub1Node = xmlDoc.CreateElement("QueueName");
                xmlDoc.DocumentElement.AppendChild(FilterListNode).AppendChild(Sub1Node);
                XmlText Sub1NodeText = xmlDoc.CreateTextNode(sQueueName);
                Sub1Node.AppendChild(Sub1NodeText);
            }



            //XmlElement AutorefreshNode = xmlDoc.CreateElement("AutoRefresh");
            //xmlDoc.DocumentElement.AppendChild(AutorefreshNode);
            //XmlText AutorefreshText = xmlDoc.CreateTextNode(cBoxAutoRefresh.Checked.ToString());
            //AutorefreshNode.AppendChild(AutorefreshText);

            //XmlElement AutoColapseNode = xmlDoc.CreateElement("AutoColapse");
            //xmlDoc.DocumentElement.AppendChild(AutoColapseNode);
            //XmlText AutoColapseText = xmlDoc.CreateTextNode(CboxAutoColapse.Checked.ToString());
            //AutoColapseNode.AppendChild(AutoColapseText);

            //XmlElement CheckArrivedNode = xmlDoc.CreateElement("CheckArrived");
            //xmlDoc.DocumentElement.AppendChild(CheckArrivedNode);
            //XmlText CheckArrivedText = xmlDoc.CreateTextNode(objVar.CheckNotify.ToString());
            //CheckArrivedNode.AppendChild(CheckArrivedText);

            //XmlElement AutoCreateQNode = xmlDoc.CreateElement("AutoCreateQ");
            //xmlDoc.DocumentElement.AppendChild(AutoCreateQNode);
            //XmlText AutoCreateQText = xmlDoc.CreateTextNode(objVar.AutoCreateQueue.ToString());
            //AutoCreateQNode.AppendChild(AutoCreateQText);

            //XmlElement ClearMessageBodyNode = xmlDoc.CreateElement("ClearMessageBody");
            //xmlDoc.DocumentElement.AppendChild(ClearMessageBodyNode);
            //XmlText ClearMessageBodyText = xmlDoc.CreateTextNode(objVar.checkmessagebody.ToString());
            //ClearMessageBodyNode.AppendChild(ClearMessageBodyText);

            //XmlElement ClearQueueNameNode = xmlDoc.CreateElement("ClearQueueName");
            //xmlDoc.DocumentElement.AppendChild(ClearQueueNameNode);
            //XmlText ClearQueueNameText = xmlDoc.CreateTextNode(objVar.checkqueuename.ToString());
            //ClearQueueNameNode.AppendChild(ClearQueueNameText);

            //XmlElement XMLFilePathNode = xmlDoc.CreateElement("XMLFilePath");
            //xmlDoc.DocumentElement.AppendChild(XMLFilePathNode);
            //XmlText XMLFilePathText = xmlDoc.CreateTextNode(txtXMLFile.Text);
            //XMLFilePathNode.AppendChild(XMLFilePathText);

            //XmlElement ClipboardLimitNode = xmlDoc.CreateElement("ClipboardLimit");
            //xmlDoc.DocumentElement.AppendChild(ClipboardLimitNode);
            //XmlText ClipboardLimitText = xmlDoc.CreateTextNode(objVar.ClipboardLimit.ToString());
            //ClipboardLimitNode.AppendChild(ClipboardLimitText);

            //XmlElement QueuePasswordNode = xmlDoc.CreateElement("QueuePassword");
            //xmlDoc.DocumentElement.AppendChild(QueuePasswordNode);
            //XmlText QueuePasswordText = xmlDoc.CreateTextNode(objVar.QueuePassword.Trim().ToString());
            //QueuePasswordNode.AppendChild(QueuePasswordText);

            //XmlElement QueueUsernameNode = xmlDoc.CreateElement("QueueUsername");
            //xmlDoc.DocumentElement.AppendChild(QueueUsernameNode);
            //XmlText QueueUsernameText = xmlDoc.CreateTextNode(objVar.QueueUserName.ToString().Trim());
            //QueueUsernameNode.AppendChild(QueueUsernameText);


            //if (txtXMLFile.Text.ToLower().Contains(".xml"))
            //{
            //    XmlElement XMLNode = xmlDoc.CreateElement("XMLFile");
            //    xmlDoc.DocumentElement.AppendChild(XMLNode);
            //    XmlText XMLNodeText = xmlDoc.CreateTextNode(txtXMLFile.Text);
            //    XMLNode.AppendChild(XMLNodeText);
            //}
            //  xmlDoc.Save(oVariables.sTempFolder + "\\QXplorer.xml");
            xmlDoc.Save(oVariables.sQXplorerxml);

            Application.Exit();
        }

        private void LoadApplicationSettings()
        {
            if (File.Exists(oVariables.sQXplorerxml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlNode xmlNode;
                xmlDoc.Load(oVariables.sQXplorerxml);
                // Remove XML Declaration
                if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    xmlDoc.RemoveChild(xmlDoc.FirstChild);
                // Load the start of the XML message
                xmlNode = xmlDoc.FirstChild;

                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.Name.ToString().ToLower().Contains("computername"))
                        oVariables.sComputerName = Convert.ToString(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("privatequeue"))
                    {
                        oVariables.bIsPrivate = Convert.ToBoolean(childNode.InnerText); ;
                        chkPrivateQueues.Checked = Convert.ToBoolean(childNode.InnerText);
                    }
                    else if (childNode.Name.ToString().ToLower().Contains("publicqueue"))
                    {
                        oVariables.bIsPublic = Convert.ToBoolean(childNode.InnerText);
                        chkPublicQueues.Checked = Convert.ToBoolean(childNode.InnerText);
                    }
                    else if (childNode.Name.ToString().ToLower().Contains("maxmessagetoquery"))
                        oVariables.iMaxMessageToQuery = Convert.ToInt32(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("enablequery"))
                        oVariables.bEnableQuery = Convert.ToBoolean(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("sortqueue"))
                        oVariables.bSortQueue = Convert.ToBoolean(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("querytype"))
                        oVariables.bQueryMessage = Convert.ToBoolean(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("enableviewbytable"))
                        oVariables.bViewByTable = Convert.ToBoolean(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("enablefilterlist"))
                        oVariables.bEnableFilter = Convert.ToBoolean(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("autorefreshqueue"))
                        oVariables.bAutoRefreshQueue = Convert.ToBoolean(childNode.InnerText);
                    else if (childNode.Name.ToString().ToLower().Contains("filterlist"))
                    {
                        foreach (XmlNode FirstSubchildNode in childNode)
                        {
                            if (FirstSubchildNode.Name.ToString().ToLower().Contains("queuename"))
                                oVariables.oFilterList_ARRAY.Add(FirstSubchildNode.InnerText);
                        }
                    }
                }
                //else if (childNode.Name.ToString().ToLower().Contains("clearqueuename"))
                //    objVar.checkqueuename = Convert.ToBoolean(childNode.InnerText);
                //else if (childNode.Name.ToString().ToLower().Contains("xmlfilepath"))
                //    txtXMLFile.Text = childNode.InnerText.ToString();
                //else if (childNode.Name.ToString().ToLower().Contains("clipboardlimit"))
                //    objVar.ClipboardLimit = Convert.ToInt32(childNode.InnerText);
                //else if (childNode.Name.ToString().ToLower().Contains("queuepassword"))
                //{
                //    objVar.QueuePassword = childNode.InnerText;
                //    MSMQ.QueuePassword= childNode.InnerText;
                //}
                //else if (childNode.Name.ToString().ToLower().Contains("queueusername"))
                //{
                //    objVar.QueueUserName = childNode.InnerText;
                //MSMQ.QueueUserName =childNode.InnerText;
                //}
            }
        }

        private void lblAdd_Click(object sender, EventArgs e)
        {
            lblAdd.BorderStyle = BorderStyle.Fixed3D;
            if (!(oVariables.oFilterList_ARRAY.Contains(ComboFilterList.Text.ToString())) && (!String.IsNullOrEmpty(ComboFilterList.Text.ToString().Trim())))
            {
                ComboFilterList.Items.Add(ComboFilterList.Text.ToString());
                oVariables.oFilterList_ARRAY.Add(ComboFilterList.Text.ToString());
            }
            lblAdd.BorderStyle = BorderStyle.None;
        }

        private void lblDel_Click(object sender, EventArgs e)
        {
            lblDel.BorderStyle = BorderStyle.Fixed3D;
            if (oVariables.oFilterList_ARRAY.Contains(ComboFilterList.Text.ToString()))
            {
                oVariables.oFilterList_ARRAY.Remove(ComboFilterList.Text.ToString());
                ComboFilterList.Items.Remove(ComboFilterList.Text.ToString());
            }
            lblDel.BorderStyle = BorderStyle.None;
        }

        private void chkEnableFilter_CheckedChanged(object sender, EventArgs e)
        {

            if (chkEnableFilter.Checked)
            {
                lblAdd.Enabled = true;
                lblDel.Enabled = true;
                ComboFilterList.Enabled = true;
                oVariables.bEnableFilter = true;

            }
            else
            {
                lblAdd.Enabled = false;
                lblDel.Enabled = false;
                ComboFilterList.Enabled = false;
                oVariables.bEnableFilter = false;
            }
        }

        #endregion

        #region Drag and Drop Calls
        private void DgViewMessage_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string FilePath = null;
                foreach (string fileName in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    FilePath = fileName;
                }
                File.Copy(FilePath, oVariables.sTempFolder + "\\messagebody.tmp");
                FileStream file = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(file);
                txtBody.Text = sr.ReadToEnd();
                oVariables.sCurrentMessageBody= sr.ReadToEnd(); 
                sr.Close();
                file.Close();
                Cursor = Cursors.Arrow;
                txtBody.BringToFront();
                rdoBytext.Checked = true;
                //txtXMLFile.Text = xmlGridView.DataFilePath.ToString();
            }
            catch
            { }
        }

        private void DgViewMessage_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
            }
            catch
            { }
        }

        private void DgViewMessage_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
            Cursor = Cursors.Arrow;

        }

        private void txtBody_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                //Cursor = Cursors.WaitCursor;
                string FilePath = null;
                foreach (string fileName in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    FilePath = fileName;
                }
                File.Copy(FilePath, oVariables.sTempFolder + "\\messagebody.tmp",true);
                FileStream file = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(file);
                txtBody.Text = sr.ReadToEnd();
                oVariables.sCurrentMessageBody = sr.ReadToEnd(); 
                sr.Close();
                file.Close();
                Cursor = Cursors.Arrow;
                //txtXMLFile.Text = xmlGridView.DataFilePath.ToString();
            }
            catch
            { }
        }

        private void txtBody_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
            Cursor = Cursors.Arrow;

        }

        private void txtBody_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
            }
            catch
            { }
        }
        #endregion

        #endregion

        #region FileHash/ MD5
        private void btnOpenFile_FileHash_Click(object sender, EventArgs e)
        {
            OpenFileDialog MyOpenFileDialog = new OpenFileDialog();
            if (MyOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFieHashPath.Text = MyOpenFileDialog.FileName;
                GetHashInfo(MyOpenFileDialog.FileName);
            }
            else
            {
                txtFieHashPath.Text = "";
                txtMD5H.Text = "";
                txtMD5L.Text = "";
                txtFileHashH.Text = "";
                txtFileHashL.Text = "";
                txtSHA256H.Text = "";
                txtSHA256L.Text = "";
                txtSHA384H.Text = "";
                txtSHA384L.Text = "";
                txtSHA512H.Text = "";
                txtSHA512L.Text = "";
            }
        }

        private void GetHashInfo(string FilePath)
        {
            FileHash Hash = new FileHash();

            try
            {
                txtMD5H.Text = Hash.GetMD5Hash(FilePath).ToUpper();
                txtMD5L.Text = Hash.GetMD5Hash(FilePath).ToLower();
                txtFileHashH.Text = Hash.GetSHA1Hash(FilePath).ToUpper();
                txtFileHashL.Text = Hash.GetSHA1Hash(FilePath).ToLower();
                txtSHA256H.Text = Hash.GetSHA256Hash(FilePath).ToUpper();
                txtSHA256L.Text = Hash.GetSHA256Hash(FilePath).ToLower();
                txtSHA384H.Text = Hash.GetSHA384Hash(FilePath).ToUpper();
                txtSHA384L.Text = Hash.GetSHA384Hash(FilePath).ToLower();
                txtSHA512H.Text = Hash.GetSHA512Hash(FilePath).ToUpper();
                txtSHA512L.Text = Hash.GetSHA512Hash(FilePath).ToLower();

            }
            catch { }
        }

        private void filehashgroupbox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {

            try
            {
                string FilePath = null;
                foreach (string fileName in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    FilePath = fileName;
                }
                txtFieHashPath.Text = FilePath;
                GetHashInfo(FilePath);
            }
            catch
            { }
        }

        private void filehashgroupbox_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                Cursor = Cursors.Default;
            }
            catch
            { }
        }

        private void filehashgroupbox_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
            Cursor = Cursors.Arrow;
        }
        #endregion

        #region Web Browser
        private void MyBrowser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            oVariables.sStatusMessage = MyBrowser.StatusText.ToString();
            //if (MyBrowser.StatusText.ToString().ToLower() == "done")
            //{
            //    if (!(ComboNavigator.Items.Contains(MyBrowser.Url.OriginalString.ToString())))
            //        ComboNavigator.Items.Add(MyBrowser.Url.OriginalString.ToString());
            //    ComboNavigator.Text = MyBrowser.Url.OriginalString.ToString(); ;
            //}
        }

        private void MyBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            oVariables.sStatusMessage = MyBrowser.StatusText.ToString();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            MyBrowser.Navigate(ComboNavigator.Text.ToString());
            ComboNavigator.Enabled = false;
            if (!(ComboNavigator.Items.Contains(ComboNavigator.Text.ToString())))
                ComboNavigator.Items.Add(ComboNavigator.Text.ToString());

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            MyBrowser.GoBack();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            MyBrowser.GoForward();
        }

        private void MyBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string sNavigationPath = null;
            if (MyBrowser.Url.Scheme == "file")
                sNavigationPath = MyBrowser.Url.LocalPath.ToString();
            else
                sNavigationPath = MyBrowser.Url.OriginalString.ToString();

            if (!(ComboNavigator.Items.Contains(sNavigationPath)))
                ComboNavigator.Items.Add(sNavigationPath);
            ComboNavigator.Text = sNavigationPath;
            ComboNavigator.Enabled = true;
        }
        #endregion

        #region Got Selected Textbox
        private void txtBody_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
            {
                txtBody.SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && ((e.KeyCode == System.Windows.Forms.Keys.X) || (e.KeyCode == System.Windows.Forms.Keys.T)))
            {
                txtBody.Cut();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.P || e.KeyCode == System.Windows.Forms.Keys.V))
            {
                txtBody.Paste();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.C))
            {
                txtBody.Copy();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }

            else
                base.OnKeyDown(e);



        }

        private void txtQueueName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
            {
                txtQueueName.SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && ((e.KeyCode == System.Windows.Forms.Keys.X) || (e.KeyCode == System.Windows.Forms.Keys.T)))
            {
                txtQueueName.Cut();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.P))
            {
                txtQueueName.Paste();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.C))
            {
                txtQueueName.Copy();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }

            else
                base.OnKeyDown(e);

        }

        private void ComBoxComputer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
            {
                ComBoxComputer.SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && ((e.KeyCode == System.Windows.Forms.Keys.X) || (e.KeyCode == System.Windows.Forms.Keys.T)))
            {
                //ComBoxComputer.
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.P))
            {
                //ComBoxComputer.Paste();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.C))
            {
                // ComBoxComputer.Copy();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }

            else
                base.OnKeyDown(e);
        }

        private void txtQueuelabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.A))
            {
                txtQueuelabel.SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && ((e.KeyCode == System.Windows.Forms.Keys.X) || (e.KeyCode == System.Windows.Forms.Keys.T)))
            {
                txtQueuelabel.Cut();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.P))
            {
                txtQueuelabel.Paste();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            else if (e.Control && (e.KeyCode == System.Windows.Forms.Keys.C))
            {
                txtQueuelabel.Copy();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }

            else
                base.OnKeyDown(e);
        }

        private void toolStripMenuItemRefresqueue_Click(object sender, EventArgs e)
        {

            UpdateTreeViewbySpecificQueuename();

        }

        private void txtBody_TextChanged(object sender, EventArgs e)
        {

            //if (oVariables.oUndoList_ARRAY.Count == 0)
            //    oVariables.oUndoList_ARRAY.Add(txtBody.Text.ToString());
            if ((!(oVariables.oUndoPressed)) && (!(oVariables.oRedoPressed)))
            {
                if (!(String.IsNullOrEmpty(oVariables.sUndoPreviousvalue)))
                    oVariables.oUndoList_ARRAY.Add(oVariables.sUndoPreviousvalue);
            }
            if (oVariables.oRedoList_ARRAY.Count >= oVariables.ClipboardLimit)
            {
                oVariables.oRedoList_ARRAY.RemoveAt(0);
            }
            if (oVariables.oUndoList_ARRAY.Count >= oVariables.ClipboardLimit)
            {
                oVariables.oUndoList_ARRAY.RemoveAt(0);
            }

        }

        private void txtBody_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue > 47)
                oVariables.sUndoPreviousvalue = txtBody.Text.ToString();


        }
        #endregion

        #region Search Engine
        private void btnGoSearch_Click(object sender, EventArgs e)
        {

            btnGoSearch.Enabled = false;
            ResultListMessage ResultTemp = new ResultListMessage();
            ArrayList RTemp = new ArrayList();
            try
            {
                dgSearchresult.Rows.Clear();

                if (oVariables.oPrivateQueue_ARRAY.Count > 0)
                {
                    RTemp = dgSearchSetValues(oVariables.oPrivateQueue_ARRAY);
                    ResultTemp.Message = RTemp;
                }
                if (oVariables.oPublicQueue_ARRAY.Count > 0)
                {
                    RTemp = dgSearchSetValues(oVariables.oPublicQueue_ARRAY);
                    ResultTemp.Message.AddRange(RTemp);
                }


                if (ResultTemp.Message != null)
                {

                    bool CheckExist = true;
                    foreach (ResultListMessage Message in oVariables.oResultList_ARRAY)
                    {
                        if (Message.Search.Equals(txtSearch.Text.ToString()))
                        {
                            //oVariables.oResultList_ARRAY.RemoveAt[oVariables.oResultList_ARRAY.LastIndexOf()
                            CheckExist = false;
                            oVariables.oResultList_ARRAY.Remove(Message);
                            break;
                        }
                    }
                    if (CheckExist)
                    {
                        DGListResult.Rows.Add(txtSearch.Text.ToString());
                    }
                    if (ResultTemp.Message.Count == 0)
                        oVariables.sStatusMessage = "Search is complete. There are no results to display";
                    else
                        oVariables.sStatusMessage = ResultTemp.Message.Count + " occurences found";

                    ResultTemp.Search = txtSearch.Text.ToString();
                    oVariables.oResultList_ARRAY.Add(ResultTemp);
                }
            }
            catch { }
            btnGoSearch.Enabled = true;

        }
        private ArrayList dgSearchSetValues(ArrayList ArrayQueue)
        {
            ArrayList arrayTemp = new ArrayList();
            try
            {
                foreach (StructuredQueue oQueues in ArrayQueue)
                {

                    Application.DoEvents();

                    if (chkSpecificQueue.Checked)
                    {
                        if ((oQueues.Message != null) && oQueues.sPath.EndsWith(cboSpecificQueue.Text))
                        {
                            arrayTemp = UpdateSearchResult(oQueues);
                            break;
                        }
                    }
                    else
                    {
                        if (oQueues.Message != null)
                            arrayTemp.AddRange(UpdateSearchResult(oQueues));
                    }
                }

            }
            catch (Exception)
            {
                oVariables.sStatusMessage = "Error in Regular Expression, " + txtSearch.Text.ToString() + " is not allowed";
            }
            return arrayTemp;
        }
        public ArrayList UpdateSearchResult(StructuredQueue oQueues)
        {
            ArrayList arrayTemp = new ArrayList();
            DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                column.FlatStyle = FlatStyle.Standard; column.ThreeState = true;
                column.CellTemplate = new DataGridViewCheckBoxCell();
                column.CellTemplate.Style.BackColor = Color.Beige;
            }
            try
            {
                Object[] cellValues = null;

                foreach (QueueInfos Messages in oQueues.Message)
                {
                    Application.DoEvents();
                    bool InfoFlag = false;
                    if ((chkqueuname.Checked) && (Regex.IsMatch(oQueues.sQueueName, txtSearch.Text.ToString(),RegexOptions.IgnoreCase)))
                        InfoFlag = true;

                    if ((chkLabel.Checked) && (Regex.IsMatch(Messages.Label, txtSearch.Text.ToString(), RegexOptions.IgnoreCase)))
                        InfoFlag = true;

                    if ((chkBody.Checked) && (Regex.IsMatch(Messages.Body, txtSearch.Text.ToString(), RegexOptions.IgnoreCase)))
                        InfoFlag = true;

                    if (Messages.Transact == 2)
                        Messages.Transaction = "True";
                    else if (Messages.Transact == 3)
                        Messages.Transaction = "False";
                    else if (Messages.Transact == 1)
                        Messages.Transaction = "Remote";
                    Messages.Path = oQueues.sPath;
                    Messages.QueueName = oQueues.sQueueName;
                    if (InfoFlag)
                    {
                        cellValues = new Object[]
                                {false,
                                (Convert.ToInt32(dgSearchresult.Rows.Count) +1) ,
                                Messages.Path, 
                                Messages.QueueName,
                                Messages.Label,
                                Messages.Body,
                                Messages.Priority,
                                Messages.ID,
                                Messages.Transaction,
                                Messages.SentTime
                                    
                                };

                        dgSearchresult.Rows.Add(cellValues);
                        arrayTemp.Add(Messages);
                    }
                }
            }
            catch { }
            return arrayTemp;
        }
        private void dgSearchresult_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // txtBodyWeb.Visible = true;
                //txtBody.Visible = true;
                //  rdoBytext.Checked = true;
                QueueInfos MyMessage = new QueueInfos();

                MyMessage.Queue = dgSearchresult.Rows[e.RowIndex].Cells["QueueName"].Value.ToString();
                MyMessage.Label = dgSearchresult.Rows[e.RowIndex].Cells["sLabel"].Value.ToString();
                MyMessage.Body = dgSearchresult.Rows[e.RowIndex].Cells["sBody"].Value.ToString();
                MyMessage.Priority = dgSearchresult.Rows[e.RowIndex].Cells["sPriority"].Value.ToString();
                MyMessage.ID = dgSearchresult.Rows[e.RowIndex].Cells["sMessageID"].Value.ToString();
                if (dgSearchresult.Rows[e.RowIndex].Cells["FormatName"].Value.ToString().Contains("private$"))
                    oVariables.sQueueType = "\\private$";
                else
                    oVariables.sQueueType = null;

                if (dgSearchresult.Rows[e.RowIndex].Cells["Transactional"].Value.ToString().Contains("True"))
                    oVariables.IsTransactional = true;
                else if (dgSearchresult.Rows[e.RowIndex].Cells["Transactional"].Value.ToString().Contains("False"))
                    oVariables.IsTransactional = false;
                oVariables.sComputerName = ComBoxComputer.Text.ToString();
                ResendtoolStripMenuItem.Enabled = true;
                msmqgroupbox.Visible = true;
                msmqgroupbox.BringToFront();
                groupboxSearch_Text.Visible = false;
                SetQueueInfos(MyMessage);
            }
            catch { }
        }
        private void btnClearResult_Click(object sender, EventArgs e)
        {

            DGListResult.Rows.Clear();
            oVariables.oResultList_ARRAY.Clear();

        }
        private void DGListResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgSearchresult.Rows.Clear();
            try
            {
                if ((DGListResult.Rows.Count > 0) && (e.RowIndex != -1))
                {
                    foreach (ResultListMessage Message in oVariables.oResultList_ARRAY)
                    {
                        if (DGListResult.Rows[e.RowIndex].Cells["SearchResult"].Value != null)
                        {
                            txtSearch.Text = DGListResult.Rows[e.RowIndex].Cells["SearchResult"].Value.ToString();
                            if (DGListResult.Rows[e.RowIndex].Cells["SearchResult"].Value.ToString().Equals(Message.Search))
                            {
                                Object[] cellValues = null;
                                foreach (QueueInfos Messages in Message.Message)
                                {

                                    cellValues = new Object[]
                                {
                                (Convert.ToInt32(dgSearchresult.Rows.Count)+1) ,
                               Messages.Path, 
                                Messages.QueueName,
                                Messages.Label,
                                Messages.Body,
                                Messages.Priority,
                                Messages.ID,
                                Messages.Transaction,
                                Messages.SentTime
                                    
                                };
                                    dgSearchresult.Rows.Add(cellValues);

                                }
                                if (dgSearchresult.RowCount == 0)
                                    oVariables.sStatusMessage = "Search is complete. There are no results to display";
                                else
                                    oVariables.sStatusMessage = dgSearchresult.RowCount.ToString() + " occurences found";
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void chkSpecificQueue_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSpecificQueue.Checked)
                cboSpecificQueue.Enabled = true;
            else
                cboSpecificQueue.Enabled = false;

        }

        private void dgSearchresult_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            try
            {
                if (e.ColumnIndex == 0 && e.RowIndex == -1)
                {
                    int i = 0;
                    bool HeaderCheck = false;
                    HeaderCheck = ((QXplorer.DGVColumnHeader)(((DataGridViewColumn)(DataGridCheckBox)).HeaderCell)).CheckAll;

                    if (HeaderCheck)
                    {
                        while (i < dgSearchresult.Rows.Count)
                        {
                            dgSearchresult.Rows[i].Cells[0].Value = dgvColumnHeader.CheckAll;


                            i++;
                        }
                    }
                    else
                    {
                        while (i < dgSearchresult.Rows.Count)
                        {
                            dgSearchresult.Rows[i].Cells[0].Value = false;
                            i++;
                        }
                    }
                }
            }
            catch { }
        }

        private void dgSearchresult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                int i = 0;
                while (i < dgSearchresult.Rows.Count)
                {
                    if (dgSearchresult.CurrentCell.RowIndex == i)
                    {
                        if (Convert.ToBoolean(dgSearchresult.Rows[i].Cells[0].Value.ToString()))
                            dgSearchresult.Rows[i].Cells[0].Value = false;
                        else
                            dgSearchresult.Rows[i].Cells[0].Value = true;
                    }
                    i++;
                }
            }
            else
            {
                base.OnClick(e);
            }
        }

        private void btnBulkResend_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                if (oVariables.IsRemote)
                {
                    MessageBox.Show("Remote sending bulk message is not supported",
                                        "Message Queuing Admin", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                }
                while (i < dgSearchresult.Rows.Count)
                {

                    if (Convert.ToBoolean(dgSearchresult.Rows[i].Cells[0].Value.ToString()))
                    {
                        oVariables.sStatusMessage = "Deleting message";
                        oVariables.sQueueFormatType = oVariables.GetQueueFormatType(oVariables.sComputerName);
                        oVariables.sMessageQueuename = dgSearchresult.Rows[i].Cells[3].Value.ToString();
                        oVariables.sMessageLabel = dgSearchresult.Rows[i].Cells[4].Value.ToString();
                        oVariables.sMessageID = dgSearchresult.Rows[i].Cells[7].Value.ToString();
                        oVariables.sMessageBody = dgSearchresult.Rows[i].Cells[5].Value.ToString();
                        oVariables.sMessagePriority = dgSearchresult.Rows[i].Cells[6].Value.ToString();
                        oVariables.IsTransactional = Convert.ToBoolean(dgSearchresult.Rows[i].Cells[8].Value.ToString());

                        oMSMQ.DeleteMessage(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                              oVariables.sMessageQueuename, oVariables.sMessageID, oVariables.sMessageLabel);
                        oVariables.sStatusMessage = "Succesfully delete message";

                        oVariables.sStatusMessage = "Start Sending message to " + oVariables.sMessageQueuename;
                        oMSMQ.SendMessage(oVariables.sQueueType, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                            oVariables.sMessageQueuename, oVariables.sMessageLabel, oVariables.sMessageBody, oVariables.IsTransactional, oVariables.sMessagePriority, oVariables.IsJournal, oVariables.IsRemote);
                        oVariables.sStatusMessage = "Sending message done";
                        i++;
                    }
                }
            }
            catch { }
        }

        private void btnBuldDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                while (i < dgSearchresult.Rows.Count)
                {

                    if (Convert.ToBoolean(dgSearchresult.Rows[i].Cells[0].Value.ToString()))
                    {
                        oVariables.sStatusMessage = "Deleting message";
                        oVariables.sQueueFormatType = oVariables.GetQueueFormatType(oVariables.sComputerName);
                        //                    oVariables.sQueueFormatType = dgSearchresult.Rows[i].Cells[2].Value.ToString();
                        oVariables.sMessageQueuename = dgSearchresult.Rows[i].Cells[3].Value.ToString();
                        oVariables.sMessageLabel = dgSearchresult.Rows[i].Cells[3].Value.ToString();
                        oVariables.sMessageID = dgSearchresult.Rows[i].Cells[7].Value.ToString();
                        oVariables.sMessageBody = dgSearchresult.Rows[i].Cells[5].Value.ToString();
                        oVariables.sMessagePriority = dgSearchresult.Rows[i].Cells[6].Value.ToString();
                        oVariables.IsTransactional = Convert.ToBoolean(dgSearchresult.Rows[i].Cells[8].Value.ToString());

                        oMSMQ.DeleteMessage(ref oVariables, oVariables.sQueueFormatType, oVariables.sComputerName + oVariables.sQueueType,
                              oVariables.sMessageQueuename, oVariables.sMessageID, oVariables.sMessageLabel);
                        oVariables.sStatusMessage = "Succesfully delete message";
                        i++;
                    }
                }
            }
            catch { }
        }
        #endregion

        #region save Message
        private void SaveByQueueName(string sFolderSave)
        {
            try
            {
                ArrayList TempArraySave = new ArrayList();
                if (oVariables.sQueueType == null)
                    TempArraySave = oVariables.oPublicQueue_ARRAY;
                else if (oVariables.sQueueType.Contains("private"))
                    TempArraySave = oVariables.oPrivateQueue_ARRAY;

                foreach (StructuredQueue Queues in TempArraySave)
                {
                    Application.DoEvents();
                    string sFileName = oVariables.RemoveSpecialChar(oVariables.sMessageQueuename);
                    if (Queues.sQueueName.Trim() == oVariables.sMessageQueuename)
                    {
                        //objVar.SaveAllQueueFilename = sFolderSave + "\\" + sFileName + ".xml";
                        int MessageCounter = 1;
                        using (StreamWriter myStream = new StreamWriter(sFolderSave + "\\" + sFileName + ".xml"))
                        {
                            myStream.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n");
                            myStream.Write("<Message>\r\n");

                            foreach (QueueInfos Messages in Queues.Message)
                            {
                                Application.DoEvents();
                                oVariables.sStatusMessage = MessageCounter + " message save -> Message Label :" + Messages.Label.Trim();
                                //myStream.Write("\r\n" + MessageCounter + "\r\n");
                                //myStream.Write(Messages.Body + "\r\n");
                                myStream.Write("  <MessageQueue>\r\n");
                                //myStream.Write("    <MSMQServer>" + objVar.sComputerName + "</MSMQServer>\r\n");
                                //myStream.Write("    <QueueName>" + Messages.Queue + "</QueueName>\r\n");
                                //myStream.Write("    <Priority>" + Messages.Priority + "</Priority>\r\n");
                                myStream.Write("    <QueueLabel>" + Messages.Label + "</QueueLabel>\r\n");
                                //myStream.Write("     <QueueType>" + objVar.sQueueType + "</QueueType>\r\n");
                                //myStream.Write("     <Transactional>" + Messages.Transact + "</Transactional>\r\n");
                                //myStream.Write("     <Journal>" + bJournal + "</Journal>\r\n");
                                myStream.Write("    <QueueBody><![CDATA[" + Messages.Body + "]]></QueueBody>\r\n");
                                myStream.Write("  </MessageQueue>\r\n");
                                MessageCounter++;
                            }
                            myStream.Write("</Message>\r\n");
                            myStream.Close();
                        }
                        //MessageBox.Show("Succesfully save message in current queue",
                        //   "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        oVariables.sStatusMessage = MessageCounter + "Succesfully save message in current queue";
                        break;
                    }
                }
            }
            catch { }
        }
        private void SaveByLabel(string sFolderSave, int MessageSaveBy)
        {
            ArrayList TempArraySave = new ArrayList();
            try
            {
                if (oVariables.sQueueType == null)
                    TempArraySave = oVariables.oPublicQueue_ARRAY;
                else if (oVariables.sQueueType.Contains("private"))
                    TempArraySave = oVariables.oPrivateQueue_ARRAY;


                foreach (StructuredQueue Queues in TempArraySave)
                {
                    if (Queues.sQueueName.Trim() == oVariables.sMessageQueuename)
                    {
                        int MessageCounter = 1;
                        int filecounter = 0;
                        foreach (QueueInfos Messages in Queues.Message)
                        {

                            string sFileName = "";
                            switch (MessageSaveBy)
                            {
                                case 1:
                                    sFileName = oVariables.RemoveSpecialChar(Messages.Label.Trim());
                                    break;
                                case 2:
                                    sFileName = oVariables.RemoveSpecialChar(Messages.ID.Trim());
                                    break;
                                case 3:
                                    sFileName = oVariables.RemoveSpecialChar(Messages.Label.Trim()) + "_" + oVariables.RemoveSpecialChar(Messages.ID.Trim());
                                    break;
                            }
                            oVariables.sStatusMessage = MessageCounter + " message save -> Message Label :" + Messages.Label.Trim();
                            if (File.Exists(sFolderSave + "\\" + sFileName + ".xml"))
                            {
                                filecounter = 0;
                                while (File.Exists(sFolderSave + "\\" + sFileName + "_" + filecounter + ".xml"))
                                {
                                    filecounter++;
                                }
                                using (StreamWriter myStream = new StreamWriter(sFolderSave + "\\" + sFileName + "_" + filecounter + ".xml"))
                                {
                                    // myStream.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n");
                                    // myStream.Write("<Message>\r\n");
                                    // myStream.Write("  <MessageQueue>\r\n");
                                    // myStream.Write("     <MSMQServer>" + objVar.sComputerName + "</MSMQServer>\r\n");
                                    //myStream.Write("     <QueueName>" + Messages.Queue + "</QueueName>\r\n");
                                    //myStream.Write("     <Priority>" + Messages.Priority + "</Priority>\r\n");
                                    //myStream.Write("     <QueueLabel>" + Messages.Label + "</QueueLabel>\r\n");
                                    //myStream.Write("     <QueueType>" + objVar.sQueueType + "</QueueType>\r\n");
                                    //myStream.Write("     <Transactional>" + Messages.Transact + "</Transactional>\r\n");
                                    //myStream.Write("     <Journal>" + bJournal + "</Journal>\r\n");
                                    //myStream.Write("     <QueueBody><![CDATA[\r\n" + Messages.Body + "]]></QueueBody>\r\n");
                                    myStream.Write(Messages.Body);
                                    //myStream.Write("  </MessageQueue>\r\n");
                                    //myStream.Write("</Message>\r\n");
                                    myStream.Close();
                                }
                            }
                            else
                            {
                                using (StreamWriter myStream = new StreamWriter(sFolderSave + "\\" + sFileName + ".xml"))
                                {
                                    //myStream.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n");
                                    //myStream.Write("<Message>\r\n");
                                    //myStream.Write("  <MessageQueue>\r\n");
                                    //myStream.Write("     <MSMQServer>" + objVar.sComputerName + "</MSMQServer>\r\n");
                                    //myStream.Write("     <QueueName>" + Messages.Queue + "</QueueName>\r\n");
                                    //myStream.Write("     <Priority>" + Messages.Priority + "</Priority>\r\n");
                                    //myStream.Write("     <QueueLabel>" + Messages.Label + "</QueueLabel>\r\n");
                                    //myStream.Write("     <QueueType>" + objVar.sQueueType + "</QueueType>\r\n");
                                    //myStream.Write("     <Transactional>" + Messages.Transact + "</Transactional>\r\n");
                                    //myStream.Write("     <Journal>" + bJournal + "</Journal>\r\n");
                                    //myStream.Write("     <QueueBody><![CDATA[" + Messages.Body + "]]></QueueBody>\r\n");
                                    myStream.Write(Messages.Body);
                                    //myStream.Write("  </MessageQueue>\r\n");
                                    // myStream.Write("</Message>\r\n");
                                    myStream.Close();
                                }
                            }
                            MessageCounter++;

                        }
                        oVariables.sStatusMessage = MessageCounter + "Succesfully save message in current queue";
                        break;
                    }
                }
            }
            catch { }
        }
        private void byQueueNameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                FolderBrowserDialog MyFolder = new FolderBrowserDialog();
                MyFolder.SelectedPath = Environment.CurrentDirectory.ToString();
                oVariables.sMessageQueuename = txtQueueName.Text.ToString().Trim();
                if (MyFolder.ShowDialog() == DialogResult.OK)
                {
                    Thread newThread = new Thread(delegate()
                               {
                                   Application.DoEvents();
                                   SaveByQueueName(MyFolder.SelectedPath);
                               });
                    newThread.Start();
                    while (newThread.IsAlive)
                    {
                        Application.DoEvents();
                        if (oVariables.bCancelAllProcessFlag)
                        {
                            oVariables.bCancelAllProcessFlag = false;
                            newThread.Abort();
                        }
                    }
                }
            }
            catch { }
        }
        private void byLabelOrMessageSelection_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog MyFolder = new FolderBrowserDialog();
                MyFolder.SelectedPath = Environment.CurrentDirectory.ToString();
                oVariables.sMessageQueuename = txtQueueName.Text.ToString().Trim();
                if (MyFolder.ShowDialog() == DialogResult.OK)
                {
                    Thread newThread = new Thread(delegate()
                               {
                                   Application.DoEvents();
                                   if (sender == byLabelToolStripMenuItem)
                                       SaveByLabel(MyFolder.SelectedPath, 1);
                                   else if (sender == byMessageIDToolStripMenuItem)
                                       SaveByLabel(MyFolder.SelectedPath, 2);
                                   else if (sender == byLabelAndMessageIDToolStripMenuItem)
                                       SaveByLabel(MyFolder.SelectedPath, 3);

                               });
                    newThread.Start();
                    while (newThread.IsAlive)
                    {
                        Application.DoEvents();
                        if (oVariables.bCancelAllProcessFlag)
                        {
                            oVariables.bCancelAllProcessFlag = false;
                            newThread.Abort();
                        }
                    }
                }
            }
            catch { }

        }
        #endregion

          public class NodeSorter : IComparer
        {

            private TreeNode _nodeToSort;

            private string _sortType;



            public NodeSorter(TreeNode node, string sortType)
            {

                this._nodeToSort = node;

                this._sortType = sortType;

            }



            public int Compare(object x, object y)
            {

                TreeNode tx = x as TreeNode;

                TreeNode ty = y as TreeNode;



                if (tx.Parent == this._nodeToSort && ty.Parent == this._nodeToSort)
                {

                    // Ascending

                    if (this._sortType == "Ascending")

                        return string.Compare(tx.Text, ty.Text);

                    // Descending

                    if (this._sortType == "Descending")

                        return string.Compare(ty.Text, tx.Text);

                }

                return 0;

            }

        }

   

     
    }
}