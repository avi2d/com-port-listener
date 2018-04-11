using System.ComponentModel;
using System.IO.Ports;

namespace SerialPortListener
{
    /// <summary>
    /// Class containing properties related to a serial port 
    /// </summary>
    public class SerialSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _portName = "";

        #region Properties
        /// <summary>
        /// The port to use (for example, COM1).
        /// </summary>
        public string PortName
        {
            get => _portName;
            set
            {
                if (_portName.Equals(value)) return;
                _portName = value;
                SendPropertyChangedEvent("PortName");
            }
        }
        
        /// <summary>
        /// The baud rate.
        /// </summary>
        public int BaudRate { get; } = 9600;

        /// <summary>
        /// One of the Parity values.
        /// </summary>
        public Parity Parity { get; } = Parity.None;

        /// <summary>
        /// The data bits value.
        /// </summary>
        public int DataBits { get; } = 8;

        /// <summary>
        /// One of the StopBits values.
        /// </summary>
        public StopBits StopBits { get; } = StopBits.One;

        /// <summary>
        /// Available ports on the computer
        /// </summary>
        public string[] PortNameCollection { get; set; }

        /// <summary>
        /// Available baud rates for current serial port
        /// </summary>
        public BindingList<int> BaudRateCollection { get; } = new BindingList<int>();

        /// <summary>
        /// Available databits setting
        /// </summary>
        public int[] DataBitsCollection { get; } = { 5, 6, 7, 8 };

        #endregion

        #region Methods
        /// <summary>
        /// Updates the range of possible baud rates for device
        /// </summary>
        /// <param name="possibleBaudRates">dwSettableBaud parameter from the COMMPROP Structure</param>
        /// <returns>An updated list of values</returns>
        public void UpdateBaudRateCollection(int possibleBaudRates)
        {
            const int BAUD_075 = 0x00000001;
            const int BAUD_110 = 0x00000002;
            const int BAUD_150 = 0x00000008;
            const int BAUD_300 = 0x00000010;
            const int BAUD_600 = 0x00000020;
            const int BAUD_1200 = 0x00000040;
            const int BAUD_1800 = 0x00000080;
            const int BAUD_2400 = 0x00000100;
            const int BAUD_4800 = 0x00000200;
            const int BAUD_7200 = 0x00000400;
            const int BAUD_9600 = 0x00000800;
            const int BAUD_14400 = 0x00001000;
            const int BAUD_19200 = 0x00002000;
            const int BAUD_38400 = 0x00004000;
            const int BAUD_56K = 0x00008000;
            const int BAUD_57600 = 0x00040000;
            const int BAUD_115200 = 0x00020000;
            const int BAUD_128K = 0x00010000;

            BaudRateCollection.Clear();

            if ((possibleBaudRates & BAUD_075) > 0)
                BaudRateCollection.Add(75);
            if ((possibleBaudRates & BAUD_110) > 0)
                BaudRateCollection.Add(110);
            if ((possibleBaudRates & BAUD_150) > 0)
                BaudRateCollection.Add(150);
            if ((possibleBaudRates & BAUD_300) > 0)
                BaudRateCollection.Add(300);
            if ((possibleBaudRates & BAUD_600) > 0)
                BaudRateCollection.Add(600);
            if ((possibleBaudRates & BAUD_1200) > 0)
                BaudRateCollection.Add(1200);
            if ((possibleBaudRates & BAUD_1800) > 0)
                BaudRateCollection.Add(1800);
            if ((possibleBaudRates & BAUD_2400) > 0)
                BaudRateCollection.Add(2400);
            if ((possibleBaudRates & BAUD_4800) > 0)
                BaudRateCollection.Add(4800);
            if ((possibleBaudRates & BAUD_7200) > 0)
                BaudRateCollection.Add(7200);
            if ((possibleBaudRates & BAUD_9600) > 0)
                BaudRateCollection.Add(9600);
            if ((possibleBaudRates & BAUD_14400) > 0)
                BaudRateCollection.Add(14400);
            if ((possibleBaudRates & BAUD_19200) > 0)
                BaudRateCollection.Add(19200);
            if ((possibleBaudRates & BAUD_38400) > 0)
                BaudRateCollection.Add(38400);
            if ((possibleBaudRates & BAUD_56K) > 0)
                BaudRateCollection.Add(56000);
            if ((possibleBaudRates & BAUD_57600) > 0)
                BaudRateCollection.Add(57600);
            if ((possibleBaudRates & BAUD_115200) > 0)
                BaudRateCollection.Add(115200);
            if ((possibleBaudRates & BAUD_128K) > 0)
                BaudRateCollection.Add(128000);

            SendPropertyChangedEvent("BaudRateCollection");
        }

        /// <summary>
        /// Send a PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of changed property</param>
        private void SendPropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
