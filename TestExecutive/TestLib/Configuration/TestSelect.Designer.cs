namespace ABT.Test.TestExecutive.TestLib.Configuration {
    public partial class TestSelect : System.Windows.Forms.Form {
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

        #region
        private void InitializeComponent() {
            this.OK = new System.Windows.Forms.Button();
            this.TestList = new System.Windows.Forms.ListView();
            this.Tests = new System.Windows.Forms.GroupBox();
            this.TestGroups = new System.Windows.Forms.RadioButton();
            this.TestOperations = new System.Windows.Forms.RadioButton();
            this.Tests.SuspendLayout();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Enabled = false;
            this.OK.Location = new System.Drawing.Point(324, 383);
            this.OK.Margin = new System.Windows.Forms.Padding(2);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(58, 36);
            this.OK.TabIndex = 2;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // TestList
            // 
            this.TestList.FullRowSelect = true;
            this.TestList.GridLines = true;
            this.TestList.HideSelection = false;
            this.TestList.LabelWrap = false;
            this.TestList.Location = new System.Drawing.Point(12, 2);
            this.TestList.MultiSelect = false;
            this.TestList.Name = "TestList";
            this.TestList.ShowGroups = false;
            this.TestList.Size = new System.Drawing.Size(666, 333);
            this.TestList.TabIndex = 0;
            this.TestList.UseCompatibleStateImageBehavior = false;
            this.TestList.View = System.Windows.Forms.View.Details;
            this.TestList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.TestList_Changed);
            this.TestList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TestList_MouseDoubleClick);
            // 
            // Tests
            // 
            this.Tests.Controls.Add(this.TestGroups);
            this.Tests.Controls.Add(this.TestOperations);
            this.Tests.Location = new System.Drawing.Point(11, 369);
            this.Tests.Name = "Tests";
            this.Tests.Size = new System.Drawing.Size(170, 92);
            this.Tests.TabIndex = 3;
            this.Tests.TabStop = false;
            // 
            // TestGroups
            // 
            this.TestGroups.AutoSize = true;
            this.TestGroups.Location = new System.Drawing.Point(15, 57);
            this.TestGroups.Name = "TestGroups";
            this.TestGroups.Size = new System.Drawing.Size(83, 17);
            this.TestGroups.TabIndex = 1;
            this.TestGroups.TabStop = true;
            this.TestGroups.Text = "Test Groups";
            this.TestGroups.UseVisualStyleBackColor = true;
            this.TestGroups.Click += new System.EventHandler(this.TestGroups_Clicked);
            // 
            // TestOperations
            // 
            this.TestOperations.AutoSize = true;
            this.TestOperations.Location = new System.Drawing.Point(15, 24);
            this.TestOperations.Name = "TestOperations";
            this.TestOperations.Size = new System.Drawing.Size(100, 17);
            this.TestOperations.TabIndex = 0;
            this.TestOperations.TabStop = true;
            this.TestOperations.Text = "Test Operations";
            this.TestOperations.UseVisualStyleBackColor = true;
            this.TestOperations.Click += new System.EventHandler(this.TestOperations_Clicked);
            // 
            // TestSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 479);
            this.ControlBox = false;
            this.Controls.Add(this.Tests);
            this.Controls.Add(this.TestList);
            this.Controls.Add(this.OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TestSelect";
            this.TopMost = true;
            this.Tests.ResumeLayout(false);
            this.Tests.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.ListView TestList;
        private System.Windows.Forms.GroupBox Tests;
        private System.Windows.Forms.RadioButton TestGroups;
        private System.Windows.Forms.RadioButton TestOperations;
    }
}