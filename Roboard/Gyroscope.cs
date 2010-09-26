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
    /// This class represents a Roboard Gyroscope. All methods to read Gyro data
    /// from an gyroscope are implemented in this class.
    /// </summary>
    /// <remarks>
    /// The Roboard Gyroscope provides 3 axes of magneticfield data, at anywhere from ******
    /// sensitivity, depending on the specific revision. See your hardware documentation for more information.
    /// Tey can measure both static (gravity) and dynamic acceleration.
    /// </remarks>
    public class Gyroscope
    {
        private string[] saReturnMessage = new string[StaticUtilities.numberOfGyroscopeAxis];
        private string sendString;

        /// <summary>
        /// Gyroscope constructor.
        /// </summary>
        public Gyroscope()
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
            this.sendString = "GyroscopeData";
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
        /// This is the function to be executed for generating 
        /// the GyropscopeChange.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NetworkClient_messageHandler(object sender, NewMessageEventsArgs e)
        {
            if (e.NewMessage == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.")
            {
                Roboard.NetworkClient.CloseConnection();
                for (int index = 0; index < StaticUtilities.numberOfGyroscopeAxis; index++)
                {
                    saReturnMessage[index] = "0";
                }
                this.ChangeGyroscope(saReturnMessage);

                return;
            }

            string[] tmp = new string[StaticUtilities.numberOfGyroscopeAxis + 1];
            tmp = e.NewMessage.Split(',');
            if (tmp[0] == "GyroscopeData")
            {
                Array.Copy(tmp, 1, saReturnMessage, 0, StaticUtilities.numberOfGyroscopeAxis);
            }
            else
            {
                for (int index = 0; index < StaticUtilities.numberOfGyroscopeAxis; index++)
                {
                    saReturnMessage[index] = "0";
                }
            }
            this.ChangeGyroscope(saReturnMessage);
            Roboard.NetworkClient.SendMessage("GyroscopeData");
        }

//============================================================================

        // Now, create a public event "GyroscopeChangeEventHandler" 
        // whose type is our GyroscopeChangeEventHandler.
        //
        /// <summary>
        /// Gyroscope Change Event
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event GyroscopeChangeEventHandler GyroscopeChange;

        // This will be our starting point of our event -- it will
        // create GyroscopeChangeEventArgs, and then raises
        // the event, passing GyroscopeChangeEventArgs.
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acceleration"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ChangeGyroscope(string[] gyroscope)
        {
            GyroscopeChangeEventArgs GyroscopeEvents = new GyroscopeChangeEventArgs(gyroscope);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // AccelerationChangeEventArgs.
            // The call must match the signature of the AccelerationChangeEventHandler.
            if (GyroscopeChange != null)
            {
                GyroscopeChange(this, GyroscopeEvents);
            }
        }
    }
}
