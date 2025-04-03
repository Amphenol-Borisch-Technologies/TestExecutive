using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.PointOfService;

namespace ABT.Test.TestExec.Logging {
    public sealed partial class SerialNumberDialog : Form {
        private PosExplorer _posExplorer;
        private Scanner _scanner = null;
        private string _serialNumberRegEx;
        private string _scannerID;

        public SerialNumberDialog(string serialNumberRegEx, string serialNumberFormat, string scannerID) {
            _serialNumberRegEx = serialNumberRegEx;
            _scannerID = scannerID;

            InitializeComponent();
            toolTip.SetToolTip(this, serialNumberFormat);

            GetBarcodeScanner();
            FormUpdate(string.Empty);
        }

        public void Set(string serialNumber) { FormUpdate(serialNumber); }

        public string Get() { return BarCodeText.Text; }

        private void GetBarcodeScanner() {
            _posExplorer = new PosExplorer();
            DeviceInfo deviceInfo = _posExplorer.GetDevice(DeviceType.Scanner, _scannerID);
            if (deviceInfo == null) throw new InvalidOperationException($"Cannot find Barcode scanner Device ID: '{_scannerID}'");

            _scanner = (Scanner)_posExplorer.CreateInstance(deviceInfo);
            if (_scanner == null) throw new InvalidOperationException("Barcode scanner cannot be initialized.");

            _scanner.DataEvent += Scanner_DataEvent;
            _scanner.ErrorEvent += Scanner_ErrorEvent;

            _scanner.Open();
            _scanner.Claim(1000); // Claim the scanner for exclusive use.
            _scanner.DeviceEnabled = true; // Enable the scanner for use.
        }

        private void Scanner_DataEvent(object sender, DataEventArgs e) {
            string scannedData = _scanner.ScanDataLabel;
            if (!string.IsNullOrEmpty(scannedData)) {
                FormUpdate(scannedData);
            }
        }

        private void Scanner_ErrorEvent(object sender, DeviceErrorEventArgs e) {
            MessageBox.Show(e.ErrorCode.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void OK_Clicked(object sender, EventArgs e) { DialogResult = DialogResult.OK; }

        private void Cancel_Clicked(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; }

        private void FormUpdate(string text) {
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