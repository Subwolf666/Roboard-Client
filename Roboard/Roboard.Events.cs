using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Roboard.Events
{
    // Events are handled with delegates, so we must establish a
    // AccelerationChangeEventHandler as a delegate:
    //
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AccelerationChangeEventHandler(object sender, AccelerationChangeEventArgs e);

    // Events are handled with delegates, so we must establish a
    // AccelerationChangeEventHandler as a delegate:
    //
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GyroscopeChangeEventHandler(object sender, GyroscopeChangeEventArgs e);

    // Events are handled with delegates, so we must establish a
    // AccelerationChangeEventHandler as a delegate:
    //
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MagneticFieldChangeEventHandler(object sender, MagneticfieldChangeEventArgs e);

    // Events are handled with delegates, so we must establish a
    // XBox360ControllerChangeEventHandler as a delegate:
    public delegate void XBox360ControllerChangeEventHandler(object sender, XBox360ControllerChangeEventArgs e);

    // Events are handled with delegates, so we must establish a
    // ReceiverChangeEventHandler as a delegate:
    public delegate void DataTableChangeEventHandler(object sender, DataTableChangeEventArgs e);

    // Class that contains the data for the acceleration events.
    // Derives from System.Eventargs.
    //
    /// <summary>
    /// 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AccelerationChangeEventArgs : EventArgs
    {
        // The acceleration event will has one pieces o information--
        // 1) the acceleration.
        //
        /// <summary>
        /// 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        public readonly string[] Acceleration;

        //  Default Constructor
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="acceleration"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AccelerationChangeEventArgs(string[] acceleration)
        {
            // The Acceleration property returns the reference of the acceleration
            // value from which this event originated.
            //
            this.Acceleration = acceleration;
        }
    }

    // Class that contains the data for the gyroscope events.
    // Derives from System.Eventargs.
    //
    public class GyroscopeChangeEventArgs : EventArgs
    {
        // The gyroscope event will has one piece of information--
        // 1) the gyroscope field (strength and/or direction).
        //

        // The Gyroscope property returns the reference of the Gyroscope
        // value from which this event originated.
        //
        [EditorBrowsable(EditorBrowsableState.Always)]
        public readonly string[] Gyroscope;

        //  Default Constructor
        //
        [EditorBrowsable(EditorBrowsableState.Never)]
        public GyroscopeChangeEventArgs(string[] gyroscope)
        {
            this.Gyroscope = gyroscope;
        }
    }

    // Class that contains the data for the compass events.
    // Derives from System.Eventargs.
    //
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MagneticfieldChangeEventArgs : EventArgs
    {
        // The compass event will has one piece of information--
        // 1) the magnetic field (strength and/or direction).
        //
        [EditorBrowsable(EditorBrowsableState.Always)]
        public readonly string[] MagneticField;

        //  Default Constructor
        //
        public MagneticfieldChangeEventArgs(string[] magneticfield)
        {

            // The MagneticField property returns the reference of the magnetometer
            // value from which this event originated.
            //
            this.MagneticField = magneticfield;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class XBox360ControllerChangeEventArgs : EventArgs
    {
        // The XBox360 Controller event will have 1 piece of information--
        // 1) Array of string which holds the state of the XBox360 Controller and
        //    which button, thumbstick and trigger is used.
        //
        [EditorBrowsable(EditorBrowsableState.Always)]
        public readonly string[] XBox360Controller = new string[7];

        //  Default Constructor
        //
        public XBox360ControllerChangeEventArgs(string[] XBox360Controller)
        {
            // The XBox360Controller property returns the reference of the XBox360 Controller
            // value from which this event originated.
            //
            this.XBox360Controller = XBox360Controller;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DataTableChangeEventArgs : EventArgs
    {
        // The DataTable event will have 1 pieces of information--
        // 1) Array of string which holds:
        //    
        //
        [EditorBrowsable(EditorBrowsableState.Always)]
        //        public readonly int Index;
        //        public readonly int State;
        public readonly string[] DataTable = new string[4];

        //  Default Constructor
        //
        public DataTableChangeEventArgs(string[] sDataTable)//int Index, int State)
        {
            // The Index property returns the reference of the axis index
            // from which this event originated.
            //
//            this.Index = Index;

            // The MagneticField property returns the reference of the magnetometer
            // value from which this event originated.
            //
//            this.State = State;
            this.DataTable = sDataTable;
        }
    }
}
