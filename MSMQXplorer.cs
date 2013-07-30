using System;
using System.Collections.Generic;
using System.Text;
using System.Messaging;
using Message = System.Messaging.Message;
using System.Net;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
//
//Author : JuLius Lucero Ramos, ECE, QA
//Email  : juliusLramos@hotmail.com
//
namespace QXplorer
{

    public class MSMQXplorer
    {

        public void CreateQueue(ref GlobalVariables oVar, string MSMQServer, string QueueName, bool IsTransaction)
        {
            try
            {
                string MQPath = MSMQServer + "\\" + QueueName;
                if (MessageQueue.Exists(MQPath))
                {
                    MessageBox.Show(QueueName + " Queue is already exist",
                               "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if(IsTransaction)
                        MessageQueue.Create(MQPath, true);
                    else
                        MessageQueue.Create(MQPath, false);
                   // MessageBox.Show(QueueName + " Queue succesfully created",
                  //              "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DeleteQueue(ref GlobalVariables oVar, string MSMQServer, string QueueName, bool AllQueue)
        {
            string MQPath = MSMQServer + "\\" + QueueName;
            try
            {

                if (MessageQueue.Exists(MQPath))
                {
                    DialogResult result;
                    if (AllQueue)
                        result = DialogResult.Yes;
                    else
                        result = MessageBox.Show("Are you sure you want to delete " + MQPath + " ?",
                             "Message Queuing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        MessageQueue.Delete(MQPath);
                        if (!(AllQueue))
                            MessageBox.Show(QueueName + " was succesfully deleted", "Message Queuing",
                             MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(QueueName + " Queue is not exist", "Message Queuing",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void PurgeQueue(ref GlobalVariables oVar, string FormatType, string MSMQServer, string QueueName,bool AllQueue)
        {
            string MQPath = FormatType + MSMQServer + "\\" + QueueName; ;
            DialogResult result ;
            if (AllQueue)
                result = DialogResult.Yes;
            else
                result = MessageBox.Show("Are you sure you want to delete all messages in the queue?",
                    "Message Queuing Admin", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                try
                {
                    MessageQueue oMQueue = new MessageQueue(MQPath);
                    try
                    {
                        oMQueue.Receive(new TimeSpan(0, 0, 1));
                        oMQueue.Purge();
                        if (!(AllQueue))
                            MessageBox.Show("All message succesfully purge",
                                "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        if (!(AllQueue))
                            MessageBox.Show(ex.Message,
                                "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    oMQueue.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void SendMessage(string sQueueType, string FormatType, string MSMQServer, string QueueName,
            string MsgLabel, string MsgBody, bool IsTransaction, string Priority, bool IsJournal, bool IsRemote)
        {

            try
            {
             string MQPath=null;
            if (string.IsNullOrEmpty(sQueueType))
                    MQPath = MSMQServer + "\\" + QueueName;
                else
                    MQPath = FormatType+MSMQServer + "\\" + QueueName;
                MessageQueue oMQueue = new MessageQueue(MQPath);
                try
                {
                    System.Messaging.Message oMsg = new System.Messaging.Message(MsgBody, new ActiveXMessageFormatter());
                    if (Priority.ToLower() == "lowest")
                    { oMsg.Priority = MessagePriority.Lowest; }
                    else if (Priority.ToLower() == "verylow")
                    { oMsg.Priority = MessagePriority.VeryLow; }
                    else if (Priority.ToLower() == "low")
                    { oMsg.Priority = MessagePriority.Low; }
                    else if (Priority.ToLower() == "normal")
                    { oMsg.Priority = MessagePriority.Normal; }
                    else if (Priority.ToLower() == "abovenormal")
                    { oMsg.Priority = MessagePriority.AboveNormal; }
                    else if (Priority.ToLower() == "high")
                    { oMsg.Priority = MessagePriority.High; }
                    else if (Priority.ToLower() == "veryhigh")
                    { oMsg.Priority = MessagePriority.VeryHigh; }
                    else if (Priority.ToLower() == "highest")
                    { oMsg.Priority = MessagePriority.Highest; }

                    if (IsJournal)
                        oMsg.UseJournalQueue = true;

                    if (!(IsRemote))
                    {
        
                            if (!oMQueue.Transactional == IsTransaction)
                                MessageBox.Show("Transaction not match",
                                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                    }
                    oMsg.Label = MsgLabel;
                    if (IsTransaction == false)
                        oMQueue.Send(oMsg);
                    else
                        oMQueue.Send(oMsg, MessageQueueTransactionType.Single);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n Please check " + QueueName + " if you have permission or the queue is exist",
                              "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                oMQueue.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n Please check " + QueueName + " if you have permission or the queue is exist",
                          "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        public void DeleteMessage(ref GlobalVariables oVar, string FormatType, string MSMQServer, string QueueName, string MessageId,string MessageLabel)
        {
            string MQPath = FormatType + MSMQServer + "\\" + QueueName;
            try
            {
                MessageQueue oMQueue = new MessageQueue(MQPath);
                try
                {

                    oMQueue.ReceiveById(MessageId);
                    oVar.sStatusMessage = " Message succesfully deleted";
                    //MessageBox.Show(MessageLabel + " Message succesfully deleted",
                     //       "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show(ex + "Queue Name is not valid or exist");
                    oVar.sStatusMessage = ex.Message;
                }
                oMQueue.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                oVar.sStatusMessage = ex.Message;
            }
        
        }

        public void GetPrivateQueues(ref GlobalVariables oVar, string MSMQServer)
        {
            MessageQueue[] PrivateQueues = null;
            StructuredQueue oQueue = new StructuredQueue();
            QueueInfos QueueInfo = new QueueInfos();
            MessageInfo MyInfo = new MessageInfo();
            string sQueueOath = "";
            try
            {
                Application.DoEvents();
                oVar.bFlagProgressBar = true;
                PrivateQueues = MessageQueue.GetPrivateQueuesByMachine(MSMQServer);
                oVar.iProgressMax = PrivateQueues.Length;
                oVar.iProgressValue = 0;
                for (int i = 0; i < PrivateQueues.Length; i++)
                {
                    Application.DoEvents();
                    oVar.sStatusMessage = "Get list of Queues .. Current Queue Name : " + PrivateQueues[i].Path;
                    oQueue.sPath = PrivateQueues[i].Path;
                    PrivateQueues[i].MessageReadPropertyFilter.SetAll();
                    oQueue.sQueueName = PrivateQueues[i].QueueName.Replace("private$\\", "");
                 //   MessageQueue MessageInQueue = new System.Messaging.MessageQueue(oQueue.sPath, QueueAccessMode.SendAndReceive);
                    oQueue.CountMessage = 0;// GetQueueMessageCount(MSMQServer, PrivateQueues[i].QueueName);
                    if (oVar.IsRemote)
                    { oQueue.bTransactional = 1; }
                    else if (PrivateQueues[i].Transactional)
                    { oQueue.bTransactional = 2; }
                    else
                    { oQueue.bTransactional = 3;}
                    //oQueue.bUseJournal = PrivateQueues[i].UseJournalQueue;
                    // oQueue.sLabel = PrivateQueues[i].Label;
                    oQueue.sPath = PrivateQueues[i].Path;
                    oQueue.sFormatName = PrivateQueues[i].FormatName;
                    //oQueue.bTransactional = PrivateQueues[i].Transactional;
                    oQueue.iQueueId = i;
                    oVar.iProgressValue = i;
                    oVar.oPrivateQueue_ARRAY.Add(oQueue);
                }
                oVar.bFlagProgressBar = false;
               // oVar.oPrivateQueue_ARRAY.Sort();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + sQueueOath,
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        
        public void GetPublicQueues(ref GlobalVariables oVar, string MSMQServer)
        {
            MessageQueue[] PublicQueues = null;
            StructuredQueue oQueue = new StructuredQueue();
            QueueInfos QueueInfo = new QueueInfos();
            MessageInfo MyInfo = new MessageInfo();
            string sQueueOath = "";
            try
            {
                Application.DoEvents();
                oVar.bFlagProgressBar = true;
                PublicQueues = MessageQueue.GetPublicQueuesByMachine(MSMQServer);
                oVar.iProgressMax = PublicQueues.Length;
                oVar.iProgressValue = 0;
                for (int i = 0; i < PublicQueues.Length; i++)
                {
                    oVar.sStatusMessage = "Get list of Queues .. Current Queue Name : " + PublicQueues[i].Path;
                    Application.DoEvents();
                    oQueue.sPath = PublicQueues[i].Path;
                    oQueue.CountMessage = 0;//PublicQueues[i].GetAllMessages().Length;
                    oQueue.sQueueName = PublicQueues[i].QueueName;
                    //if (oVar.IsRemote)
                    //{ oQueue.bTransactional = 1; }
                    if (PublicQueues[i].Transactional)
                    { oQueue.bTransactional = 2; }
                    else
                    { oQueue.bTransactional = 3; }
                    //oQueue.bUseJournal = PrivateQueues[i].UseJournalQueue;
                    // oQueue.sLabel = PrivateQueues[i].Label;
                    oQueue.sPath = PublicQueues[i].Path;
                    oQueue.sFormatName = PublicQueues[i].FormatName;
                    //oQueue.bTransactional = PrivateQueues[i].Transactional;
                    oQueue.iQueueId = i;
                    oVar.iProgressValue = i;
                    //oVar.oPrivateQueue_ARRAY.Insert(MyInfo.ID, oQueue);
                    oVar.oPublicQueue_ARRAY.Add(oQueue);
                }
                oVar.bFlagProgressBar = false;
               // oVar.oPublicQueue_ARRAY.Sort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + sQueueOath,
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        //public void GetSystemQueues(ref GlobalVariables oVar, string MSMQServer)
        //{
        //    MessageQueue[] SystemQueues = null;
        //    StructuredQueue oQueue = new StructuredQueue();
        //    QueueInfos QueueInfo = new QueueInfos();
        //    MessageInfo MyInfo = new MessageInfo();
        //    string sQueueOath = "";
        //    try
        //    {
        //        oVar.bFlagProgressBar = true;
        //        //SystemQueues = MessageQueue(MSMQServer);
        //        oVar.iProgressMax = PublicQueues.Length;
        //        for (int i = 0; i < PublicQueues.Length; i++)
        //        {
        //            sQueueOath = PublicQueues[i].Path;
        //            oQueue.sQueueName = PublicQueues[i].QueueName;
        //            //if (oVar.IsRemote)
        //            //{ oQueue.bTransactional = 1; }
        //            if (PublicQueues[i].Transactional)
        //            { oQueue.bTransactional = 2; }
        //            else
        //            { oQueue.bTransactional = 3; }
        //            //oQueue.bUseJournal = PrivateQueues[i].UseJournalQueue;
        //            // oQueue.sLabel = PrivateQueues[i].Label;
        //            oQueue.sPath = PublicQueues[i].Path;
        //            //oQueue.bTransactional = PrivateQueues[i].Transactional;
        //            oQueue.iQueueId = i;
        //            oVar.iProgressValue = i;
        //            //oVar.oPrivateQueue_ARRAY.Insert(MyInfo.ID, oQueue);
        //            oVar.oPublicQueue_ARRAY.Add(oQueue);
        //        }
        //        oVar.bFlagProgressBar = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + "\r\n" + sQueueOath,
        //            "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }


        //}
        
        public void GetOutgoingQueues(ref GlobalVariables oVar, string MSMQServer)
        {
            ArrayList OutgoingQueuename = new ArrayList();
            try
            {
                MSMQ.MSMQApplication q = new MSMQ.MSMQApplication();
                object obj = q.ActiveQueues;

                Object[] oArray = (Object[])obj;
                for (int i = 0; i < oArray.Length; i++)
                {
                    if (oArray[i] == null)
                        continue;

                    if (oArray[i].ToString().IndexOf("DIRECT=") >= 0)
                    {
                        OutgoingQueuename.Add(oArray[i].ToString());

                    }
                }


            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
           // return OutgoingQueuename;

        //    MessageQueue[] SystemQueues = null;
        //    StructuredQueue oQueue = new StructuredQueue();
        //    QueueInfos QueueInfo = new QueueInfos();
        //    MessageInfo MyInfo = new MessageInfo();
        //    string sQueueOath = "";
        //    try
        //    {
        //        oVar.bFlagProgressBar = true;
        //        //SystemQueues = MessageQueue(MSMQServer);
        //        oVar.iProgressMax = PublicQueues.Length;
        //        for (int i = 0; i < PublicQueues.Length; i++)
        //        {
        //            sQueueOath = PublicQueues[i].Path;
        //            oQueue.sQueueName = PublicQueues[i].QueueName;
        //            //if (oVar.IsRemote)
        //            //{ oQueue.bTransactional = 1; }
        //            if (PublicQueues[i].Transactional)
        //            { oQueue.bTransactional = 2; }
        //            else
        //            { oQueue.bTransactional = 3; }
        //            //oQueue.bUseJournal = PrivateQueues[i].UseJournalQueue;
        //            // oQueue.sLabel = PrivateQueues[i].Label;
        //            oQueue.sPath = PublicQueues[i].Path;
        //            //oQueue.bTransactional = PrivateQueues[i].Transactional;
        //            oQueue.iQueueId = i;
        //            oVar.iProgressValue = i;
        //            //oVar.oPrivateQueue_ARRAY.Insert(MyInfo.ID, oQueue);
        //            oVar.oPublicQueue_ARRAY.Add(oQueue);
        //        }
        //        oVar.bFlagProgressBar = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + "\r\n" + sQueueOath,
        //            "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }


        }

        public ArrayList GetAllMessageInMSMQ(ref GlobalVariables oVar, ArrayList aTemp)
        {
            QueueInfos QueueInfo = new QueueInfos();
            MessageInfo MyInfo = new MessageInfo();
            ArrayList aTempReturn = new ArrayList();
            int MaxQuery = oVar.iMaxMessageToQuery;
            string sQueueOath = "";
            try
            {
                oVar.bFlagProgressBar = true;
               // ArrayList aTemp = new ArrayList(oVar.oPrivateQueue_ARRAY);
                //oVar.oPrivateQueue_ARRAY.Clear();
                oVar.iProgressMax = aTemp.Count;
                bool bQueryMessage =oVar.bQueryMessage;
                foreach (StructuredQueue oQueueTemp in aTemp)
                {
                    StructuredQueue oQueue = new StructuredQueue();
                    oQueue = oQueueTemp;
                    oVar.iProgressValue = oQueueTemp.iQueueId;
                    Application.DoEvents();
                    oVar.sStatusMessage = "Quering " + oQueueTemp.sPath + " Please wait...";
                    Thread newThread = new Thread(delegate()
                    {
                        Application.DoEvents();
                        if(bQueryMessage)
                            MyInfo = PeekAllMessage(oQueueTemp, oQueueTemp.iQueueId, MaxQuery);
                        else
                            MyInfo = GetAllMessage(oQueueTemp, oQueueTemp.iQueueId, MaxQuery);
                    });
                    newThread.Start();
                    while (newThread.IsAlive)
                    {
                        Application.DoEvents();
                        if (oVar.bCancelAllProcessFlag)
                        {
                            oVar.bCancelAllProcessFlag = false;
                            newThread.Abort();
                        }
                    }
                    oQueue.CountMessage = MyInfo.ID;
                    oQueue.Message = MyInfo.Message;
                    aTempReturn.Add(oQueue);
                }
                oVar.bFlagProgressBar = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + sQueueOath,
                    "Message Queuing Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return aTempReturn;

        }

        public MessageInfo GetAllMessage(StructuredQueue oQueueTemp, int myID, int MaxQuery)
        {
            MessageInfo MyInfo = new MessageInfo();
            try
            {
                Message[] messages = null;
                
                MessageQueue MessageInQueue = new System.Messaging.MessageQueue(oQueueTemp.sPath, QueueAccessMode.SendAndReceive);
                // MessageInQueue.MessageReadPropertyFilter.SetAll();
                MessageInQueue.MessageReadPropertyFilter.Id = true;
                MessageInQueue.MessageReadPropertyFilter.Priority = true;
                MessageInQueue.MessageReadPropertyFilter.SentTime = true;
                MessageInQueue.MessageReadPropertyFilter.MessageType = true;
                MessageInQueue.MessageReadPropertyFilter.Label = true;
                MessageInQueue.MessageReadPropertyFilter.Body = true;
                Application.DoEvents();
                messages = MessageInQueue.GetAllMessages();
                Application.DoEvents();
                ArrayList MyMessage = new ArrayList();
                // MyInfo.ID = MessageInQueue.GetAllMessages().Length;
                MyInfo.ID = messages.Length;
                for (int index = 0; index < messages.Length; index++)
                {
                    Application.DoEvents();
                    if (index >= MaxQuery)
                        break;
                    QueueInfos QueueInfo = new QueueInfos();
                    QueueInfo.ID = messages[index].Id;
                    QueueInfo.SentTime = messages[index].SentTime.ToString();
                    QueueInfo.Body = ReadMessageStream(messages[index].BodyStream);
                    QueueInfo.Label = messages[index].Label;
                    QueueInfo.Priority = messages[index].Priority.ToString();
                    QueueInfo.MessageID = index.ToString();
                    QueueInfo.Transact = oQueueTemp.bTransactional;
                    QueueInfo.Queue = oQueueTemp.sQueueName;
                    MyMessage.Add(QueueInfo);


                }
                MyInfo.Message = MyMessage;

            }
            catch 
            {

                    
               
            }
           
                return MyInfo;
          
        }

        public MessageInfo PeekAllMessage(StructuredQueue oQueueTemp, int myID, int MaxQuery)
        {
            //Message messages = null;
            MessageInfo MyInfo = new MessageInfo();
            try
            {
                
                MessageQueue MessageInQueue = new System.Messaging.MessageQueue(oQueueTemp.sPath, QueueAccessMode.SendAndReceive);
                System.Messaging.Cursor cursor = MessageInQueue.CreateCursor();
                Message m = PeekWithoutTimeout(MessageInQueue, cursor, PeekAction.Current);
                Application.DoEvents();
                ArrayList MyMessage = new ArrayList();
                MessageInQueue.MessageReadPropertyFilter.Id = true;
                MessageInQueue.MessageReadPropertyFilter.Priority = true;
                MessageInQueue.MessageReadPropertyFilter.SentTime = true;
                MessageInQueue.MessageReadPropertyFilter.MessageType = true;
                MessageInQueue.MessageReadPropertyFilter.Label = true;
                MessageInQueue.MessageReadPropertyFilter.Body = true;
                MessageInQueue.BeginPeek(new TimeSpan(0, 0, 10, 0));
                //MessageEnumerator msgQenum = MessageInQueue.GetMessageEnumerator2();
                int index = 0;
                //  MyInfo.ID = MessageInQueue.GetAllMessages().Length;
                if (m != null)
                {
                    while ((m = PeekWithoutTimeout(MessageInQueue, cursor, PeekAction.Current)) != null)
                    //while (msgQenum.MoveNext())
                    {
                        Application.DoEvents();

                        if (!(index > MaxQuery))
                        {
                            //                       messages = msgQenum.Current;
                            QueueInfos QueueInfo = new QueueInfos();
                            //QueueInfo.ID = messages.Id;
                            //QueueInfo.SentTime = messages.SentTime.ToString();
                            //QueueInfo.Body = ReadMessageStream(messages.BodyStream);
                            //QueueInfo.Label = messages.Label;
                            //QueueInfo.Priority = messages.Priority.ToString();
                            //QueueInfo.MessageID = index.ToString();
                            //QueueInfo.Transact = oQueueTemp.bTransactional;
                            //QueueInfo.Queue = oQueueTemp.sQueueName;
                            QueueInfo.ID = m.Id;
                            QueueInfo.SentTime = m.SentTime.ToString();
                            QueueInfo.Body = ReadMessageStream(m.BodyStream);
                            QueueInfo.Label = m.Label;
                            QueueInfo.Priority = m.Priority.ToString();
                            QueueInfo.MessageID = index.ToString();
                            QueueInfo.Transact = oQueueTemp.bTransactional;

                            QueueInfo.Queue = oQueueTemp.sQueueName;
                            MyMessage.Add(QueueInfo);
                        }
                        index++;
                        PeekWithoutTimeout(MessageInQueue, cursor, PeekAction.Next);
                    }
                }
                MyInfo.ID = index;
                MyInfo.Message = MyMessage;

            }
            catch  {}
           
                return MyInfo;
           
        }

        protected Message PeekWithoutTimeout(MessageQueue q, System.Messaging.Cursor cursor, PeekAction action)
        {
            Message ret = null;
            try
            {
                ret = q.Peek(new TimeSpan(1), cursor, action);
            }
            catch (MessageQueueException mqe)
            {
                if (!mqe.Message.ToLower().Contains("timeout"))
                {
                    throw;
                }
            }
            return ret;
        }

        //protected int GetMessageCount(MessageQueue q)
        //{
        //    int count = 0;
        //    Cursor cursor = q.CreateCursor();

        //    Message m = PeekWithoutTimeout(q, cursor, PeekAction.Current);
        //    if (m != null)
        //    {
        //        count = 1;
        //        while ((m = PeekWithoutTimeout(q, cursor, PeekAction.Next)) != null)
        //        {
        //            count++;
        //        }
        //    }
        //    return count;
        //}


        private string ReadMessageStream(Stream messageStream)
        {

            StreamReader messageReader = new StreamReader(messageStream, Encoding.Default);
            string messageString = messageReader.ReadToEnd().Replace("\0", "");
            messageReader.Close();
            return messageString;
        }
    }
}

