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
        // Private static variables
        private static int DataTableCounter;
        private static bool ReadDataTabledone = false;
        public static string[,] motionDataTable;
        private static string[] strMessage;

        public static void Start()
        {
            DataTableCounter = 1;
            ReadDataTabledone = false;
            Roboard.NetworkClient.messageHandler += new Roboard.NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        public static bool readMotionDataTable()
        {
            ReadDataTabledone = false;
            Roboard.NetworkClient.SendMessage("DataTable,Open");
            Roboard.NetworkClient.SendMessage(string.Format("DataTable,Get,{0}", DataTableCounter));
            return true;
        }

        public static void Stop()
        {
            Roboard.NetworkClient.messageHandler -= new Roboard.NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        private static void NetworkClient_messageHandler(object sender, Roboard.NewMessageEventsArgs e)
        {
            strMessage = e.NewMessage.Split(',');
            switch (strMessage[0])
            {
                case "Open":
                    // Read the number of motions from the server and place it in the
                    // staticutilities class
                    StaticUtilities.numberOfMotions = Convert.ToInt32(strMessage[1]);
                    // Read the number of DataTable Items from the server and place it in the
                    // staticutilities class
                    StaticUtilities.numberOfDataTableItems = Convert.ToInt32(strMessage[2]);
                    // Create the motionDataTable.
                    motionDataTable = new string[StaticUtilities.numberOfMotions, StaticUtilities.numberOfDataTableItems];
                    break;
                case "Get":
                    for (int i = 0; i < StaticUtilities.numberOfDataTableItems; i++)
                        motionDataTable[DataTableCounter - 1, i] = strMessage[i + 1];

                    // next motion to read
                    if (DataTableCounter < StaticUtilities.numberOfMotions)
                    {
                        DataTableCounter++;
                    }
                    else
                    {
                        // stop the DataTable instance.
                        DataTableCounter = 1;
                        ReadDataTabledone = true;
                        return;
                    }
                    Roboard.NetworkClient.SendMessage(string.Format("DataTable,Get,{0}", DataTableCounter));
                    break;
                default:
                    break;
            }
        }

        // Property
        //
        public static bool Done
        {
            get
            {
                return ReadDataTabledone;
            }
        }
    }
}
