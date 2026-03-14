namespace ABT.Test.TestExecutive.TestLib.Miscellaneous {
    partial class UUT_Connections {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Completed = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 2000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // Completed
            // 
            this.Completed.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Completed.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Completed.Location = new System.Drawing.Point(329, 400);
            this.Completed.Name = "Completed";
            this.Completed.Size = new System.Drawing.Size(138, 38);
            this.Completed.TabIndex = 0;
            this.Completed.Text = "&Completed";
            this.Completed.UseVisualStyleBackColor = true;
            this.Completed.Click += new System.EventHandler(this.Completed_Click);
            // 
            // _13764504_1_Connections
            // 
            this.AcceptButton = this.Completed;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(788, 450);
            this.ControlBox = false;
            this.Controls.Add(this.Completed);
            this.DoubleBuffered = true;
            this.Name = "_13764504_1_Connections";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect as pictured.";
            this.toolTip.SetToolTip(this, "Close when completed.");
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button Completed;
    }
}