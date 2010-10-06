using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Roboard
{
    // Class with a function that creates the eventargs and initiates the event.
    //
    public static class NetworkClient
    {
        private static string myIP;
        public static Int32 timeout = 400;

        private static TcpClient tcpServer;
        private static NetworkStream networkStream;
        private static StreamWriter swSender;
        private static StreamReader srReceiver;
        private static Thread thrMessaging;
        public static bool Connected;

        // Properties
        //
        public static string ServerIPAddress
        {
            get { return myIP; }
            set { myIP = value; }
        }

        public static bool Connect()
        {
            // If we are not currently connected but awaiting to connect
            if (Connected == false)
            {
                // Initialize the connection
                InitializeConnection();
            }
            //else // We are connected, thus disconnect
            //{
            //    CloseConnection();
            //}
            return Connected;
        }

        private static void InitializeConnection()
        {
            try
            {
                int port = 1986;

                // Parse the IP address from the txtIp string into an IPAddress object
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(myIP), port);

                // Start a new TCP connection to the chat server
                tcpServer = TimeOutSocket.Connect(remoteEndPoint, timeout);
                // set the timeout for sender and receiver
                tcpServer.SendTimeout = 5000;
                //tcpServer.ReceiveTimeout = 5000;

                networkStream = tcpServer.GetStream();

                // Send the desired username to the server
                swSender = new StreamWriter(networkStream);
                swSender.WriteLine("KHR-1HV");
                swSender.Flush();

                // Receive the response from the server
                srReceiver = new StreamReader(networkStream);
                
                // If the first character of the response is 1, connection was successful
                string ConResponse = srReceiver.ReadLine();
                // if ConResponse is empty there was no user
                if (ConResponse != null)
                {
                    // If the first character is a 1, connection was successful
                    if (ConResponse[0] == '1')
                    {
                        // Helps us track whether we are connected or not
                        Connected = true;
                    }
                    else // If the first character is not a 1 (probably a 0), the connection was unsuccessful
                    {
                        CloseConnection();
                        // Helps us track whether we are connected or not
                        Connected = false;
                        // Exit the method
                        return;
                    }
                }
                else // If the string = null (probably no username), the connection was unsuccessful
                {
                    CloseConnection();
                    // Exit the method
                    return;
                }

                // Start the thread for receiving messages and further communication
                thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
                thrMessaging.Start();
            }
            catch (Exception ex)
            {
                OnNewMessage(ex.Message);
            }
        }

        public static void ReceiveMessages()
        {
            // While we are successfully connected, read incoming lines from the server
            while (Connected)
            {
                try
                {
                    // Show the messages in the log TextBox
                    string receiveString = srReceiver.ReadLine();
                    OnNewMessage(receiveString);
                }
                catch (Exception ex)
                {
                    //return value zodat de while loop hierop kan testen
                    OnNewMessage(ex.Message);
                }
            }
        }

        // Sends the message typed in to the server
        public static void SendMessage(string strMessage)
        {
            try
            {
                if (strMessage.Length >= 1)
                {
                    swSender.WriteLine(strMessage);
                    swSender.Flush();
                }
            }
            catch (Exception ex)
            {
                // Server is down...
                OnNewMessage(ex.Message);
            }
        }

        // Closes a current connection
        public static void CloseConnection()
        {
            // Check if the network is connected
            if (Connected)
            {
                // Close the objects
                swSender.Close();
                srReceiver.Close();
                tcpServer.Close();
            }
            Connected = false;
        }

        // Events are handled with delegates, so we must establish a
        // NewMessageEventHandler as a delegate:
        //
        public delegate void NewMessageEventHandler(object sender, NewMessageEventsArgs e);

        // Now, create a public event "messageHandler" 
        // whose type is our NewMessageEventHandler.
        //
        public static event NewMessageEventHandler messageHandler;

        // This will be our starting point of our event -- it will
        // create NewMessageEventsArgs, and then raises
        // the event, passing NewMessageEventsArgs.
        //
        public static void OnNewMessage(string str)
        {

            NewMessageEventsArgs MessageEvents = new NewMessageEventsArgs(str);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // NewMessageEventsArgs.
            // The call must match the signature of the NewMessageEventHandler.
            if (messageHandler != null)
            {
                // Invoke the delegate
                messageHandler(null, MessageEvents);
            }
        }
    }

//============================================================================
    // Class that contains the data for the receive messsage events.
    // Derives from System.Eventargs.
    //
    public class NewMessageEventsArgs : EventArgs
    {
        // The receive ressage event will have one piece of information
        // 1) ReceiveMessage
        //
        private string EventMsg;

        // The NewMessage property returns the reference of the
        // message from which this event originated.
        //
        public string NewMessage
        {
            get { return EventMsg; }
            set { EventMsg = value; }
        }

        // Default Constructor
        //
        public NewMessageEventsArgs(string strEventMsg)
        {
            this.EventMsg = strEventMsg;
        }
    }

//=============================================================================
    class TimeOutSocket
    {
        public static TcpClient Connect(IPEndPoint remoteEndPoint, int timeousMSec)
        {
            TcpClient tcpclient = new TcpClient();
            IAsyncResult asyncResult = tcpclient.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, null, null);
            if (asyncResult.AsyncWaitHandle.WaitOne(timeousMSec, false))
            {
                try
                {
                    tcpclient.EndConnect(asyncResult);
                    return tcpclient;
                }
                catch
                {
                    tcpclient.Close();
                    throw;
                }
            }
            else
            {
                tcpclient.Close();
                throw new TimeoutException("TimeOut Exception");
            }
        }
    }
}
