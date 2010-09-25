using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Roboard.Events;

namespace Roboard
{
    // Class with a function that creates the eventargs and initiates the event.
    //
    /// <summary>
    /// This class represents a Roboard MagneticField. All methods to read magneticfield data
    /// from an magnetometer are implemented in this class.
    /// </summary>
    /// <remarks>
    /// The Roboard Magnetometer provides 3 axes of magneticfield data, at anywhere from ******
    /// sensitivity, depending on the specific revision. See your hardware documentation for more information.
    /// Tey can measure both static (gravity) and dynamic acceleration.
    /// </remarks>
    public class XBox360Controller
    {
        private string[] saReturnMessage;// = new string[8];
        private string sendString;

//        private string[] strReceiver = new string[] { "0", "0", "0", "0", "0", "0", "0", "0" };

        /// <summary>
        /// XBox360 Controller constructor.
        /// </summary>
        public XBox360Controller()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool Start()
        {
            // create a new instance of the class that will be firing an event
            // chat connection to the server
            //
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
            this.sendString = "XBox360Controller,Open";
            Roboard.NetworkClient.SendMessage(this.sendString);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Stop()
        {
            Roboard.NetworkClient.messageHandler -= new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public void SaveStop(string tmp)
        {
            Roboard.NetworkClient.SendMessage(string.Format("XBox360Controller,Stop,{0}", tmp));
            Roboard.NetworkClient.messageHandler -= new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void NetworkClient_messageHandler(object sender, NewMessageEventsArgs e)
        {
            if (e.NewMessage == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.")
            {
                Roboard.NetworkClient.CloseConnection();
                for (int index = 0; index < 8; index++)
                {
                    saReturnMessage[index] = "0";
                }
                this.ChangeReceiver(saReturnMessage);

                return;
            }
            saReturnMessage = e.NewMessage.Split(',');
            //strReceiver = e.NewMessage.Split(',');
            //this.ChangeReceiver(strReceiver);
            this.ChangeReceiver(saReturnMessage);
            Roboard.NetworkClient.SendMessage("XBox360Controller,Read");
        }

        // Now, create a public event "AccelerationChangeEventHandler" 
        // whose type is our AccelerationChangeEventHandler.
        //
        /// <summary>
        /// Acceleration Change Event
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event ReceiverChangeEventHandler ReceiverChange;

        // This will be our starting point of our event -- it will
        // create AccelerationChangeEventArgs, and then raises
        // the event, passing AccelerationChangeEventArgs.
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acceleration"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ChangeReceiver(string[] Receiver)
        {
            ReceiverChangeEventArgs ReceiverEvents = new ReceiverChangeEventArgs(Receiver);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // AccelerationChangeEventArgs.
            // The call must match the signature of the AccelerationChangeEventHandler.
            if (ReceiverChange != null)
            {
                ReceiverChange(this, ReceiverEvents);
            }
        }
    }
}
