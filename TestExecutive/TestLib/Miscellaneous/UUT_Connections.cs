using System.Drawing;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.Miscellaneous {
    public partial class UUT_Connections : Form {
        public UUT_Connections(Image image) {
            InitializeComponent();
            BackgroundImage = image;
        }

        private void Completed_Click(System.Object sender, System.EventArgs e) {
            Close();
        }
    }
}
