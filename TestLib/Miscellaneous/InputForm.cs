using System;
using System.Drawing;
using System.Windows.Forms;

namespace ABT.Test.TestLib.Miscellaneous {
    public partial class InputForm : Form {
        public InputForm() {
            InitializeComponent();
        }

        public static String Show(String Title, Icon OptionalIcon = null) {
            InputForm inputForm = new InputForm {
                Text = Title,
                Icon = (OptionalIcon is null ? SystemIcons.Information : OptionalIcon),
            };
            inputForm.ShowDialog();
            if (inputForm.DialogResult == DialogResult.OK) return inputForm.textInput.Text;
            else return null;
        }

        private void TextInput_Changed(Object sender, EventArgs e) {
            buttonOK.Enabled = textInput.TextLength != 0;
        }

        private void ButtonOK_Clicked(Object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}
