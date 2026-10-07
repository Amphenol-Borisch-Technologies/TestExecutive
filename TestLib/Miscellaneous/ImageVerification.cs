using System;
using System.Drawing;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.Miscellaneous {
    public partial class ImageVerification : Form {
        private PictureBox ImageVerify;
        private Button No;
        private Label Question;
        private Panel PanelControlsInner;
        private Panel PanelPicture;
        private Panel PanelControlsOuter;
        private Button Yes;

        public ImageVerification(String TitleBar, String QuestionText, Image ImageToVerify, IWin32Window FormOwner) {
            InitializeComponent();
            ImageVerify.Image = ImageToVerify;
            this.Text = TitleBar;
            this.Question.Text = QuestionText;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public static DialogResult Show(String TitleBar, String QuestionText, Image ImageToVerify, IWin32Window FormOwner = null) {
            using (ImageVerification imageVerification = new ImageVerification(TitleBar, QuestionText, ImageToVerify, FormOwner)) {
                return imageVerification.ShowDialog(FormOwner);
            }
        }

        private void InitializeComponent() {
            this.ImageVerify = new System.Windows.Forms.PictureBox();
            this.Yes = new System.Windows.Forms.Button();
            this.No = new System.Windows.Forms.Button();
            this.Question = new System.Windows.Forms.Label();
            this.PanelControlsInner = new System.Windows.Forms.Panel();
            this.PanelPicture = new System.Windows.Forms.Panel();
            this.PanelControlsOuter = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.ImageVerify)).BeginInit();
            this.PanelControlsInner.SuspendLayout();
            this.PanelPicture.SuspendLayout();
            this.PanelControlsOuter.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageVerify
            // 
            this.ImageVerify.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImageVerify.Location = new System.Drawing.Point(0, 0);
            this.ImageVerify.Name = "ImageVerify";
            this.ImageVerify.Size = new System.Drawing.Size(1124, 537);
            this.ImageVerify.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageVerify.TabIndex = 2;
            this.ImageVerify.TabStop = false;
            // 
            // Yes
            // 
            this.Yes.BackColor = System.Drawing.Color.Green;
            this.Yes.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Yes.Location = new System.Drawing.Point(343, 3);
            this.Yes.Name = "Yes";
            this.Yes.Size = new System.Drawing.Size(194, 51);
            this.Yes.TabIndex = 1;
            this.Yes.Text = "&Yes";
            this.Yes.UseVisualStyleBackColor = false;
            this.Yes.Click += new System.EventHandler(this.Yes_Click);
            // 
            // No
            // 
            this.No.BackColor = System.Drawing.Color.Red;
            this.No.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.No.Location = new System.Drawing.Point(579, 3);
            this.No.Name = "No";
            this.No.Size = new System.Drawing.Size(194, 51);
            this.No.TabIndex = 2;
            this.No.Text = "&No";
            this.No.UseVisualStyleBackColor = false;
            this.No.Click += new System.EventHandler(this.No_Click);
            // 
            // Question
            // 
            this.Question.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Question.AutoSize = true;
            this.Question.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Question.Location = new System.Drawing.Point(191, 8);
            this.Question.Name = "Question";
            this.Question.Size = new System.Drawing.Size(719, 32);
            this.Question.TabIndex = 3;
            this.Question.Text = "Does the above expected image match the actual?";
            // 
            // PanelControlsInner
            // 
            this.PanelControlsInner.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PanelControlsInner.AutoSize = true;
            this.PanelControlsInner.Controls.Add(this.No);
            this.PanelControlsInner.Controls.Add(this.Yes);
            this.PanelControlsInner.Location = new System.Drawing.Point(5, 35);
            this.PanelControlsInner.Name = "PanelControlsInner";
            this.PanelControlsInner.Size = new System.Drawing.Size(1116, 57);
            this.PanelControlsInner.TabIndex = 4;
            // 
            // PanelPicture
            // 
            this.PanelPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelPicture.Controls.Add(this.ImageVerify);
            this.PanelPicture.Location = new System.Drawing.Point(18, 25);
            this.PanelPicture.Name = "PanelPicture";
            this.PanelPicture.Size = new System.Drawing.Size(1124, 537);
            this.PanelPicture.TabIndex = 5;
            // 
            // PanelControlsOuter
            // 
            this.PanelControlsOuter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelControlsOuter.Controls.Add(this.PanelControlsInner);
            this.PanelControlsOuter.Controls.Add(this.Question);
            this.PanelControlsOuter.Location = new System.Drawing.Point(18, 568);
            this.PanelControlsOuter.Name = "PanelControlsOuter";
            this.PanelControlsOuter.Size = new System.Drawing.Size(1124, 95);
            this.PanelControlsOuter.TabIndex = 6;
            this.PanelControlsOuter.Resize += new System.EventHandler(this.PanelControlsOuter_Resize);
            // 
            // ImageVerification
            // 
            this.ClientSize = new System.Drawing.Size(1165, 675);
            this.Controls.Add(this.PanelPicture);
            this.Controls.Add(this.PanelControlsOuter);
            this.Font = new System.Drawing.Font("Consolas", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ImageVerification";
            this.ShowIcon = false;
            this.Text = "Image Verification";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.ImageVerify)).EndInit();
            this.PanelControlsInner.ResumeLayout(false);
            this.PanelPicture.ResumeLayout(false);
            this.PanelControlsOuter.ResumeLayout(false);
            this.PanelControlsOuter.PerformLayout();
            this.ResumeLayout(false);

        }

        private void Yes_Click(Object sender, EventArgs e) {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void No_Click(Object sender, EventArgs e) {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void PanelControlsOuter_Resize(Object sender, EventArgs e) {
            Question.Left = (PanelControlsOuter.Width - Question.Width) / 2;
            PanelControlsInner.Left = (PanelControlsOuter.Width - PanelControlsInner.Width) / 2;
        }
    }
}
