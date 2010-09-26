using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Roboard
{
    public class TimeOut
    {
        System.Timers.Timer watchDogTimer;
        private static bool watchDogTimedOut;

        public TimeOut()
        {
            watchDogTimer = new System.Timers.Timer();
        }

        public void Start(int milliSeconds)
        {
            watchDogTimedOut = false;
            watchDogTimer.Interval = milliSeconds;
            watchDogTimer.Elapsed += new ElapsedEventHandler(OnWatchDogBark);
            watchDogTimer.Enabled = true;
        }

        public void Stop()
        {
            watchDogTimer.Elapsed -= new ElapsedEventHandler(OnWatchDogBark);
            watchDogTimer.Enabled = false;
        }

        public bool Done
        {
            get { return watchDogTimedOut; }
            set { watchDogTimedOut = value; }
        }

        private void OnWatchDogBark(object source, ElapsedEventArgs e)
        {
            watchDogTimer.Elapsed -= new ElapsedEventHandler(OnWatchDogBark);
            watchDogTimer.Enabled = false;
            watchDogTimedOut = true;
        }
    }
}
