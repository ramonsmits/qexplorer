using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
//
//Author : JuLius Lucero Ramos, ECE, QA
//Email  : juliusLramos@hotmail.com
//

namespace QXplorer
{
    public class GlobalVariables
    {
        public string sStatusText = "";
        public bool bAddComputer = true;
        public bool bAddComputerUpdate = true;
        public string sComputerName = "";

        public bool IsRemote = false;
        public bool IsHostname = true;
        public bool IsMultimessage = false;
        public string sQueueFormatType = "";
        public string sQueueType = null;
        public string sMessageBody = null;
        public string sMessageLabel = null;
        public string sMessagePriority = null;
        public string sMessageQueuename = null;
        public string sMessageID = null;

        public int iNumberofCopy;
        public int iMessageDelay;

        public bool IsJournal = false;
        public bool IsTransactional = true;

        public string sWebUrl = "";
        public string sTempFolder = Environment.GetEnvironmentVariable("Temp");
        public string sQXplorerxml = Environment.GetEnvironmentVariable("Temp") + "\\QXplorer.xml";
        public string sCurrentMessageBody;

        public bool bRefreshOnly= false;
        public bool bCallRefresh;
        public bool bSortQueue = false;

        public bool bLastRdo = true;
        public bool bClickDonate = false;

        public Int32 iCurrentSelectedBox=0;

        #region CheckBox Option
        public bool bIsPrivate;
        public bool bIsPublic;
        public bool bIsTransactional;
        public bool bIsOutgoing;
        public bool bIsSystem;
        public bool bIsJournal;
        #endregion

        #region Enable/Disable Buttons
        public bool bsavemessageStripMenuItem = false;
        public bool bloadmessageStripMenuItem = false;
        public bool bconnecToAnotherComputerToolStripMenuItem = true;
        public bool brecentComputersToolStripMenuItem = false;
        public bool bexitToolStripMenuItem =true;
        public bool bqueueToolStripMenuItem =true;
        public bool bnewQueueToolStripMenuItem;
        public bool bdeleteQueueToolStripMenuItem;
        public bool bmessageToolStripMenuItem = true;
        public bool bpurgeQueueToolStripMenuItem;
        public bool brefreshQueueToolStripMenuItem =true;
        public bool bRefreshQueuetoolStripButton = true;
        public bool bpurgeAllQueueToolStripMenuItem;
        public bool bdeleteAllQueueToolStripMenuItem;
        public bool bundoToolStripMenuItem;
        public bool bredoToolStripMenuItem;
        public bool bcutToolStripMenuItem ;
        public bool bcopyToolStripMenuItem;
        public bool bpasteToolStripMenuItem = true;
        public bool bselectAllToolStripMenuItem;
        public bool bclearUndoRedoToolStripMenuItem;
        public bool bsettingsToolStripMenuItem = true;
        public bool bhelpToolStripMenuItem = true;
        public bool bvisitMSMQQXplorerWebsiteToolStripMenuItem = true;
        public bool baboutToolStripMenuItem = true;
        public bool bCreateQueueStripButton;
        public bool bDeleteQueuetoolStripButton;
        public bool bPurgeQueuetoolStripButton;
        public bool bSendMessagetoolStripButton;
        public bool bDeleteMessagetoolStripButton;
        public bool bUndotoolStripButton;
        public bool bRedotoolStripButton;
        public bool bCuttoolStripButton;
        public bool bCopytoolStripButton;
        public bool bPastetoolStripButton=true;
        public bool bsendmessageStripMenuItem;
        public bool bdeletemessageStripMenuItem;
        public bool bbtnRefreshQueue = true;
        public bool bbtnSendMessage;
        public bool bbtnPurgeQueue;
        public bool bPurgeAllQRunning;
        public bool bCancelAllProcess = false;
        public bool bCancelAllProcessFlag = false;
       
        #endregion


        #region Settings
        public int iMaxMessageToQuery=100;
        public bool bEnableQuery = true;
        public bool bQueryMessage= true;
        public bool bEnableFilter = false;
        ArrayList oFilterList = new ArrayList();
        public ArrayList oFilterList_ARRAY
        {
            get { return oFilterList; }
            set
            {
                if (value != null)
                    oFilterList = value;
            }
        }
        public bool bViewByTable = true;
        public bool bAutoRefreshQueue = false;
        public int ClipboardLimit = 20;
        public bool bMessageQueueSorting = true;
        #endregion

        public string sStatusMessage = "";
        public int iProgressValue = 0;
        public int iProgressMax = 0;
        public bool bFlagProgressBar = false;

        ArrayList oPrivateQueue = new ArrayList();
        ArrayList oPublicQueue = new ArrayList();
        ArrayList oSystemQueue = new ArrayList();
        ArrayList oOutgoingQueue = new ArrayList();
        ArrayList oComputerList = new ArrayList();
              
        public ArrayList oPrivateQueue_ARRAY
        {
            get { return oPrivateQueue; }
            set
            {
                if (value != null)
                    oPrivateQueue = value;
            }
        }

        public ArrayList oPublicQueue_ARRAY
        {
            get { return oPublicQueue; }
            set
            {
                if (value != null)
                    oPublicQueue = value;
            }
        }
        public ArrayList oSystemQueue_ARRAY
        {
            get { return oSystemQueue; }
            set
            {
                if (value != null)
                    oSystemQueue = value;
            }
        }
        public ArrayList oOutgoingQueue_ARRAY
        {
            get { return oOutgoingQueue; }
            set
            {
                if (value != null)
                    oOutgoingQueue = value;
            }
        }

