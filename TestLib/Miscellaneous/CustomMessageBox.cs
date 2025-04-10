using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.Miscellaneous {
    public partial class CustomMessageBox : Form {
        public CustomMessageBox() {
            InitializeComponent();
        }

        public static void Show(String Title, String Message, Icon OptionalIcon = null) {
            CustomMessageBox cms = new CustomMessageBox {
                Text = Title,
                Icon = (OptionalIcon is null ? SystemIcons.Information : OptionalIcon),
            };
            cms.richTextBox.Text = Message;
            cms.ShowDialog();
        }

        private void ButtonClipboard_Click(Object sender, EventArgs e) { Clipboard.SetText(richTextBox.Text); }

        private void RichTextBox_LinkClicked(Object sender, LinkClickedEventArgs e) {
            try {
                Uri uri = new Uri(e.LinkText);
                if (uri.Scheme == Uri.UriSchemeFile) {
                    String filePath = uri.LocalPath;
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                } else if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) {
                    Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
                } else {
                    MessageBox.Show("Unsupported link type: " + e.LinkText);
                }
            } catch (Exception exception) {
                MessageBox.Show("Error opening link: " + exception.Message);
            }
        }
    }
}
