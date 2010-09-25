using System;
using System.Collections.Generic;
using System.Text;

namespace Roboard
{
    // Class with a function that creates the eventargs and initiates the event.
    //
    public class TrimServos
    {
        private Roboard.TimeOut watchDogTimer;
        private static string sReturnMessage;
        private string[] saReturnMessage;
        private string sendString;

        private string[] strTrimData = new string[StaticUtilities.numberOfServos]
        {
            "0","0","0","0","0","0","0","0",
            "0","0","0","0","0","0","0","0",
            "0","0","0","0","0","0","0","0"
        };

        public TrimServos()
        {
        }

        public bool Start()
        {
            // create a new instance of the class that will be firing an event
            // chat connection to the server
            //
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
            this.sendString = "Trim,Get";
            if (handleNetworkMessage(this.sendString))
            {
                this.ReceivedTrimServos(saReturnMessage);
                return true;
            }
            return false;
        }

        public void Stop()
        {
            Roboard.NetworkClient.SendMessage("Trim,Stop");
            Roboard.NetworkClient.messageHandler -= new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
        }

        // This function will send the trim value of the changed
        // servo to the server.
        //
        public void changeAllChannels(string[] saTrimData)
        {
            sendString = string.Format("Trim,Set,{0}", string.Join(",", saTrimData));
            handleNetworkMessage(this.sendString);
        }

        // Method
        //
        private bool handleNetworkMessage(string sendString)
        {
            watchDogTimer = new TimeOut();

            sReturnMessage = string.Empty;
            Roboard.NetworkClient.SendMessage(sendString);
            watchDogTimer.Start(1000);
            // wait till ok received
            while ((sReturnMessage == string.Empty) && (!watchDogTimer.Done)) ;
            // hier moet nog iets komen om te kijken of de sReturnMessage gevuld is.
            saReturnMessage = sReturnMessage.Split(',');
            if (watchDogTimer.Done)
            {
                return false;
            }
            return true;
        }

        private void NetworkClient_messageHandler(object sender, NewMessageEventsArgs e)
        {
            if (e.NewMessage == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.")
            {
                Roboard.NetworkClient.CloseConnection();
                return;
            }

            sReturnMessage = Convert.ToString(e.NewMessage);
        }

//=============================================================================
        // Events are handled with delegates, so we must establish a
        // TrimFrameChangeEventHandler as a delegate:
        //
        public delegate void TrimServosEventHandler(object sender, TrimServosEventArgs e);

        // Now, create a public event "AccelerationChangeEventHandler" 
        // whose type is our AccelerationChangeEventHandler.
        //
        public event TrimServosEventHandler TrimServosHandler;

        // This will be our starting point of our event -- it will
        // create AccelerationChangeEventArgs, and then raises
        // the event, passing AccelerationChangeEventArgs.
        //
        private void ReceivedTrimServos(string[] trimservos)
        {
            TrimServosEventArgs TrimServosEvents = new TrimServosEventArgs(trimservos);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // AccelerationChangeEventArgs.
            // The call must match the signature of the AccelerationChangeEventHandler.
            if (TrimServosHandler != null)
            {
                TrimServosHandler(this, TrimServosEvents);
            }
        }
    }

    // Class that contains the data for the trim servos events.
    // Derives from System.Eventargs.
    //
    public class TrimServosEventArgs : EventArgs
    {
        // The acceleration event will have one pieces of information--
        // 1) which axis and 2) the acceleration.
        //
        private string[] trimservos;

        //  Default Constructor
        //
        public TrimServosEventArgs(string[] trimservos)
        {
            this.trimservos = trimservos;
        }

        // The Index property returns the reference of the axis index
        // from which this event originated.
        //
        public string[] TrimServos
        {
            get { return this.trimservos; }
            set { this.trimservos = value; }
        }
    }
}
