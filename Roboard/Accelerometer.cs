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
    /// This class represents a Roboard Accelerometer. All methods to read acceleration data
    /// from an accelerometer are implemented in this class.
    /// </summary>
    /// <remarks>
    /// The Roboard Accelerometer provides 3 axes of acceleration data, at anywhere from 1g to 6.5g
    /// sensitivity, depending on the specific revision. See your hardware documentation for more information.
    /// Tey can measure both static (gravity) and dynamic acceleration.
    /// </remarks>
    public class Accelerometer
    {
        private string[] saReturnMessage = new string[StaticUtilities.numberOfAcceleroMeterAxis];
        private string sendString;
        
        /// <summary>
        /// Accelerometer constructor.
        /// </summary>
        public Accelerometer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool Start()
        {
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
            this.sendString = "AcceleroData";
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void NetworkClient_messageHandler(object sender, NewMessageEventsArgs e)
        {
            if (e.NewMessage == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.")
            {
                Roboard.NetworkClient.CloseConnection();
                for (int index = 0; index < StaticUtilities.numberOfAcceleroMeterAxis; index++)
                {
                    saReturnMessage[index] = "0";
                }
                this.ChangeAcceleration(saReturnMessage);

                return;
            }

            string[] tmp = new string[StaticUtilities.numberOfAcceleroMeterAxis + 1];
            tmp = e.NewMessage.Split(',');
            if (tmp[0] == "AcceleroData")
            {
                Array.Copy(tmp, 1, saReturnMessage, 0, StaticUtilities.numberOfAcceleroMeterAxis);
            }
            else
            {
                for (int index = 0; index < StaticUtilities.numberOfAcceleroMeterAxis; index++)
                {
                    saReturnMessage[index] = "0";
                }
            }
            this.ChangeAcceleration(saReturnMessage);
            Roboard.NetworkClient.SendMessage("AcceleroData");

        }

//============================================================================

        // Now, create a public event "AccelerationChangeEventHandler" 
        // whose type is our AccelerationChangeEventHandler.
        //
        /// <summary>
        /// Acceleration Change Event
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event AccelerationChangeEventHandler AccelerationChange;

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
        private void ChangeAcceleration(string[] acceleration)
        {
            AccelerationChangeEventArgs AccelerationEvents = new AccelerationChangeEventArgs(acceleration);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // AccelerationChangeEventArgs.
            // The call must match the signature of the AccelerationChangeEventHandler.
            if (AccelerationChange != null)
            {
                AccelerationChange(this, AccelerationEvents);
            }
        }
    }
}