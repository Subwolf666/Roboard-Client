using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Roboard.Events;

namespace Roboard
{
    /// <summary>
    /// This class represents the Roboard XBox360 Controller. All methods to read XBox360 Controller data
    /// from a XBox360 Controller are implemented in this class.
    /// </summary>
    /// <remarks>
    /// The Roboard XBox360 Controller provides 3 axes of magneticfield data, at anywhere from ******
    /// sensitivity, depending on the specific revision. See your hardware documentation for more information.
    /// Tey can measure both static (gravity) and dynamic acceleration.
    /// </remarks>
    public static class DataTable
    {
        private static string[] saReturnMessage;
        private static string sendString;
        private static int DataTableCounter;
        private static bool ReadDataTabledone = false;
        public static string[,] motionDataTable;

        /// <summary>
        /// Start reading data from the DataTable from the server
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public static bool Start()
        {
            DataTableCounter = 1;
            ReadDataTabledone = false;
            Roboard.NetworkClient.messageHandler += new Roboard.NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
            sendString = "DataTable,Open";
            Roboard.NetworkClient.SendMessage(sendString);
            sendString = string.Format("DataTable,Get,{0}", DataTableCounter);
            Roboard.NetworkClient.SendMessage(sendString);
            return true;
        }

        /// <summary>
        /// Stop reading data from the DataTable from the server.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public static void Stop()
        {
            Roboard.NetworkClient.messageHandler -= new Roboard.NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static void NetworkClient_messageHandler(object sender, Roboard.NewMessageEventsArgs e)
        {
            if (e.NewMessage == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.")
            {
                Roboard.NetworkClient.CloseConnection();
                return;
            }

            //strMessage = e.NewMessage.Split(',');
            //switch (strMessage[0])
            saReturnMessage = e.NewMessage.Split(',');
            switch (saReturnMessage[0])
            {
                case "Open":
                    // Read the number of motions from the server and place it in the
                    // staticutilities class
                    StaticUtilities.numberOfMotions = Convert.ToInt32(saReturnMessage[1]);
                    // Read the number of DataTable Items from the server and place it in the
                    // staticutilities class
                    StaticUtilities.numberOfDataTableItems = Convert.ToInt32(saReturnMessage[2]);
                    // Create the motionDataTable.
                    motionDataTable = new string[StaticUtilities.numberOfMotions, StaticUtilities.numberOfDataTableItems];
                    break;
                case "Get":
                    for (int i = 0; i < StaticUtilities.numberOfDataTableItems; i++)
                        motionDataTable[DataTableCounter - 1, i] = saReturnMessage[i + 1];

                    // next motion to read
                    if (DataTableCounter < StaticUtilities.numberOfMotions)
                    {
                        DataTableCounter++;
                        sendString = string.Format("DataTable,Get,{0}", DataTableCounter);
                        Roboard.NetworkClient.SendMessage(sendString);
                        break;
                    }
                    else
                    {
                        // stop the DataTable instance.
                        DataTableCounter = 1;
                        ReadDataTabledone = true;
                        Stop();
                        return;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// Let the user know that the DataTable is finished reading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Done
        {
            get
            {
                return ReadDataTabledone;
            }
        }
    }
}
