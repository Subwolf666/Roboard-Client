using System;
using System.Collections.Generic;
using System.Text;

namespace Roboard
{

    // Class that contains the data for the trim servos events.
    // Derives from System.Eventargs.
    //
    public class ServosEventArgs : EventArgs
    {
        // The acceleration event will have one pieces of information--
        // 1) which axis and 2) the acceleration.
        //
        private string[] servos;

        //  Default Constructor
        //
        public ServosEventArgs(string[] servos)
        {
            this.servos = servos;
        }


        // The Index property returns the reference of the axis index
        // from which this event originated.
        //
        public string[] Servos
        {
            get { return this.servos; }
            set { this.servos = value; }
        }
    }

    // Class with a function that creates the eventargs and initiates the event.
    //
    public class Servos
    {
//        private Roboard.NetworkClient networkClient;
        
        private string[] strServoData = new string[StaticUtilities.numberOfServos]
        {
            "0","0","0","0","0","0","0","0",
            "0","0","0","0","0","0","0","0",
            "0","0","0","0","0","0","0","0"
        };

        public Servos()
        {
        }

        public bool Start()
        {
            // create a new instance of the class that will be firing an event
            // chat connection to the server
            //
//            networkClient = new NetworkClient();
//            networkClient.UserName = "Servos";
            //if (networkClient.Connect())
            //{
            Roboard.NetworkClient.messageHandler += new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
            return true;
            //}
            //return false;
        }

        public void Stop()
        {
            //if (networkClient.Connected)
            //{
                try
                {
//                    networkClient.SendMessage("Trim,Stop");
                    Roboard.NetworkClient.messageHandler -= new NetworkClient.NewMessageEventHandler(NetworkClient_messageHandler);
//                    networkClient.CloseConnection();
                }
                catch
                {
                }
            //}
        }

        private void NetworkClient_messageHandler(object sender, NewMessageEventsArgs e)
        {
            // dit onderstaande stuk kan ook naar de server:
            // de server checkt of de data verandert en stuurt de veranderde
            // data door; axis en value.
//            if (e.NewUser == "Admin")
            {
                strServoData = e.NewMessage.Split(',');

                this.ReceivedServos(strServoData);
            }
        }

        // Events are handled with delegates, so we must establish a
        // TrimFrameChangeEventHandler as a delegate:
        //
        public delegate void ServosEventHandler(object sender, ServosEventArgs e);

        // Now, create a public event "AccelerationChangeEventHandler" 
        // whose type is our AccelerationChangeEventHandler.
        //
        public event ServosEventHandler ServosHandler;

        // This will be our starting point of our event -- it will
        // create AccelerationChangeEventArgs, and then raises
        // the event, passing AccelerationChangeEventArgs.
        //
        private void ReceivedServos(string[] servos)
        {
            ServosEventArgs ServosEvents = new ServosEventArgs(servos);

            // Now, raise the event by invoking the delegate. Pass in
            // the objects that initiated the event (this) as wel as
            // AccelerationChangeEventArgs.
            // The call must match the signature of the AccelerationChangeEventHandler.
            if (ServosHandler != null)
            {
                ServosHandler(this, ServosEvents);
            }
        }

        // This function will send the trim value of the changed
        // servo to the server.
        //
        public void changeAllChannels(string[] strArray)
        {
            string message = string.Join(",", strArray);
            message = string.Format("Servos,Set,{0}", message);
            Roboard.NetworkClient.SendMessage(message);
        }
    }
}
