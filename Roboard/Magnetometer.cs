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
    public class Magnetometer
    {
        private string[] saReturnMessage = new string[StaticUtilities.numberOfMagnetoMeterAxis];
        private string sendString;

        /// <summary>
        /// Magnetometer constructor.
        /// </summary>
        public Magnetometer()
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
            this.sendString = "MagnetoData";
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
                for (int index = 0; index < StaticUtilities.numberOfMagnetoMeterAxis; index++)
                {
                    saReturnMessage[index] = "0";
                }
                this.ChangeMagneticfield(saReturnMessage);
                
                return;
            }

            string[] tmp = new string[StaticUtilities.numberOfMagnetoMeterAxis + 1];
            tmp = e.NewMessage.Split(',');
            if (tmp[0] == "MagnetoData")
            {
                Array.Copy(tmp, 1, saReturnMessage, 0, StaticUtilities.numberOfMagnetoMeterAxis);
            }
            else
            {
                for (int index = 0; index < StaticUtilities.numberOfMagnetoMeterAxis; index++)
                {
                    saReturnMessage[index] = "0";
                }
            }
            this.ChangeMagneticfield(saReturnMessage);
            Roboard.NetworkClient.SendMessage("MagnetoData");
        }

//============================================================================

        // Now, create a public event "MagneticFieldChangeEventHandler" 
        // whose type is our MagneticFieldChangeEventHandler.
        //
        /// <summary>
        /// MagneticField Change Event
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public event MagneticFieldChangeEventHandler MagneticfieldChange;

        // This will be our starting point of our event -- it will
        // create MagneticfieldChangeEventArgs, and then raises
        // the event, passing MagneticfieldChangeEventArgs.
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="acceleration"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private void ChangeMagneticfield(string[] magneticfield)
        {
            MagneticfieldChangeEventArgs MagneticfieldEvents = new MagneticfieldChangeEventArgs(magneticfield);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // MagneticfieldChangeEventArgs.
            // The call must match the signature of the MagneticFieldChangeEventHandler.
            if (MagneticfieldChange != null)
            {
                MagneticfieldChange(this, MagneticfieldEvents);
            }
        }
    }
}