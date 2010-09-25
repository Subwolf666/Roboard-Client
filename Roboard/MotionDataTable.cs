using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roboard
{
    public static class MotionDataTable
    {
        //private static Roboard.NetworkClient networkClient;

        // Private static variables
        private static int DataTableCounter;
        private static bool ReadDataTabledone = false;
        public static string[,] motionDataTable;
        private static string[] strMessage;

        public static void Init()
        {
//            networkClient = new Roboard.NetworkClient();
//            networkClient.UserName = "MotionDataTable";

            DataTableCounter = 1;
            ReadDataTabledone = false;
            //if (!Roboard.NetworkClient.Connect())
            //{
            //    Close();
            //    return;
            //}

            Roboard.NetworkClient.messageHandler += new Roboard.NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        public static bool readMotionDataTable()
        {
            ReadDataTabledone = false;
            Roboard.NetworkClient.SendMessage("DataTable,Open");

            Roboard.NetworkClient.SendMessage(string.Format("DataTable,Get,{0}", DataTableCounter));
            return true;
        }

        public static void Close()
        {
            //if (Roboard.NetworkClient.messageHandler == null)
            //    return;
            Roboard.NetworkClient.messageHandler -= new Roboard.NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
//            Roboard.NetworkClient.CloseConnection();
        }

        private static void NetworkClient_messageHandler(object sender, Roboard.NewMessageEventsArgs e)
        {
            strMessage = e.NewMessage.Split(',');
            switch (strMessage[0])
            {
                case "Open":
                    StaticUtilities.numberOfMotions = Convert.ToInt32(strMessage[1]);
                    StaticUtilities.numberOfDataTableItems = Convert.ToInt32(strMessage[2]);
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