        public ArrayList oComputerList_ARRAY
        {
            get { return oComputerList; }
            set
            {
                if (value != null)
                    oComputerList = value;
            }
        }
        ArrayList oResultList = new ArrayList();
        public ArrayList oResultList_ARRAY
        {
            get { return oResultList; }
            set
            {
                if (value != null)
                    oResultList = value;
            }
        }
      

        ArrayList oUndoList = new ArrayList();
        public bool oUndoPressed =false;
        public ArrayList oUndoList_ARRAY
        {
            get { return oUndoList; }
            set
            {
                if (value != null)
                    oUndoList = value;
            }
        }
        public string sUndoPreviousvalue = null;

        
        ArrayList oRedoList = new ArrayList();
        public bool oRedoPressed = false;
        public ArrayList oRedoList_ARRAY
        {
            get { return oRedoList; }
            set
            {
                if (value != null)
                    oRedoList = value;
            }
        }
        public string sRedoPreviousvalue = null;

        public bool CheckIsRemote(string ComputerName)
        {
            bool flag = true;
            try
            {
                string CompareIP = Dns.GetHostEntry(ComputerName).AddressList[0].ToString().Trim();
                string LocalIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString().Trim();

                string CompareHostname = ComputerName.ToLower().Trim();
                string LocalHostname = Dns.GetHostName().ToString().ToLower().Trim();

                if ((LocalHostname == CompareHostname) || ((LocalIP == CompareIP)))
                    flag = false;
            }
            catch { }
            return flag;
        }
        public string GetQueueFormatType(string HostName)
        {
            if (Regex.IsMatch(HostName, "^\\d{1,3}.\\d{1,3}.\\d{1,3}.\\d{1,3}.*", RegexOptions.IgnoreCase))
            {
                sQueueFormatType = "FormatName:Direct=TCP:";
                IsHostname = false;
                return "FormatName:Direct=TCP:";
            }
            else
            {
                sQueueFormatType = "FormatName:Direct=OS:";
                IsHostname = true;
                return "FormatName:Direct=OS:";
            }
        }
        public string[] SplitByString(string testString, string split)
        {
            int offset = 0;
            int index = 0;
            int[] offsets = new int[testString.Length + 1];

            while (index < testString.Length)
            {
                int indexOf = testString.IndexOf(split, index);
                if (indexOf != -1)
                {
                    offsets[offset++] = indexOf;
                    index = (indexOf + split.Length);
                }
                else
                {
                    index = testString.Length;
                }
            }

            string[] final = new string[offset + 1];
            if (offset == 0)
            {
                final[0] = testString;
            }
            else
            {
                offset--;
                final[0] = testString.Substring(0, offsets[0]);
                for (int i = 0; i < offset; i++)
                {
                    final[i + 1] = testString.Substring(offsets[i] + split.Length, offsets[i + 1] - offsets[i] - split.Length);
                }
                final[offset + 1] = testString.Substring(offsets[offset] + split.Length);
            }
            return final;
        }
        public string RemoveSpecialChar(string sFileName)
        {
            try
            {
                string SpecialChar = "\\,/,*,:,?,<,>,|,\",\" \"";
                foreach (string s in SpecialChar.Split(new Char[] { ',' }))
                {
                    sFileName = sFileName.Replace(s, "");
                }
                if (String.IsNullOrEmpty(sFileName))
                    sFileName = "NoLabel";
                return sFileName;
            }
            catch
            {
                return "errorremove";
            }

        }
    }
    public struct StructuredQueue
    {
        public string sQueueName;
        public string sPath;
        public int bTransactional;
        public bool bUseJournal;
        public string sLabel;
        public int CountMessage;
        public string sFormatName;
        public int iQueueId;
        public ArrayList Message;
    }
    public class MessageInfo
    {
        public int ID;
        public ArrayList Message;
    }
    public class ResultListMessage
    {
        public string Search;
        public ArrayList Message;
    }

    class QueueInfos
    {
        public string Body;
        public string Label;
        public string ID;
        //public string LookUpId;
        public string Priority;
        public string SentTime;
        public string Queue;
        public string MessageID;
        public string Path;
        public string QueueName;
        public int Transact;
        public string Transaction;

    }
    class DGVColumnHeader : DataGridViewColumnHeaderCell
    {
        private Rectangle CheckBoxRegion;
        private bool checkAll = false;

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates dataGridViewElementState,
            object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value,
                formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            graphics.FillRectangle(new SolidBrush(cellStyle.BackColor), cellBounds);

            CheckBoxRegion = new Rectangle(
                cellBounds.Location.X + 1,
                cellBounds.Location.Y + 2,
                15, cellBounds.Size.Height - 4);


            if (this.checkAll)
                ControlPaint.DrawCheckBox(graphics, CheckBoxRegion, ButtonState.Checked);
            else
                ControlPaint.DrawCheckBox(graphics, CheckBoxRegion, ButtonState.Normal);

            Rectangle normalRegion =
                new Rectangle(
                cellBounds.Location.X + 1 + 15,
                cellBounds.Location.Y,
                cellBounds.Size.Width - 15,
                cellBounds.Size.Height);

            graphics.DrawString(value.ToString(), cellStyle.Font, new SolidBrush(cellStyle.ForeColor), normalRegion);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            //Convert the CheckBoxRegion 
            Rectangle rec = new Rectangle(new Point(0, 0), this.CheckBoxRegion.Size);
            this.checkAll = !this.checkAll;
            if (rec.Contains(e.Location))
            {
                this.DataGridView.Invalidate();
            }
            base.OnMouseClick(e);
        }

        public bool CheckAll
        {
            get { return this.checkAll; }
            set { this.checkAll = value; }
        }
    }


}
