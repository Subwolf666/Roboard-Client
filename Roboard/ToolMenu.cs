using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
//using Ini;
using Roboard;
using Roboard.Events;

namespace Roboard
{
    public class ToolMenu
    {
        private TimeOut watchDogTimer;

        IniFile motionData;
        IniFile tempMotionData;

        private string _selectedMotionIndex;
        private string sendString;
        private string[] strMessage = new string[50];

        public ToolMenu()
        {
            watchDogTimer = new TimeOut();
        }

        // Play Motion
        //
        public bool Play()
        {
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(WriteNetworkClient_messageHandler);

            // 1 - send the selectedmotion index to the server
            //
            sendString = string.Format("PlayMotionFile,{0}", _selectedMotionIndex);
            Roboard.NetworkClient.SendMessage(sendString);

            this.Close();
            return true;
        }

        public bool Stop()
        {
            this.Close();
            return true;
        }

        public bool Pause()
        {
            this.Close();
            return true;
        }

        // Delete
        //
        public bool Delete()
        {
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(WriteNetworkClient_messageHandler);

            string strCommand = string.Format("DeleteMotionFile");

            // 1 - send the selectedmotion index to the server
            //
            sendString = string.Format("{0},{1}", strCommand, _selectedMotionIndex);
            if (!handleNetworkMessage(sendString))
                return false;

            this.Close();
            return true;
        }

        // Methods
        //
        public bool Read()
        {
            // Read a motion from the server
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(WriteNetworkClient_messageHandler);
            string strCommand = string.Format("ReadMotionFile");

            // 1 - send the selectedmotion index to the server
            //
            sendString = string.Format("{0},Open,{1}", strCommand, _selectedMotionIndex);
            if (!handleNetworkMessage(sendString))
                return false;
            // 2 - if exists than create a new motionData.
            //
            tempMotionData = new IniFile(string.Empty);
            //========================================
            // 3 - Read the GraphicalEdit part section
            //========================================

            // send to the server
            sendString = string.Format("{0},{1}", strCommand, StaticUtilities.SectionGraphicalEdit);
            if (!handleNetworkMessage(sendString))
                return false;

            IniSection graphicalEditSection = new IniSection();
            for (int i = 0; i < StaticUtilities.GraphicalEdit.Length; i++)
                graphicalEditSection.Add(StaticUtilities.GraphicalEdit[i], strMessage[i]);
            this.tempMotionData.Add(StaticUtilities.SectionGraphicalEdit, graphicalEditSection);

            //===========================
            // 4 - Read the items section
            //===========================
            int Items = Convert.ToInt32(tempMotionData[StaticUtilities.SectionGraphicalEdit][StaticUtilities.GraphicalEditItems]);
            for (int i = 0; i < Items; i++)
            {
                // send to the server
                sendString = string.Format("{0},{1}", strCommand, StaticUtilities.SectionItem);
                if (!handleNetworkMessage(sendString))
                    return false;

                IniSection itemSection = new IniSection();
                for (int j = 0; j < StaticUtilities.Item.Length - 1; j++) // minus one because Prm
                    itemSection.Add(StaticUtilities.Item[j], strMessage[j]);
                string strChannel = string.Format("{0}", strMessage[StaticUtilities.Item.Length - 1]);
                for (int k = StaticUtilities.Item.Length; k < strMessage.Length; k++)
                    strChannel = string.Format("{0},{1}", strChannel, strMessage[k]);
                itemSection.Add(StaticUtilities.ItemPrm, strChannel);
                this.tempMotionData.Add(string.Format("{0}{1}", StaticUtilities.SectionItem, i), itemSection);
            }
            //===========================
            // 5 - Read the links section
            //===========================
            int Links = Convert.ToInt32(tempMotionData[StaticUtilities.SectionGraphicalEdit][StaticUtilities.GraphicalEditLinks]);
            for (int i = 0; i < Links; i++)
            {
                // send to the server
                sendString = string.Format("{0},{1}", strCommand, StaticUtilities.SectionLink);
                if (!handleNetworkMessage(sendString))
                    return false;

                IniSection linkSection = new IniSection();
                for (int j = 0; j < StaticUtilities.Link.Length - 1; j++) // minus one because Point
                    linkSection.Add(StaticUtilities.Link[j], strMessage[j]);

                string strPoint = string.Format("{0}", strMessage[StaticUtilities.Link.Length - 1]);
                for (int k = StaticUtilities.Link.Length; k < strMessage.Length; k++)
                    strPoint = string.Format("{0},{1}", strPoint, strMessage[k]);
                linkSection.Add(StaticUtilities.LinkPoint, strPoint);
                this.tempMotionData.Add(string.Format("{0}{1}", StaticUtilities.SectionLink, i), linkSection);
            }
            // 6 - close and other things to be done.
            motionData = tempMotionData;
            this.Close();
            return true;
        }

