using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        private SerialPortManager _spManager;

        public MainForm()
        {
            InitializeComponent();

            UserInitialization();
        }

        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            var mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(StopBits));

            _spManager.NewSerialDataRecieved += _spManager_NewSerialDataRecieved;
            FormClosing += MainForm_FormClosing;
        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        private void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), sender, e);
                return;
            }

            var str = BitConverter.ToString(e.Data);
            tbData.AppendText($"{str}\n\n");
            SendKeys.SendWait(str);

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
        }
    }
}
