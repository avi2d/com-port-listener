using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection;

namespace SerialPortListener
{
    /// <summary>
    /// Manager for serial port data
    /// </summary>
    public sealed class SerialPortManager : IDisposable
    {
        public SerialPortManager()
        {
            // Finding installed serial ports on hardware
            CurrentSerialSettings.PortNameCollection = SerialPort.GetPortNames();
            CurrentSerialSettings.PropertyChanged += _currentSerialSettings_PropertyChanged;

            // If serial ports is found, we select the first found
            if (CurrentSerialSettings.PortNameCollection.Length > 0)
                CurrentSerialSettings.PortName = CurrentSerialSettings.PortNameCollection[0];
        }

        
        ~SerialPortManager()
        {
            Dispose(false);
        }


        #region Fields
        private SerialPort _serialPort;
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved; 

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current serial port settings
        /// </summary>
        public SerialSettings CurrentSerialSettings { get; } = new SerialSettings();

        #endregion

        #region Event handlers
        private void _currentSerialSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // if serial port is changed, a new baud query is issued
            if (e.PropertyName.Equals("PortName"))
                UpdateBaudRateCollection();
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var dataLength = _serialPort.BytesToRead;
            var data = new byte[dataLength];
            var nbrDataRead = _serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;
            
            // Send data to whom ever interested
            NewSerialDataRecieved?.Invoke(this, new SerialDataEventArgs(data));
        }

        #endregion

        #region Methods
        /// <summary>
        /// Connects to a serial port defined through the current settings
        /// </summary>
        public void StartListening()
        {
            // Closing serial port if it is open
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            // Setting serial port settings
            _serialPort = new SerialPort(
                CurrentSerialSettings.PortName,
                CurrentSerialSettings.BaudRate,
                CurrentSerialSettings.Parity,
                CurrentSerialSettings.DataBits,
                CurrentSerialSettings.StopBits);

            // Subscribe to event and open serial port for data
            _serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.Open();
        }

        /// <summary>
        /// Closes the serial port
        /// </summary>
        public void StopListening()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// Retrieves the current selected device's COMMPROP structure, and extracts the dwSettableBaud property
        /// </summary>
        private void UpdateBaudRateCollection()
        {
            _serialPort = new SerialPort(CurrentSerialSettings.PortName);
            _serialPort.Open();
            var p = _serialPort.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(_serialPort.BaseStream);
            var dwSettableBaud = (int)p.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);

            _serialPort.Close();
            CurrentSerialSettings.UpdateBaudRateCollection(dwSettableBaud);
        }

        // Call to release serial port
        public void Dispose()
        {
            Dispose(true);
        }

        // Part of basic design pattern for implementing Dispose
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort.DataReceived -= _serialPort_DataReceived;
            }
            // Releasing serial port (and other unmanaged objects)
            if (_serialPort == null) return;
            if (_serialPort.IsOpen)
                _serialPort.Close();

            _serialPort.Dispose();
        }


        #endregion

    }

    /// <summary>
    /// EventArgs used to send bytes recieved on serial port
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }

        /// <summary>
        /// Byte array containing data from serial port
        /// </summary>
        public readonly byte[] Data;
    }
}
