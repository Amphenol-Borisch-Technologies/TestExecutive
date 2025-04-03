using System;
using System.Windows.Forms;

namespace ABT.Test.TestExec.Miscellaneous {
    public partial class MessageBoxMonoSpaced : Form {
        public MessageBoxMonoSpaced(String Title, String Text, String Link) {
            InitializeComponent();
            this.Text = Title;
            this.Label.Text = Text;
            this.Link.Text = Link;
            if (String.Equals(Link, String.Empty)) {
                this.Link.Enabled = false;
                this.Link.Visible = false;
            }
        }

        private void OK_Clicked(Object sender, EventArgs e) {
            this.Close();
        }

        private void Link_Clicked(Object sender, EventArgs e) {
            System.Diagnostics.Process.Start(this.Link.Text);
            this.Link.LinkVisited = true;
        }
    }
}
