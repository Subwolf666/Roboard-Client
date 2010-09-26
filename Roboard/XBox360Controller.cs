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
    public class XBox360Controller
    {
        private string[] saReturnMessage;
        private string sendString;

        /// <summary>
        /// XBox360 Controller constructor.
        /// </summary>
        public XBox360Controller()
        {
        }

        /// <summary>
        /// Start reading data from the XBox360 Controller
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
        /// Stop reading data from the XBox360 Controller.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Stop()
        {
            Roboard.NetworkClient.messageHandler -= new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        /// <summary>
        /// Start reading data from the XBox360 Controller.
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
                this.ChangeXBox360Controller(saReturnMessage);

                return;
            }
            saReturnMessage = e.NewMessage.Split(',');
            this.ChangeXBox360Controller(saReturnMessage);
            Roboard.NetworkClient.SendMessage("XBox360Controller,Read");
        }

        /// <summary>
        /// Xbox360 Controller Change Event Handler
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event XBox360ControllerChangeEventHandler XBox360ControllerChange;

        /// <summary>
        /// This will create XBox360ControllerChangeEventArgs, and then raises
        /// the event, passing XBox360ControllerChangeEventArgs.
        /// </summary>
        /// <param name="XBox360Controller"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ChangeXBox360Controller(string[] XBox360Controller)
        {
            XBox360ControllerChangeEventArgs XBox360ControllerEvents = new XBox360ControllerChangeEventArgs(XBox360Controller);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // XBox360ControllerChangeEventArgs.
            // The call must match the signature of the XBox360ControllerChangeEventHandler.
            if (XBox360ControllerChange != null)
            {
                XBox360ControllerChange(this, XBox360ControllerEvents);
            }
        }
    }
}
