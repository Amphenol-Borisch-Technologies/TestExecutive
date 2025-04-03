using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.PointOfService;

namespace ABT.Test.TestExec.Logging {
    public sealed partial class SerialNumberDialog : Form {
        private PosExplorer _posExplorer;
        private Scanner _scanner = null;
        private readonly String _serialNumberRegEx;
        private readonly String _scannerID;

        public SerialNumberDialog(String serialNumberRegEx, String serialNumberFormat, String scannerID) {
            _serialNumberRegEx = serialNumberRegEx;
            _scannerID = scannerID;

            InitializeComponent();
            toolTip.SetToolTip(this, serialNumberFormat);

            GetBarcodeScanner();
            FormUpdate(String.Empty);
        }

        public void Set(String serialNumber) { FormUpdate(serialNumber); }

        public String Get() { return BarCodeText.Text; }

        private void GetBarcodeScanner() {
            _posExplorer = new PosExplorer();
            DeviceInfo deviceInfo = _posExplorer.GetDevice(DeviceType.Scanner, _scannerID) ?? throw new InvalidOperationException($"Cannot find Barcode scanner Device ID: '{_scannerID}'");
            _scanner = (Scanner)_posExplorer.CreateInstance(deviceInfo);
            if (_scanner == null) throw new InvalidOperationException("Barcode scanner cannot be initialized.");

            _scanner.DataEvent += Scanner_DataEvent;
            _scanner.ErrorEvent += Scanner_ErrorEvent;

            _scanner.Open();
            _scanner.Claim(1000); // Claim the scanner for exclusive use.
            _scanner.DeviceEnabled = true; // Enable the scanner for use.
        }

        private void Scanner_DataEvent(Object sender, DataEventArgs e) {
            Byte[] scannedBytes = _scanner.ScanDataLabel;
            String scannedString = Encoding.UTF8.GetString(scannedBytes);
            if (!String.IsNullOrEmpty(scannedString)) {
                FormUpdate(scannedString);
            }
        }

        private void Scanner_ErrorEvent(Object sender, DeviceErrorEventArgs e) {
            MessageBox.Show(e.ErrorCode.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void OK_Clicked(Object sender, EventArgs e) { DialogResult = DialogResult.OK; }

        private void Cancel_Clicked(Object sender, EventArgs e) { DialogResult = DialogResult.Cancel; }

        private void FormUpdate(String text) {
            BarCodeText.Text = text;
            if (Regex.IsMatch(text, _serialNumberRegEx)) {
                OK.Enabled = true;
                OK.BackColor = System.Drawing.Color.Green;
                OK.Focus();
            } else {
                OK.Enabled = false;
                OK.BackColor = System.Drawing.Color.DimGray;
            }
        }
    }
}