        // Methods
        //
        public bool Write()
        {
            if (this.motionData == null)
                return false;

            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(WriteNetworkClient_messageHandler);

            string strCommand = string.Format("WriteMotionFile");

            // 5 parts to write
            //======================================
            // 1 - open the new motion to be written
            //======================================
            this.sendString = string.Format("{0},Open,{1}", strCommand, _selectedMotionIndex);
            if ((!handleNetworkMessage(this.sendString)) || (strMessage[0] != "Ok"))
                return false;

            //====================================
            // 2 - Write the graphicaledit section
            //====================================
            int geItems = motionData[StaticUtilities.SectionGraphicalEdit].Count;

            string[] str = new string[StaticUtilities.GraphicalEdit.Length];
            for (int i = 0; i < StaticUtilities.GraphicalEdit.Length; i++)
                str[i] = this.motionData[StaticUtilities.SectionGraphicalEdit][StaticUtilities.GraphicalEdit[i]];
            string message = string.Join(",", str);

            // send to the server
            this.sendString = string.Format("{0},{1},{2}", strCommand, StaticUtilities.SectionGraphicalEdit, message);
            if ((!this.handleNetworkMessage(this.sendString)) || (this.strMessage[0] != "Ok"))
                return false;

            //============================
            // 3 - Write the items section
            //============================
            int Items = Convert.ToInt32(motionData[StaticUtilities.SectionGraphicalEdit][StaticUtilities.GraphicalEditItems]);
            string[] strItem = new string[StaticUtilities.Item.Length];
            for (int i = 0; i < Items; i++)
            {
                string item = string.Format("{0}{1}", StaticUtilities.SectionItem, i);
                for (int j = 0; j < StaticUtilities.Item.Length; j++)
                    strItem[j] = this.motionData[item][StaticUtilities.Item[j]];
                message = string.Join(",", strItem);
                // send to the server
                this.sendString = string.Format("{0},{1},{2}", strCommand, StaticUtilities.SectionItem, message);
                if ((!this.handleNetworkMessage(this.sendString)) || (this.strMessage[0] != "Ok"))
                    return false;
            }

            //============================
            // 4 - Write the links section
            //============================
            int Links = Convert.ToInt32(motionData[StaticUtilities.SectionGraphicalEdit][StaticUtilities.GraphicalEditLinks]);
            string[] strLink = new string[StaticUtilities.Link.Length];
            for (int i = 0; i < Links; i++)
            {
                string item = string.Format("{0}{1}", StaticUtilities.SectionLink, i);
                for (int j = 0; j < StaticUtilities.Link.Length; j++)
                    strLink[j] = this.motionData[item][StaticUtilities.Link[j]];
                message = string.Join(",", strLink);
                // send to the server
                this.sendString = string.Format("{0},{1},{2}", strCommand, StaticUtilities.SectionLink, message);
                if ((!this.handleNetworkMessage(this.sendString)) || (this.strMessage[0] != "Ok"))
                    return false;
            }

            //============================
            // 5 - And save the new motion
            //============================
            this.sendString = string.Format("{0},Save", strCommand);
            if ((!this.handleNetworkMessage(this.sendString)) || (this.strMessage[0] != "Ok"))
                return false;

            this.Close();
            return true;
        }

        // Function
        //
        private bool handleNetworkMessage(string sendString)
        {
            strMessage[0] = string.Empty;
            Roboard.NetworkClient.SendMessage(sendString);
            watchDogTimer.Start(1000);
            // wait till ok received
            while ((strMessage[0] == string.Empty) && (!watchDogTimer.Done)) ;
            if (watchDogTimer.Done)
            {
                this.Close();
                return false;
            }
            return true;
        }

        // Methods
        //
        private void Close()
        {
            Roboard.NetworkClient.messageHandler -= new NetworkClient.NewMessageEventHandler(WriteNetworkClient_messageHandler);
        }

        // Property
        //
        public IniFile MotionData
        {
            get
            {
                return this.motionData;
            }
            set
            {
                this.motionData = value;
            }
        }

        // Property
        //
        public int SelectedMotionIndex
        {
            get { return Convert.ToInt32(this._selectedMotionIndex); }
            set { this._selectedMotionIndex = value.ToString(); }
        }

        //=========================
        private void WriteNetworkClient_messageHandler(object sender, NewMessageEventsArgs e)
        {
            strMessage =  e.NewMessage.Split(',');
        }
    }
}
